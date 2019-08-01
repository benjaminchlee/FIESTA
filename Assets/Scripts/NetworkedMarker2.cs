using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class NetworkedMarker2 : MonoBehaviourPunCallbacks
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

    //public Rigidbody rb;

    public Color markerColor;

    private bool rightGripPressedFlag = false;
    private bool lineStartFlag = false;
    private int lineID;
    private int chartID;

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
            if (!lineStartFlag)
            {
                Debug.Log("11111");
                InstantiateLine();
                Debug.Log("11111DONE");

                Debug.Log("22222");
                if (photonView.IsMine)
                    CallCreateLine(currentLine);
                Debug.Log("22222DONE");

                Debug.Log("33333");
                lineStartFlag = true;
                rightGripPressedFlag = true;
                Debug.Log("33333DONE");
            }
        }

        if (!rcCE.gripClicked)
        //if (!rcCE.triggerPressed)
        {
            if (rightGripPressedFlag)
            {
                Debug.Log("44444");
                lineStartFlag = false;
                //Mesh mesh = new Mesh();
                //lineRenderer.BakeMesh(mesh, true);
                //meshCollider.sharedMesh = mesh;
                Debug.Log("44444DONE");
                Debug.Log("55555");
                CallUpdateMesh(currentLine);
                Debug.Log("55555DONE");
            }
            rightGripPressedFlag = false;
        }

        if (rightGripPressedFlag) //check whether or not the user is holding the grip button down
        {
            Vector3 tempFingerPos = transform.position;
            //Debug.Log("tempFingerPos" + tempFingerPos);
            //Debug.Log("Vector3.Distance" + Vector3.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]));
            if (Vector3.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]) != 0f)
            {
                Debug.Log("66666");
                //UpdateLine(tempFingerPos);
                CallUpdateLine(tempFingerPos);
                Debug.Log("66666DONE");
            }
        }
    }

    public void InstantiateLine()
    {
        currentLine = PhotonNetwork.Instantiate("Line", Vector3.zero, Quaternion.identity);
        lineID = currentLine.GetComponent<PhotonView>().ViewID;
    }

    public void CallFindLine()
    {
        photonView.RPC("FindLine", RpcTarget.Others);
    }

    public GameObject FindLine()
    {
        GameObject line = PhotonView.Find(lineID).gameObject;
        return line;
    }

    public void CallCreateLine(GameObject line)
    {
        photonView.RPC("CreateLine", RpcTarget.All, line);
    }

    [PunRPC]
    private void CreateLine(GameObject line)
    {
        lineRenderer = line.GetComponent<LineRenderer>();

        fingerPositions.Clear();

        // Set the first two points of the line renderer component
        fingerPositions.Add(gameObject.transform.position);
        fingerPositions.Add(gameObject.transform.position);
        lineRenderer.SetPosition(0, fingerPositions[0]);
        lineRenderer.SetPosition(1, fingerPositions[1]);

    }

    public void CallUpdateMesh(GameObject line)
    {
        photonView.RPC("UpdateMesh", RpcTarget.All, line);
    }

    [PunRPC]
    private void UpdateMesh(GameObject line)
    {
        Mesh mesh = new Mesh();

        lineRenderer = line.GetComponent<LineRenderer>();
        meshCollider = line.GetComponent<MeshCollider>();

        lineRenderer.BakeMesh(mesh, true);
        meshCollider.sharedMesh = mesh;
    }

    public void CallUpdateLine(Vector3 pos)
    {
        //Debug.Log("!!!");
        photonView.RPC("UpdateLine", RpcTarget.All, pos);
    }

    [PunRPC]
    private void UpdateLine(Vector3 newFingerPos)
    {
        //Debug.Log(newFingerPos);
        fingerPositions.Add(newFingerPos);
        Debug.Log("fingerPositions length: " + fingerPositions.Count);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newFingerPos);

    }


    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("collider.gameObject.tag: " + collider.gameObject.tag);
        if (collider.gameObject.tag == "Chart")
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

            GameObject line = this.gameObject.GetComponent<NetworkedMarker2>().linePrefab.gameObject;
            line.GetComponent<Renderer>().sharedMaterial.color = markerColor;
            //Debug.Log("line.GetComponent<Renderer>().sharedMaterial.color " + line.GetComponent<Renderer>().sharedMaterial.color);
        }
    }
}

