using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class NetworkedLabel : MonoBehaviourPunCallbacks
{
    public TextMeshPro textMesh;
    private string text;
    private Vector2 rectTransformSize;

    private void Start()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();
    }

    public void SetText(string value)
    {
        photonView.RPC("PropagateSetText", RpcTarget.All, value);
    }

    [PunRPC]
    private void PropagateSetText(string value)
    {
        text = value;
        textMesh.text = text;
    }

    public void SetRectTransform(Vector2 size)
    {
        photonView.RPC("PropagateSetRectTransform", RpcTarget.All, size);
    }

    [PunRPC]
    private void PropagateSetRectTransform(Vector2 size)
    {
        rectTransformSize = size;
        RectTransform container = GetComponent<RectTransform>();
        container.sizeDelta = size;
    }
}