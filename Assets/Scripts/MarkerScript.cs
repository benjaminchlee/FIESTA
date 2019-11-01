using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class MarkerScript : MonoBehaviourPunCallbacks
{
    public GameObject linePrefab;
    public GameObject currentLine;
    public string currentLineID;
    public Transform markerTip;
    public LineRenderer lineRenderer;
    public List<Vector3> fingerPositions = new List<Vector3>();
    public MeshCollider meshCollider;
    public Color markerColor;

    public Photon.Realtime.Player OriginalOwner { get; private set; }

    private VRTK_InteractableObject interactableObject;
    private int viewID;
    private bool isDrawing = false;
    private bool isTouchingChart;
    private GameObject touchingChart;

    [Serializable]
    public class DrawingLineEvent : UnityEvent<List<float>> { }
    public DrawingLineEvent drawingLineEvent;

    private void Start()
    {
        OriginalOwner = photonView.Owner;

        interactableObject = GetComponent<VRTK_InteractableObject>();
        interactableObject.InteractableObjectUsed += OnMarkerEnabled;
        interactableObject.InteractableObjectUnused += OnMarkerDisabled;

        if (photonView.IsMine)
        {
            InitialiseMarkerColor(PlayerPreferencesManager.Instance.SharedBrushColor);
        }
    }

    private void OnDestroy()
    {
        interactableObject.InteractableObjectUsed -= OnMarkerEnabled;
        interactableObject.InteractableObjectUnused -= OnMarkerDisabled;
    }

    private void OnMarkerEnabled(object sender, InteractableObjectEventArgs e)
    {
        if (!isDrawing && photonView.IsMine)
        {
            currentLine = PhotonNetwork.Instantiate("Line", Vector3.zero, Quaternion.identity);
            viewID = currentLine.GetComponent<PhotonView>().ViewID;
            CallCreateLine(viewID);

            isDrawing = true;

            currentLineID = Guid.NewGuid().ToString();
            currentLine.GetComponent<LineScript>().SetLineID(currentLineID);

            DataLogger.Instance.LogActionData(this, OriginalOwner, photonView.Owner, "Marker Draw start", currentLineID);
        }
    }

    private void OnMarkerDisabled(object sender, InteractableObjectEventArgs e)
    {
        if (isDrawing)
        {
            isDrawing = false;

            CallUpdateMesh(viewID);

            DataLogger.Instance.LogActionData(this, OriginalOwner, photonView.Owner, "Marker Draw end", currentLineID);
        }
    }
    
    private void Update()
    {
        if (isDrawing && currentLine != null)
        {
            Vector3 tempFingerPos = markerTip.position;

            if (Vector3.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]) >= 0.005f)
            {
                CallUpdateLine(tempFingerPos);
            }
            
            if (isTouchingChart && currentLine.transform.parent == null)
            {
                currentLine.transform.SetParent(touchingChart.transform);
            }

            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(interactableObject.GetGrabbingObject()), 0.025f);
        }
    }

    public void CallCreateLine(int id)
    {
        photonView.RPC("CreateLine", RpcTarget.All, id);
    }

    [PunRPC]
    private void CreateLine(int id)
    {
        if (photonView.IsMine)
        {
            lineRenderer = currentLine.GetComponent<LineRenderer>();
        }
        else
        {
            currentLine = PhotonView.Find(id).gameObject;
            lineRenderer = currentLine.GetComponent<LineRenderer>();
        }

        viewID = id;

        currentLine.GetComponent<Renderer>().material.color = markerColor;

        fingerPositions.Clear();

        // Set the first two points of the line renderer component
        fingerPositions.Add(markerTip.position);
        fingerPositions.Add(markerTip.position);

        //Debug.Log("createline linerenderer" + lineRenderer == null);
        lineRenderer.SetPosition(0, fingerPositions[0]);
        lineRenderer.SetPosition(1, fingerPositions[1]);

    }

    public void CallUpdateLine(Vector3 pos)
    {
        photonView.RPC("UpdateLine", RpcTarget.All, pos);
    }

    [PunRPC]
    private void UpdateLine(Vector3 newFingerPos)
    {
        fingerPositions.Add(newFingerPos);

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newFingerPos);
    }

    public void CallUpdateMesh(int id)
    {
        photonView.RPC("UpdateMesh", RpcTarget.All, id);
    }

    [PunRPC]
    private void UpdateMesh(int id)
    {
        Mesh mesh = new Mesh();

        lineRenderer = PhotonView.Find(id).gameObject.GetComponent<LineRenderer>();
        
        lineRenderer.BakeMesh(mesh, Camera.main, true);
        
        meshCollider = PhotonView.Find(id).gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Chart")
        {
            isTouchingChart = true;
            touchingChart = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Chart" && other == touchingChart)
        {
            isTouchingChart = false;
            touchingChart = null;
        }
    }

    [PunRPC]
    private void InitialiseMarkerColor(Color color)
    {
        // Set the color of shared brushing as the color of the eraser and share them to others via RPC only if this belongs to the player
        if (photonView.IsMine)
        {
            photonView.RPC("InitialiseMarkerColor", RpcTarget.OthersBuffered, color);
        }

        markerColor = color;

        GameObject markerLid = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        markerLid.GetComponent<Renderer>().material.color = markerColor;

        GameObject markerTip = this.gameObject.transform.GetChild(0).GetChild(2).GetChild(2).gameObject;
        markerTip.GetComponent<Renderer>().material.color = markerColor;
    }
}
