using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using VRTK;

public class PreSceneManager : MonoBehaviourPunCallbacks {
    
    private VRTK_InteractableObject interactableObject;
    private bool levelLoaded = false;

    private void Start()
    {
        if (interactableObject == null) interactableObject = GetComponent<VRTK_InteractableObject>();

        if (interactableObject != null)
            interactableObject.InteractableObjectGrabbed += LoadLevel;
    }

    private void Update () {
        if (Input.GetKeyDown(KeyCode.F12) && !levelLoaded)
        {
            LoadLevel();
        }
    }

    private void LoadLevel(object sender, InteractableObjectEventArgs e)
    {
        LoadLevel();
    }
    
    public void LoadLevel()
    {
        levelLoaded = true;
        photonView.RPC("ChangeScene", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void ChangeScene()
    {
        levelLoaded = true;
        PhotonNetwork.LoadLevel("MainScene");
    }
}
