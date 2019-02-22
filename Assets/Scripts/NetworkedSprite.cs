using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedSprite : Photon.MonoBehaviour {

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public Sprite Sprite
    {
        get { return spriteRenderer.sprite; }
        set
        {
            if (value == null)
            {
                photonView.RPC("PropagateSprite", PhotonTargets.All, "");
            }
            else
            {

                photonView.RPC("PropagateSprite", PhotonTargets.All, value.name);
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
