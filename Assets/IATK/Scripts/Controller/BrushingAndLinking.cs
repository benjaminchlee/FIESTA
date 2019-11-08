using IATK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using Photon.Pun;
using UnityEngine.Events;
using UnityEngine.Rendering;
using VRTK.Examples;

public class BrushingAndLinking : MonoBehaviourPunCallbacks, IPunObservable {

    [SerializeField]
    public ComputeShader computeShader;

    private int kernelHandleBrushTexture;
    private int kernelHandleBrushArrayIndices;
    private int kernelResetBrushTexture;
    private int kernelComputeNearestDistances;

    ComputeBuffer buffer;
    ComputeBuffer brushedIndicesBuffer;
    ComputeBuffer filteredIndicesBuffer;
    ComputeBuffer nearestDistancesBuffer;
    
    [SerializeField]
    public Material myRenderMaterial;

    public static RenderTexture brushedIndicesTexture;
    public static int texSize;

    [SerializeField]
    public List<Visualisation> brushingVisualisations;
    [SerializeField]
    public List<Visualisation> brushedVisualisations;
    [SerializeField]
    public List<LinkingVisualisations> brushedLinkingVisualisations;

    public bool shareBrushing = false;
    [SerializeField]
    public Color privateBrushColor = Color.yellow;
    [SerializeField]
    public Color sharedBrushColor = Color.red;

    [SerializeField]
    public Transform input1;
    [SerializeField]
    public Transform input2;

    [SerializeField] [Range(0f, 1f)]
    public float radiusSphere;
    [SerializeField]
    public bool brushEnabled;

    public Visualisation visualisationToInspect;
    [SerializeField] [Range(0f, 1f)]
    public float radiusInspector;
    [SerializeField]
    public bool inspectButtonController;

    public BrushType BRUSH_TYPE;
    public enum BrushType
    {
        SPHERE = 0,
        BOX = 1
    };

    public SelectionType SELECTION_TYPE;
    public enum SelectionType
    {
        FREE = 0,
        ADDITIVE = 1,
        SUBTRACTIVE = 2
    };

    public Material debugObjectTexture;

    private AsyncGPUReadbackRequest detailsOnDemandRequest;
    public List<int> brushedIndices;
    private AsyncGPUReadbackRequest nearestDistancesRequest;
    public List<float> nearestDistances;
    
    [Serializable]
    public class NearestDistancesComputedEvent : UnityEvent<List<float>> { }
    public NearestDistancesComputedEvent NearestDistancesComputed;

    public Color PrivateBrushColor
    {
        get { return privateBrushColor; }
        set
        {
            photonView.RPC("PrivateBrushColorRPC", RpcTarget.AllBuffered, value);
        }
    }

    [PunRPC]
    private void PrivateBrushColorRPC(Color value)
    {
        privateBrushColor = value;

        computeShader.SetFloats("brushColor", privateBrushColor.r, privateBrushColor.g, privateBrushColor.b, privateBrushColor.a);
    }

    public Color SharedBrushColor
    {
        get { return sharedBrushColor; }
        set
        {
            photonView.RPC("SharedBrushColorRPC", RpcTarget.AllBuffered, value);
        }
    }

    [PunRPC]
    private void SharedBrushColorRPC(Color value)
    {
        sharedBrushColor = value;
        
        computeShader.SetFloats("sharedBrushColor", sharedBrushColor.r, sharedBrushColor.g, sharedBrushColor.b, sharedBrushColor.a);
    }

    private void Awake()
    {
        // Create a copy of the compute shader that is specific to this brushing and linking script
        computeShader = Instantiate(computeShader);
    }

    // Use this for initialization
    private void Start()
    {
        ChartManager.Instance.ChartAdded.AddListener(ChartAdded);

        brushingVisualisations = new List<Visualisation>();

        InitialiseShaders();
        InitialiseBuffersAndTextures();

        ResetBrushTexture();
    }
    
