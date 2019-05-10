using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkedTrackedObject : MonoBehaviourPunCallbacks {

    [SerializeField]
    private Renderer renderer;

    private GameObject gameObjectToTrack = null;
    private bool isTracking = true;


    public void SetTrackedObject(GameObject go)
    {
        if (photonView.IsMine)
        {
            gameObjectToTrack = go;
        }
    }

    public void SetColor(Color col)
    {
        photonView.RPC("PropagateSetColor", RpcTarget.AllBuffered, col);
    }

    [PunRPC]
    public void PropagateSetColor(Color col)
    {
        renderer.material.SetColor("_Color", col);
    }
    
    private void Update()
    {
        if (isTracking && gameObjectToTrack != null)
        {
            if (gameObjectToTrack.activeInHierarchy)
            {
                transform.position = gameObjectToTrack.transform.position;
                transform.rotation = gameObjectToTrack.transform.rotation;
                transform.localScale = gameObjectToTrack.transform.localScale;
            }
            else
            {
                transform.localScale = Vector3.zero;
            }
        }
    }
}
