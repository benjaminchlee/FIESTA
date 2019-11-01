using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PointerCustomisation : MonoBehaviourPunCallbacks
{
    public Transform stick;
    public GameObject stickSphere;

    public void Enable3DStick()
    {
        photonView.RPC("Enable3DStickRPC", RpcTarget.All);
    }

    [PunRPC]
    private void Enable3DStickRPC()
    {
        stick.gameObject.SetActive(true);
    }

    public void Disable3DStick()
    {
        photonView.RPC("Disable3DStickRPC", RpcTarget.All);
    }

    [PunRPC]
    private void Disable3DStickRPC()
    {
        stick.gameObject.SetActive(false);
    }
}
