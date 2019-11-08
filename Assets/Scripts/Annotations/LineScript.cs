using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VRTK;

public class LineScript : MonoBehaviourPunCallbacks
{
    public Photon.Realtime.Player OriginalOwner { get; private set; }

    public string ID;
    public bool isBeingDrawn = true;

    private LineRenderer lineRenderer;
    private StringBuilder stringBuilder;

    private void Start()
    {
        OriginalOwner = photonView.Owner;

        lineRenderer = GetComponent<LineRenderer>();
        stringBuilder = new StringBuilder();
    }

    public void SetLineID(string id)
    {
        photonView.RPC("SetLineIDRPC", RpcTarget.All, id);
    }

    [PunRPC]
    private void SetLineIDRPC(string id)
    {
        ID = id;
    }

    public string GetLinePositions()
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        stringBuilder.Clear();

        foreach (var pos in positions)
        {
            stringBuilder.Append(transform.TransformPoint(pos).ToString("F3"));
            stringBuilder.Append("|");
        }

        stringBuilder.Remove(stringBuilder.Length - 1, 1);

        return stringBuilder.ToString();
    }
}
