using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    //public Rigidbody rb;

    public Color markerColor;

    private bool rightGripPressedFlag = false;
    private bool lineStartFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        vrio = GetComponent<VRTK_InteractableObject>();
        rc = GameObject.Find("RightController");
        rcCE = rc.GetComponent<VRTK_ControllerEvents>();
        lc = GameObject.Find("LeftController");
        lcCE = lc.GetComponent<VRTK_ControllerEvents>();

        initialiseMarkerColor();
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
                CreateLine();
            lineStartFlag = true;
            rightGripPressedFlag = true;
        }
        if (!rcCE.gripClicked)
        //if (!rcCE.triggerPressed)
        {
            if (rightGripPressedFlag)
            {
                lineStartFlag = false;
                Mesh mesh = new Mesh();
                lineRenderer.BakeMesh(mesh, true);
                meshCollider.sharedMesh = mesh;
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
                UpdateLine(tempFingerPos);
            }
        }

        //if (lcCE.gripClicked)
        //{
        //    DestroyGameObject();
        //}
        //}
    }

    private void CreateLine()
    {
        // Create a new line and save it into a different variable
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity); //photonnetwork.instantiate
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        meshCollider = currentLine.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        //rb = currentLine.AddComponent<Rigidbody>();
        //rb.useGravity = false;
        fingerPositions.Clear();

        // Set the first two points of the line renderer component
        fingerPositions.Add(gameObject.transform.position);
        fingerPositions.Add(gameObject.transform.position);
        lineRenderer.SetPosition(0, fingerPositions[0]);
        lineRenderer.SetPosition(1, fingerPositions[1]);

    }

    private void UpdateLine(Vector3 newFingerPos)
    {
        fingerPositions.Add(newFingerPos);
        //Debug.Log("fingerPositions length: " + fingerPositions.Count);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newFingerPos);

    }

    //void DestroyGameObject()
    //{
    //    Destroy(currentLine);
    //}

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

    private void initialiseMarkerColor()
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
            Debug.Log("line.GetComponent<Renderer>().sharedMaterial.color " + line.GetComponent<Renderer>().sharedMaterial.color);
        }
    }
}