    private void Update()
    {
        if (brushEnabled)
        {
            // Get list of visualisations that the brush is touching
            switch (BRUSH_TYPE)
            {
                case BrushType.SPHERE:
                    Collider[] colliders = Physics.OverlapSphere(input1.position, radiusSphere);
                    brushingVisualisations.Clear();
                    foreach (Collider col in colliders)
                    {
                        if (col.gameObject.CompareTag("Chart"))
                            brushingVisualisations.Add(col.gameObject.GetComponent<Chart>().Visualisation);
                    }
                    break;

                case BrushType.BOX:
                    Debug.LogError("Box brushing not fully implemented nor tested.");
                    break;
            }

            // Update the brush texture (i.e. get brushed points) only when there is at least one touching visualisation
            if (brushingVisualisations.Count != 0)
            {
                updateBrushTexture();
            }
        }

        // Called for details on demand
        if (inspectButtonController && visualisationToInspect != null)
        {
            updateNearestDistances();
        }
    }

    public void InitialiseShaders()
    {
        kernelHandleBrushTexture = computeShader.FindKernel("CSMain");
        kernelHandleBrushArrayIndices = computeShader.FindKernel("ComputeBrushedIndicesArray");
        kernelResetBrushTexture = computeShader.FindKernel("ResetBrushTexture");
        kernelComputeNearestDistances = computeShader.FindKernel("ComputeNearestDistances");
    }

    public void InitialiseBuffersAndTextures()
    {
        int datasetSize = ChartManager.Instance.DataSource.DataCount;

        buffer = new ComputeBuffer(datasetSize, 12);
        buffer.SetData(new Vector3[datasetSize]);
        computeShader.SetBuffer(kernelHandleBrushTexture, "dataBuffer", buffer);

        brushedIndicesBuffer = new ComputeBuffer(datasetSize, 4);
        int[] brushIni = new int[datasetSize];
        for (int i = 0; i < datasetSize; i++)
            brushIni[i] = -1;
        brushedIndicesBuffer.SetData(brushIni);

        filteredIndicesBuffer = new ComputeBuffer(datasetSize, 4);
        filteredIndicesBuffer.SetData(new float[datasetSize]);

        nearestDistancesBuffer = new ComputeBuffer(datasetSize, 4);
        nearestDistancesBuffer.SetData(new float[datasetSize]);

        computeShader.SetBuffer(kernelHandleBrushArrayIndices, "dataBuffer", buffer);
        computeShader.SetBuffer(kernelHandleBrushArrayIndices, "brushedIndices", brushedIndicesBuffer);
        computeShader.SetBuffer(kernelComputeNearestDistances, "dataBuffer", buffer);
        computeShader.SetBuffer(kernelComputeNearestDistances, "nearestDistances", nearestDistancesBuffer);

        texSize = computeTextureSize(datasetSize);

        // If a shared render texture has not yet been created, make it now
        if (brushedIndicesTexture == null)
        {
            //texSize = computeTextureSize(datasetSize);

            brushedIndicesTexture = new RenderTexture(texSize, texSize, 24);
            brushedIndicesTexture.enableRandomWrite = true;
            brushedIndicesTexture.filterMode = FilterMode.Point;
            brushedIndicesTexture.Create();
        }

        myRenderMaterial.SetTexture("_MainTex", brushedIndicesTexture);

        computeShader.SetTexture(kernelHandleBrushTexture, "Result", brushedIndicesTexture);
        computeShader.SetTexture(kernelHandleBrushArrayIndices, "Result", brushedIndicesTexture);
        computeShader.SetTexture(kernelResetBrushTexture, "Result", brushedIndicesTexture);

        computeShader.SetFloat("_size", (float)texSize);

        // Set colors of brushing and share them to others via RPC only if this belongs to the player
        if (photonView.IsMine)
        {
            PrivateBrushColor = PlayerPreferencesManager.Instance.PrivateBrushColor;
            SharedBrushColor = PlayerPreferencesManager.Instance.SharedBrushColor;
        }
    }

    int computeTextureSize(int sizeDatast)
    {
        return NextPowerOf2((int)Mathf.Sqrt((float)sizeDatast));
    }

    /// <summary>
    /// finds the next power of 2 for 
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private int NextPowerOf2(int number)
    {
        int pos = 0;

        while (number > 0)
        {
            pos++;
            number = number >> 1;
        }

        return (int) Mathf.Pow(2, pos);
    }
    
    public void UpdateComputeBuffers(Visualisation visualisation)
    {
        buffer.SetData(visualisation.theVisualizationObject.viewList[0].BigMesh.getBigMeshVertices());
        computeShader.SetBuffer(kernelHandleBrushTexture, "dataBuffer", buffer);

        filteredIndicesBuffer.SetData(visualisation.theVisualizationObject.viewList[0].GetFilterChannel());
        computeShader.SetBuffer(kernelHandleBrushTexture, "filteredIndices", filteredIndicesBuffer);
        computeShader.SetBuffer(kernelComputeNearestDistances, "filteredIndices", filteredIndicesBuffer);
    }


