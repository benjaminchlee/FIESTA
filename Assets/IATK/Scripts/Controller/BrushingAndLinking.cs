using IATK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEditor;
using System;
using System.Linq;
using System.Security.Policy;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

public class BrushingAndLinking : Photon.PunBehaviour
{

    [SerializeField] public ComputeShader computeShader;
    ComputeBuffer buffer;

    ComputeBuffer brushedIndicesBuffer;

    ComputeBuffer filteredIndicesBuffer;

    [SerializeField] public Material myRenderMaterial;

    RenderTexture brushedIndicesTexture;

    int kernelHandleBrushTexture;
    int kernelHandleBrushArrayIndices;

    GameObject viewHolder;
    int texSize;

    [SerializeField] public List<Visualisation> brushingVisualisations;

    [SerializeField] public List<Visualisation> brushedVisualisations;

    [SerializeField] public List<LinkingVisualisations> brushedLinkingVisualisations;

    [SerializeField] public bool showBrush = false;

    [SerializeField] public bool shareBrushing = false;

    [SerializeField] public Color brushColor = Color.red;

    [SerializeField] [Range(1f, 10f)] public float brushSizeFactor = 1f;

    [SerializeField] public Transform input1;

    [SerializeField] public Transform input2;

    [SerializeField] [Range(0f, 1f)] public float radiusSphere;

    [SerializeField] public bool brushButtonController;

    public struct VecIndexPair
    {
        public Vector3 point;
        public int index;
    }

    public enum BrushType
    {
        SPHERE = 0,
        BOX = 1
    };

    public BrushType BRUSH_TYPE;

    public enum SelectionType
    {
        FREE = 0,
        ADDITIVE,
        SUBTRACTIVE
    };

    public SelectionType SELECTION_TYPE;

    int computeTextureSize(int sizeDatast)
    {
        return NextPowerOf2((int) Mathf.Sqrt((float) sizeDatast));
    }

    public Material debugObjectTexture;

    private AsyncGPUReadbackRequest detailsOnDemandRequest;
    public List<int> brushedIndices;

    // private fields
    private bool activated = false;

    // Use this for initialization
    void Start()
    {
        brushingVisualisations = new List<Visualisation>();

        //InitialiseShaders();
    }

    // Update is called once per frame
    void Update()
    {
        if (brushButtonController)
        {
            switch (BRUSH_TYPE)
            {
                case BrushType.SPHERE:
                    Collider[] colliders = Physics.OverlapSphere(input1.position, radiusSphere);
                    brushingVisualisations.Clear();
                    foreach (Collider col in colliders)
                    {
                        if (col.gameObject.CompareTag("Chart"))
                        {
                            brushingVisualisations.Add(col.gameObject.GetComponent<Chart>().Visualisation);
                        }
                    }

                    break;

                case BrushType.BOX:
                    break;
            }
        }

        if (brushingVisualisations.Count != 0 && !activated)
        {
            InitialiseShaders();
            InitialiseBuffersAndTextures(brushingVisualisations[0].theVisualizationObject.viewList[0].BigMesh
                .getBigMeshVertices());
            activated = true;
        }

        if (brushingVisualisations.Count != 0 && (brushButtonController) && input1 != null && input2 != null
        ) // && brushedVisualisations.Count>0)
        {
            updateBrushTexture();

            //EXPERIMENTAL - GET details of original data
            getDetailsOnDemand();
        }
    }

    public void InitialiseShaders()
    {
        kernelHandleBrushTexture = computeShader.FindKernel("CSMain");
        kernelHandleBrushArrayIndices = computeShader.FindKernel("ComputeBrushedIndicesArray");

        //computeShader.Dispatch(kernelHandleBrushTexture, 32, 32, 1);

        setTexture(brushedIndicesTexture);
    }

