using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkedSprite : MonoBehaviourPunCallbacks {

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public Sprite Sprite
    {
        get { return spriteRenderer.sprite; }
        set
        {
            if (value == null)
            {
                photonView.RPC("PropagateSprite", RpcTarget.All, "");
            }
            else
            {

                photonView.RPC("PropagateSprite", RpcTarget.All, value.name);
            }
        }
    }

    [PunRPC]
    private void PropagateSprite(string name)
    {
        if (name == "")
        {
            spriteRenderer.sprite = null;
        }
        else
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/" + name);
        }
    }
}
