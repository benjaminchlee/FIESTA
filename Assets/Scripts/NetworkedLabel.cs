using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkedLabel : Photon.MonoBehaviour
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
        text = value;
        textMesh.text = text;
    }

    public void SetRectTransform(Vector2 size)
    {
        rectTransformSize = size;
        RectTransform container = GetComponent<RectTransform>();
        container.sizeDelta = size;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(text);
            stream.SendNext(rectTransformSize);
        }
        else
        {
            SetText((string) stream.ReceiveNext());
            SetRectTransform((Vector2) stream.ReceiveNext());
        }
    }
}