    public void InitialiseBuffersAndTextures(Vector3[] data)
    {
        int datasetSize = data.Length;

        buffer = new ComputeBuffer(datasetSize, 12);
        buffer.SetData(data);
        computeShader.SetBuffer(kernelHandleBrushTexture, "dataBuffer", buffer);

        brushedIndicesBuffer = new ComputeBuffer(data.Length, 4);
        int[] brushIni = new int[data.Length];
        for (int i = 0; i < data.Length; i++)
            brushIni[i] = -1;
        brushedIndicesBuffer.SetData(brushIni);

        filteredIndicesBuffer = new ComputeBuffer(data.Length, 4);
        filteredIndicesBuffer.SetData(new float[data.Length]);

        computeShader.SetBuffer(kernelHandleBrushArrayIndices, "dataBuffer", buffer);
        computeShader.SetBuffer(kernelHandleBrushArrayIndices, "brushedIndices", brushedIndicesBuffer);

        texSize = computeTextureSize(datasetSize);
        Debug.Log(texSize);
        brushedIndicesTexture = new RenderTexture(texSize, texSize, 24);
        brushedIndicesTexture.enableRandomWrite = true;
        brushedIndicesTexture.filterMode = FilterMode.Point;
        brushedIndicesTexture.Create();

        computeShader.SetTexture(kernelHandleBrushTexture, "Result", brushedIndicesTexture);
        computeShader.SetTexture(kernelHandleBrushArrayIndices, "Result", brushedIndicesTexture);

        setSize((float) texSize);
    }

    /// <summary>
    /// sets the index texture
    /// </summary>
    /// <param name="_tex"></param>
    public void setTexture(Texture _tex)
    {
        myRenderMaterial.SetTexture("_MainTex", _tex);
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

    /// <summary>
    /// sets the size of the texture in the compute shader program. this is needed to adress the right uv coordinates
    /// to store the brushed information corretcly
    /// </summary>
    /// <param name="TexSize"></param>
    public void setSize(float TexSize)
    {
        computeShader.SetFloat("_size", TexSize);
    }

    float time = 0f;

    /// <summary>
    /// reads the brushed indices
    /// </summary>
    public void readBrushTexture()
    {
        //Texture2D tex = (brushingVisualisation.theVisualizationObject.viewList[0].BigMesh.SharedMaterial.GetTexture("_BrushedTexture") as Texture2D);
    }

    public void UpdateComputeBuffers(Visualisation visualisation)
    {
        buffer.SetData(visualisation.theVisualizationObject.viewList[0].BigMesh.getBigMeshVertices());
        computeShader.SetBuffer(kernelHandleBrushTexture, "dataBuffer", buffer);

        filteredIndicesBuffer.SetData(visualisation.theVisualizationObject.viewList[0].GetFilterChannel());
        computeShader.SetBuffer(kernelHandleBrushTexture, "filteredIndices", filteredIndicesBuffer);
    }

    /// <summary>
    /// runs the compute shader kernel and updates the brushed indices
    /// </summary>
    /// Texture2D bla 
    Texture2D cachedTexture;

    public void updateBrushTexture()
    {
        //bla.ReadPixels(new Rect(),0,0).
        //set brushgin mode
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
                    projectedPointer1 =
                        brushingVisualisation.transform.InverseTransformPoint(input1.transform.position);
                    //  Vector3 
                    computeShader.SetFloat("pointer1x", projectedPointer1.x);
                    computeShader.SetFloat("pointer1y", projectedPointer1.y);
                    computeShader.SetFloat("pointer1z", projectedPointer1.z);

                    break;
                case BrushType.BOX:
                    projectedPointer1 =
                        brushingVisualisation.transform.InverseTransformPoint(input1.transform.position);
                    projectedPointer2 =
                        brushingVisualisation.transform.InverseTransformPoint(input2.transform.position);

                    //  Vector3 
                    computeShader.SetFloat("pointer1x", projectedPointer1.x);
                    computeShader.SetFloat("pointer1y", projectedPointer1.y);
                    computeShader.SetFloat("pointer1z", projectedPointer1.z);

                    computeShader.SetFloat("pointer2x", projectedPointer2.x);
                    computeShader.SetFloat("pointer2y", projectedPointer2.y);
                    computeShader.SetFloat("pointer2z", projectedPointer2.z);
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
            computeShader.Dispatch(kernelHandleBrushTexture, Mathf.Max(texSize / 32, 1), Mathf.Max(texSize / 32, 1), 1);

            brushingVisualisation.theVisualizationObject.viewList[0].BigMesh.SharedMaterial
                .SetTexture("_BrushedTexture", brushedIndicesTexture);
            brushingVisualisation.theVisualizationObject.viewList[0].BigMesh.SharedMaterial
                .SetFloat("_DataWidth", texSize);
            brushingVisualisation.theVisualizationObject.viewList[0].BigMesh.SharedMaterial
                .SetFloat("_DataHeight", texSize);
            brushingVisualisation.theVisualizationObject.viewList[0].BigMesh.SharedMaterial
                .SetFloat("showBrush", Convert.ToSingle(showBrush));
            brushingVisualisation.theVisualizationObject.viewList[0].BigMesh.SharedMaterial
                .SetColor("brushColor", brushColor);
        }

        //foreach (var bv in brushedVisualisations)// visualisationsMaterials)
        foreach (var bv in ChartManager.Instance.Visualisations) // visualisationsMaterials)
        {
            foreach (var v in bv.theVisualizationObject.viewList)
            {
                v.BigMesh.SharedMaterial.SetTexture("_BrushedTexture", brushedIndicesTexture);
                v.BigMesh.SharedMaterial.SetFloat("_DataWidth", texSize);
                v.BigMesh.SharedMaterial.SetFloat("_DataHeight", texSize);
                v.BigMesh.SharedMaterial.SetFloat("showBrush", Convert.ToSingle(showBrush));
                v.BigMesh.SharedMaterial.SetColor("brushColor", brushColor);
            }
        }

        foreach (var item in brushedLinkingVisualisations)
        {
            item.View.BigMesh.SharedMaterial.SetTexture("_BrushedTexture", brushedIndicesTexture);
            item.View.BigMesh.SharedMaterial.SetFloat("_DataWidth", texSize);
            item.View.BigMesh.SharedMaterial.SetFloat("_DataHeight", texSize);
            item.View.BigMesh.SharedMaterial.SetFloat("showBrush", Convert.ToSingle(showBrush));
            item.View.BigMesh.SharedMaterial.SetColor("brushColor", brushColor);
        }

        //cachedTexture = (brushingVisualisation.theVisualizationObject.viewList[0].BigMesh.SharedMaterial.GetTexture("_BrushedTexture") as Texture2D);
        //if (cachedTexture.GetPixel(0, 0).r > 0f) print("selected!!");
        //float t = Time.time;
        //brushedIndicesBuffer.GetData(brushIni);
        //  if (brushIni[0] > 0f) print("Selected");
        //getDetailsOnDemand();
        //debugObjectTexture.SetTexture("_MainTex", brushedIndicesTexture);

    }

