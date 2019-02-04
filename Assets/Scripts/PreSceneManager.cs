using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreSceneManager : MonoBehaviour {
    
	private void Update () {
        if (Input.GetKeyDown("a"))
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.LoadLevel("MainScene");
                //SteamVR_LoadLevel.Begin("MainScene");
            }
        }
    }
}
