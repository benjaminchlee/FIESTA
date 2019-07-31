using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class EraserScript : MonoBehaviourPunCallbacks
{
    #region Public Fields

    public Photon.Realtime.Player originalOwner { get; private set; }

    public Color eraserColor;

    #endregion

    #region Private Fields

    GameObject lc;

    VRTK_ControllerEvents lcCE;


    #endregion


    // Start is called before the first frame update
    void Start()
    {
        lc = GameObject.Find("LeftController");
        lcCE = lc.GetComponent<VRTK_ControllerEvents>();

        originalOwner = photonView.Owner;

        InitialiseEraserColor();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Line")
        {
            //Debug.Log("Collider detected");
            //Debug.Log("rcCE.gripClicked " + rcCE.gripClicked);
            //Debug.Log("other.gameObject.tag " + other.gameObject.tag);
            if (lcCE.gripClicked)
            {
                //PhotonNetwork.Destroy(other.gameObject);
                //Destroy(other.gameObject);

                //CallDestroyLine(other.gameObject);

                //if (photonView.IsMine)
                //{
                //    PhotonNetwork.Destroy(other.gameObject);
                //    Debug.Log("Destroyed");
                //}

                int viewID = other.gameObject.GetComponent<PhotonView>().ViewID;
                Debug.Log("line viewID " + viewID);
                CallDestroyLine(viewID);

            }
        }
    }

    #region Private Methods

    private bool IsOriginalOwner()
    {
        if (originalOwner == null)
            return (photonView.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

        return (originalOwner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
    }

    private void InitialiseEraserColor()
    {
        // Set the color of shared brushing as the color of the eraser and share them to others via RPC only if this belongs to the player
        if (photonView.IsMine)
        {
            eraserColor = PlayerPreferencesManager.Instance.SharedBrushColor;
            this.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = eraserColor;
        }
    }

    //public void CallDestroyLine(GameObject line)
    //{
    //    photonView.RPC("DestroyLine", RpcTarget.MasterClient, line);
    //    //Debug.Log("CallDestroyLine master client " + RpcTarget.MasterClient);
    //}

    //[PunRPC]
    //private void DestroyLine(GameObject obj)
    //{
    //    PhotonNetwork.Destroy(obj);
    //    //Destroy(obj);
    //    Debug.Log("destroyline");
    //}

    public void CallDestroyLine(int id)
    {
        photonView.RPC("DestroyLine", RpcTarget.All, id);
        //Debug.Log("CallDestroyLine master client " + RpcTarget.MasterClient);
    }

    [PunRPC]
    private void DestroyLine(int id)
    {
        Debug.Log(id + " " + PhotonView.Find(id).gameObject.name);
        //Destroy(PhotonView.Find(id).gameObject);
        PhotonNetwork.Destroy(PhotonView.Find(id).gameObject);
    }

    #endregion


}