    public void getDetailsOnDemand()
    {
        if (detailsOnDemandRequest.done)
        {
            if (!detailsOnDemandRequest.hasError)
            {
                // Get data
                int[] result = detailsOnDemandRequest.GetData<int>().ToArray();
                //Debug.Log(result.Length);
                //brushedIndices = Enumerable.Range(0, result.Length)
                //    .Where(x => result[x] == 1)
                //    .ToList();
                brushedIndices = result.ToList();
            }

            // Dispatch again
            computeShader.Dispatch(kernelHandleBrushArrayIndices, Mathf.CeilToInt(brushedIndicesBuffer.count / 16f), 1,
                1);
            detailsOnDemandRequest = AsyncGPUReadback.Request(brushedIndicesBuffer);
        }
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

        //Visualisation.OnUpdateViewAction -= Visualisation_OnUpdateViewAction;
    }

    private void OnApplicationQuit()
    {
        if (buffer != null)
            buffer.Release();

        if (brushedIndicesBuffer != null)
            brushedIndicesBuffer.Release();

        //Visualisation.OnUpdateViewAction -= Visualisation_OnUpdateViewAction;
    }


    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        { 
            stream.SendNext(brushButtonController);
            stream.SendNext(shareBrushing);
            stream.SendNext(radiusSphere);
            stream.SendNext(BRUSH_TYPE);
            stream.SendNext(SELECTION_TYPE);
        }
        else
        {
            bool isBrushing = (bool) stream.ReceiveNext();
            shareBrushing = (bool) stream.ReceiveNext();
            radiusSphere = (float) stream.ReceiveNext();
            BRUSH_TYPE = (BrushType) stream.ReceiveNext();
            SELECTION_TYPE = (SelectionType) stream.ReceiveNext();

            // Only turn on the brush if the client who owns it is also sharing the brush
            if (isBrushing && shareBrushing)
                brushButtonController = true;
            else
                brushButtonController = false;
        }
    }
}