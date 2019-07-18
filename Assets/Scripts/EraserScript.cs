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

    GameObject rc;

    VRTK_ControllerEvents rcCE;

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        rc = GameObject.Find("RightController");
        rcCE = rc.GetComponent<VRTK_ControllerEvents>();

        originalOwner = photonView.Owner;

        initialiseEraserColor();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Line")
        {
            if (rcCE.gripClicked)
            {
                Destroy(other.gameObject);
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

    private void initialiseEraserColor()
    {
        // Set the color of shared brushing as the color of the eraser and share them to others via RPC only if this belongs to the player
        if (photonView.IsMine)
        {
            eraserColor = PlayerPreferencesManager.Instance.SharedBrushColor;
            this.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = eraserColor;
        }
    }

    #endregion


}