    public void ChartAdded(Chart chart)
    {
        StartCoroutine(SetChartViewProperties(chart));
    }

    /// <summary>
    /// Waits a frame before setting properties in the charts view. 
    /// </summary>
    /// <param name="chart"></param>
    /// <returns></returns>
    private IEnumerator SetChartViewProperties(Chart chart)
    {
        yield return null;

        while (chart.Visualisation.theVisualizationObject == null)
        {
            yield return null;
        }

        while (chart.Visualisation.theVisualizationObject.viewList.Count == 0)
        {
            yield return null;
        }

        foreach (var v in chart.Visualisation.theVisualizationObject.viewList)
        {
            v.BigMesh.SharedMaterial.SetTexture("_BrushedTexture", brushedIndicesTexture);
            v.BigMesh.SharedMaterial.SetInt("_IsBrushedTextureSet", 1);
            v.BigMesh.SharedMaterial.SetFloat("_DataWidth", texSize);
            v.BigMesh.SharedMaterial.SetFloat("_DataHeight", texSize);
        }
    }
    
    /// <summary>
    /// runs the compute shader kernel and updates the brushed indices
    /// </summary>
    /// Texture2D bla 
    Texture2D cachedTexture;

    public void updateBrushTexture()
    {
        // Set brushing mode
        computeShader.SetInt("BrushMode", (int) (BRUSH_TYPE));
        computeShader.SetInt("SelectionMode", (int) (SELECTION_TYPE));

        Vector3 projectedPointer1;
        Vector3 projectedPointer2;

        foreach (Visualisation brushingVisualisation in brushingVisualisations)
        {
            UpdateComputeBuffers(brushingVisualisation);

            switch (BRUSH_TYPE)
            {
                case BrushType.SPHERE:
                    projectedPointer1 = brushingVisualisation.transform.InverseTransformPoint(input1.transform.position);
                    
                    computeShader.SetFloats("pointer1", projectedPointer1.x, projectedPointer1.y, projectedPointer1.z);

                    break;
                case BrushType.BOX:
                    projectedPointer1 = brushingVisualisation.transform.InverseTransformPoint(input1.transform.position);
                    projectedPointer2 = brushingVisualisation.transform.InverseTransformPoint(input2.transform.position);
                    
                    computeShader.SetFloats("pointer1", projectedPointer1.x, projectedPointer1.y, projectedPointer1.z);
                    computeShader.SetFloats("pointer2", projectedPointer2.x, projectedPointer2.y, projectedPointer2.z);
                    break;
                default:
                    break;
            }

            //set the filters and normalisation values of the brushing visualisation to the computer shader
            computeShader.SetFloat("_MinNormX", brushingVisualisation.xDimension.minScale);
            computeShader.SetFloat("_MaxNormX", brushingVisualisation.xDimension.maxScale);
            computeShader.SetFloat("_MinNormY", brushingVisualisation.yDimension.minScale);
            computeShader.SetFloat("_MaxNormY", brushingVisualisation.yDimension.maxScale);
            computeShader.SetFloat("_MinNormZ", brushingVisualisation.zDimension.minScale);
            computeShader.SetFloat("_MaxNormZ", brushingVisualisation.zDimension.maxScale);

            computeShader.SetFloat("_MinX", brushingVisualisation.xDimension.minFilter);
            computeShader.SetFloat("_MaxX", brushingVisualisation.xDimension.maxFilter);
            computeShader.SetFloat("_MinY", brushingVisualisation.yDimension.minFilter);
            computeShader.SetFloat("_MaxY", brushingVisualisation.yDimension.maxFilter);
            computeShader.SetFloat("_MinZ", brushingVisualisation.zDimension.minFilter);
            computeShader.SetFloat("_MaxZ", brushingVisualisation.zDimension.maxFilter);

            computeShader.SetFloat("RadiusSphere", radiusSphere);

            computeShader.SetFloat("width", brushingVisualisation.width);
            computeShader.SetFloat("height", brushingVisualisation.height);
            computeShader.SetFloat("depth", brushingVisualisation.depth);

            // Set whether or not the brushing is currently being shared
            computeShader.SetBool("shareBrushing", shareBrushing);

            //run the compute shader with all the filtering parameters
            computeShader.Dispatch(kernelHandleBrushTexture, Mathf.CeilToInt(texSize / 32f), Mathf.CeilToInt(texSize / 32f), 1);
        }
    }

