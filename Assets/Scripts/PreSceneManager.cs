using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreSceneManager : Photon.MonoBehaviour {
    
    private bool levelLoaded = false;

	private void Update () {
        if (Input.GetKeyDown(KeyCode.F12) && !levelLoaded)
        {
            levelLoaded = true;
            photonView.RPC("ChangeScene", PhotonTargets.AllViaServer);
        }
    }

    [PunRPC]
    private void ChangeScene()
    {
        levelLoaded = true;
        PhotonNetwork.LoadLevel("MainScene");
    }
}
