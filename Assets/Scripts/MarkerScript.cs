using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class MarkerScript : MonoBehaviourPunCallbacks
{
    VRTK_InteractableObject vrio;

    GameObject rc;

    VRTK_ControllerEvents rcCE;

    GameObject lc;

    VRTK_ControllerEvents lcCE;

    public GameObject linePrefab;

    public GameObject currentLine;

    public LineRenderer lineRenderer;

    public List<Vector3> fingerPositions;

    public MeshCollider meshCollider;

    public Photon.Realtime.Player originalOwner { get; private set; }

    //public Rigidbody rb;

    public Color markerColor;

    private bool rightGripPressedFlag = false;
    private bool lineStartFlag = false;
    private static int viewID;

    [Serializable]
    public class DrawingLineEvent : UnityEvent<List<float>> { }
    public DrawingLineEvent drawingLineEvent;

    // Start is called before the first frame update
    void Start()
    {
        vrio = GetComponent<VRTK_InteractableObject>();
        rc = GameObject.Find("RightController");
        rcCE = rc.GetComponent<VRTK_ControllerEvents>();
        lc = GameObject.Find("LeftController");
        lcCE = lc.GetComponent<VRTK_ControllerEvents>();

        InitialiseMarkerColor();
    }



    // Update is called once per frame
    void Update()
    {
        //Debug.Log("trigger: " + rcCE.triggerClicked + "; gripClicked: " + rcCE.gripClicked + "; gripPressed: " + rcCE.gripPressed);

        //if (rcCE.triggerClicked) // check if the trigger button is pressed down
        //{
        if (rcCE.gripClicked) // check if the grip button is pressed down
        //if (rcCE.triggerPressed)
        {
            //Debug.Log("IsOriginalOwner()" + IsOriginalOwner());
            if (!lineStartFlag && photonView.IsMine && IsOriginalOwner())
            {
                currentLine = PhotonNetwork.Instantiate("Line", Vector3.zero, Quaternion.identity);
                viewID = currentLine.GetComponent<PhotonView>().ViewID;
                Debug.Log("current viewid: " + viewID);
                CallCreateLine(viewID);
            }
                

            lineStartFlag = true;
            rightGripPressedFlag = true;
        }

        if (!rcCE.gripClicked)
        //if (!rcCE.triggerPressed)
        {
            if (rightGripPressedFlag)
            {
                lineStartFlag = false;
                CallUpdateMesh(viewID);
            }
            rightGripPressedFlag = false;
        }

        if (rightGripPressedFlag) //check whether or not the user is holding the grip button down
        {
            Vector3 tempFingerPos = gameObject.transform.position;
            //Debug.Log("tempFingerPos" + tempFingerPos);
            //Debug.Log("Vector3.Distance" + Vector3.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]));
            if (Vector3.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]) != 0f)
            {
                //UpdateLine(tempFingerPos);
                CallUpdateLine(tempFingerPos);
            }
        }

        //if (lcCE.gripClicked)
        //{
        //    DestroyGameObject();
        //}
        //}
    }

    public void CallCreateLine(int id)
    {
        photonView.RPC("CreateLine", RpcTarget.All, id);
    }

    [PunRPC]
    private void CreateLine(int id)
    {
        Debug.Log("000");
        // Create a new line and save it into a different variable
        //currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity); 
        //currentLine = PhotonNetwork.Instantiate("Line", Vector3.zero, Quaternion.identity);
        //Debug.Log("currentLine.GetComponent<PhotonView>().ViewID" + currentLine.GetComponent<PhotonView>().ViewID);
        if (photonView.IsMine)
        {
            lineRenderer = currentLine.GetComponent<LineRenderer>();
            Debug.Log("111");
        }
        else
        {
            Debug.Log("22OK");
            Debug.Log("22" + PhotonView.Find(id).gameObject.name);
            lineRenderer = PhotonView.Find(id).gameObject.GetComponent<LineRenderer>();
            Debug.Log("222");
        }
            
        //Debug.Log(lineRenderer.transform.GetSiblingIndex());

        //meshCollider = currentLine.AddComponent<MeshCollider>();
        //meshCollider.convex = true;
        //meshCollider.isTrigger = true;
        //rb = currentLine.AddComponent<Rigidbody>();
        //rb.useGravity = false;

        fingerPositions.Clear();

        // Set the first two points of the line renderer component
        fingerPositions.Add(gameObject.transform.position);
        fingerPositions.Add(gameObject.transform.position);

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
        //Debug.Log("fingerPositions length: " + fingerPositions.Count);
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
        //Debug.Log("updateMesh linerenderer" + lineRenderer == null);

        //bool isOriginalOwner = IsOriginalOwner();
        //Debug.Log("isOriginalOwner " + isOriginalOwner);
        //if (isOriginalOwner)

        lineRenderer = PhotonView.Find(id).gameObject.GetComponent<LineRenderer>();

        //if (photonView.IsMine)
        //{
        //    //lineRenderer = currentLine.GetComponent<LineRenderer>();
        //    //Debug.Log("Get local player's linerenderer");
        //    //return;
        //}
        //else
        //{
        //    //Debug.Log("44OK");
        //    //Debug.Log("id " + id);
        //    //Debug.Log("44" + PhotonView.Find(id).gameObject.name);
        //    lineRenderer = PhotonView.Find(id).gameObject.GetComponent<LineRenderer>();
        //    //Debug.Log("444");
        //}

        lineRenderer.BakeMesh(mesh, true);

        //if (photonView.IsMine)
        //{
        //    meshCollider = currentLine.GetComponent<MeshCollider>();
        //    Debug.Log("555");
        //}
        //else
        //{
        //    Debug.Log("55OK");
        //    Debug.Log("55" + PhotonView.Find(id).gameObject.name);
        //    meshCollider = PhotonView.Find(id).gameObject.GetComponent<MeshCollider>();
        //    Debug.Log("555");
        //}

        meshCollider = PhotonView.Find(id).gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    //void DestroyGameObject()
    //{
    //    Destroy(currentLine);
    //}

    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("collider.gameObject.tag: " + collider.gameObject.tag);
        if (collider.gameObject.tag == "Chart" && rightGripPressedFlag)
        {
            Transform newParent = collider.gameObject.transform;
            //Debug.Log("collider.gameObject " + collider.gameObject);
            currentLine.transform.SetParent(newParent);
            //Debug.Log("currentLine.transform.parent " + currentLine.transform.parent);
        }
    }

    //void OnCollisionEnter(Collision col)
    //{
    //    if (col.gameObject. tag == "Chart")
    //    {
    //        Transform newParent = col.gameObject.transform;
    //        currentLine.transform.SetParent(newParent);
    //        Debug.Log("currentLine.transform.parent " + currentLine.transform.parent);
    //    }
    //}

    private void InitialiseMarkerColor()
    {
        // Set the color of shared brushing as the color of the eraser and share them to others via RPC only if this belongs to the player
        if (photonView.IsMine)
        {
            markerColor = PlayerPreferencesManager.Instance.SharedBrushColor;

            GameObject markerLid = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
            markerLid.GetComponent<Renderer>().material.color = markerColor;

            GameObject markerTip = this.gameObject.transform.GetChild(0).GetChild(2).GetChild(2).gameObject;
            markerTip.GetComponent<Renderer>().material.color = markerColor;

            GameObject line = this.gameObject.GetComponent<MarkerScript>().linePrefab.gameObject;
            line.GetComponent<Renderer>().sharedMaterial.color = markerColor;
            //Debug.Log("line.GetComponent<Renderer>().sharedMaterial.color " + line.GetComponent<Renderer>().sharedMaterial.color);
        }
    }

    private bool IsOriginalOwner()
    {
        if (originalOwner == null)
            return (photonView.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

        return (originalOwner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
    }
}