    public void updateNearestDistances()
    {
        if (nearestDistancesRequest.done)
        {
            if (!nearestDistancesRequest.hasError)
            {
                nearestDistances = nearestDistancesRequest.GetData<float>().ToList();
                NearestDistancesComputed.Invoke(nearestDistances);
            }
            
            UpdateComputeBuffers(visualisationToInspect);

            Vector3 inversePosition = visualisationToInspect.transform.InverseTransformPoint(input1.transform.position);
            computeShader.SetFloats("pointer1", inversePosition.x, inversePosition.y, inversePosition.z);

            //set the filters and normalisation values of the brushing visualisation to the computer shader
            computeShader.SetFloat("_MinNormX", visualisationToInspect.xDimension.minScale);
            computeShader.SetFloat("_MaxNormX", visualisationToInspect.xDimension.maxScale);
            computeShader.SetFloat("_MinNormY", visualisationToInspect.yDimension.minScale);
            computeShader.SetFloat("_MaxNormY", visualisationToInspect.yDimension.maxScale);
            computeShader.SetFloat("_MinNormZ", visualisationToInspect.zDimension.minScale);
            computeShader.SetFloat("_MaxNormZ", visualisationToInspect.zDimension.maxScale);

            computeShader.SetFloat("_MinX", visualisationToInspect.xDimension.minFilter);
            computeShader.SetFloat("_MaxX", visualisationToInspect.xDimension.maxFilter);
            computeShader.SetFloat("_MinY", visualisationToInspect.yDimension.minFilter);
            computeShader.SetFloat("_MaxY", visualisationToInspect.yDimension.maxFilter);
            computeShader.SetFloat("_MinZ", visualisationToInspect.zDimension.minFilter);
            computeShader.SetFloat("_MaxZ", visualisationToInspect.zDimension.maxFilter);

            computeShader.SetFloat("RadiusInspector", radiusInspector);

            computeShader.SetFloat("width", visualisationToInspect.width);
            computeShader.SetFloat("height", visualisationToInspect.height);
            computeShader.SetFloat("depth", visualisationToInspect.depth);

            computeShader.Dispatch(kernelComputeNearestDistances, Mathf.CeilToInt(nearestDistancesBuffer.count / 32f), 1, 1);
            nearestDistancesRequest = AsyncGPUReadback.Request(nearestDistancesBuffer);
        }
    }

    public void ResetBrushTexture()
    {
        computeShader.Dispatch(kernelResetBrushTexture, Mathf.CeilToInt(texSize / 32), Mathf.CeilToInt(texSize / 32), 1);
    }

    /// <summary>
    /// on destroy release the buffers on the graphic card
    /// </summary>
    void OnDestroy()
    {
        if (buffer != null)
            buffer.Release();

        if (brushedIndicesBuffer != null)
            brushedIndicesBuffer.Release();

        if (filteredIndicesBuffer != null)
            filteredIndicesBuffer.Release();

        if (nearestDistancesBuffer != null)
            nearestDistancesBuffer.Release();
    }

    private void OnApplicationQuit()
    {
        if (buffer != null)
            buffer.Release();

        if (brushedIndicesBuffer != null)
            brushedIndicesBuffer.Release();

        if (filteredIndicesBuffer != null)
            filteredIndicesBuffer.Release();

        if (nearestDistancesBuffer != null)
            nearestDistancesBuffer.Release();
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        { 
            stream.SendNext(brushEnabled);
            stream.SendNext(shareBrushing);
            stream.SendNext(radiusSphere);
            stream.SendNext(BRUSH_TYPE);
            stream.SendNext(SELECTION_TYPE);
        }
        else
        {
            bool isBrushing = (bool) stream.ReceiveNext();

            // Only turn on the brush if the client who owns it is also sharing the brush
            brushEnabled = (isBrushing && shareBrushing) ? true : false;

            shareBrushing = (bool) stream.ReceiveNext();
            radiusSphere = (float) stream.ReceiveNext();
            BRUSH_TYPE = (BrushType) stream.ReceiveNext();
            SELECTION_TYPE = (SelectionType) stream.ReceiveNext();
        }
    }
}