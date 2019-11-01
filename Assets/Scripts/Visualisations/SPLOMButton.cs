using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class SPLOMButton : MonoBehaviourPunCallbacks
{

    [SerializeField] private TextMeshPro textMesh;
    private VRTK_InteractableObject interactableObject;
    private List<string> dimensions;
    public int parentSplomPhotonID;

    public string Text
    {
        get { return textMesh.text; }
        set { textMesh.text = value; }
    }
    
    private void Start()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        interactableObject = GetComponent<VRTK_InteractableObject>();
        interactableObject.InteractableObjectUsed += OnSPLOMButtonClicked;

        dimensions = GetAttributesList();
    }

    public void RangeClick()
    {
        int index = dimensions.IndexOf(Text);
        index = (index + 1) % dimensions.Count;

        textMesh.text = dimensions[index];

        PhotonView pv = PhotonView.Find(parentSplomPhotonID);
        pv.RPC("ScatterplotMatrixDimensionChanged", pv.Owner, photonView.ViewID, Text);

        DataLogger.Instance.LogActionData(this, GetComponentInParent<Chart>().OriginalOwner, GetComponentInParent<Chart>().photonView.Owner, transform.parent.name + " Splom Button Range Clicked", "", Text);
    }

    private void OnSPLOMButtonClicked(object sender, InteractableObjectEventArgs e)
    {
        int index = dimensions.IndexOf(Text);
        index = (index + 1) % dimensions.Count;

        textMesh.text = dimensions[index];

        PhotonView pv = PhotonView.Find(parentSplomPhotonID);
        pv.RPC("ScatterplotMatrixDimensionChanged", pv.Owner, photonView.ViewID, Text);
        
        DataLogger.Instance.LogActionData(this, GetComponentInParent<Chart>().OriginalOwner, GetComponentInParent<Chart>().photonView.Owner, transform.parent.name + " Splom Button Clicked", "", Text);
    }

    private List<string> GetAttributesList()
    {
        List<string> dimensions = new List<string>();
        for (int i = 0; i < ChartManager.Instance.DataSource.DimensionCount; ++i)
        {
            dimensions.Add(ChartManager.Instance.DataSource[i].Identifier);
        }

        return dimensions;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Text);
            stream.SendNext(parentSplomPhotonID);
        }
        else
        {
            Text = (string) stream.ReceiveNext();
            parentSplomPhotonID = (int) stream.ReceiveNext();
        }
    }
}
