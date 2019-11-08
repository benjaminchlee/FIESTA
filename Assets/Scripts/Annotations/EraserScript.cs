using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class EraserScript : MonoBehaviourPunCallbacks
{
    public Color eraserColor;

    public Photon.Realtime.Player OriginalOwner { get; private set; }

    private VRTK_InteractableObject interactableObject;
    private bool isErasing;
    

    private void Start()
    {
        OriginalOwner = photonView.Owner;

        interactableObject = GetComponent<VRTK_InteractableObject>();
        interactableObject.InteractableObjectUsed += OnEraserEnabled;
        interactableObject.InteractableObjectUnused += OnEraserDisabled;

        OriginalOwner = photonView.Owner;

        if (photonView.IsMine)
        {
            InitialiseEraserColor(PlayerPreferencesManager.Instance.SharedBrushColor);
        }
    }

    private void OnDestroy()
    {
        interactableObject.InteractableObjectUnused -= OnEraserEnabled;
        interactableObject.InteractableObjectUnused -= OnEraserDisabled;
    }

    private void OnEraserEnabled(object sender, InteractableObjectEventArgs e)
    {
        isErasing = true;

        DataLogger.Instance.LogActionData(this, OriginalOwner, photonView.Owner, "Eraser start");
    }

    private void OnEraserDisabled(object sender, InteractableObjectEventArgs e)
    {
        isErasing = false;

        DataLogger.Instance.LogActionData(this, OriginalOwner, photonView.Owner, "Eraser end");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isErasing)
        {
            if (other.gameObject.tag == "Line")
            {
                LineScript line = other.GetComponent<LineScript>();
                DataLogger.Instance.LogActionData(this, line.OriginalOwner, line.photonView.Owner, "Line erased", line.ID);

                PhotonNetwork.Destroy(other.gameObject);

                VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(interactableObject.GetGrabbingObject()), 0.3f);
                
            }
        }
    }
    
    [PunRPC]
    private void InitialiseEraserColor(Color color)
    {
        // Set the color of shared brushing as the color of the eraser and share them to others via RPC only if this belongs to the player
        if (photonView.IsMine)
        {
            photonView.RPC("InitialiseEraserColor", RpcTarget.OthersBuffered, color);
        }
        
        eraserColor = color;
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = eraserColor;
    }
    
    public void CallDestroyLine(int id)
    {
        photonView.RPC("DestroyLine", RpcTarget.All, id);
    }

    [PunRPC]
    private void DestroyLine(int id)
    {
        PhotonNetwork.Destroy(PhotonView.Find(id).gameObject);
    }
}