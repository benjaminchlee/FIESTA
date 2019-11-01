using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using VRTK;

public class PanelHandle : MonoBehaviourPunCallbacks {

    [SerializeField]
    private Panel parent;
    [SerializeField]
    private VRTK_InteractableObject interactableObject;

    private Vector3 localParentPosition;
    private bool isGrabbed = false;

    private void Start()
    {
        interactableObject.InteractableObjectGrabbed += OnHandleGrabbed;
        interactableObject.InteractableObjectUngrabbed += OnHandleUngrabbed;

        localParentPosition = transform.InverseTransformPoint(parent.transform.position);
    }

    private void OnHandleGrabbed(object sender, InteractableObjectEventArgs e)
    {
        if (!photonView.IsMine)
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

        if (!parent.photonView.IsMine)
            parent.photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

        isGrabbed = true;
        SetPositionAndRotation();

        DataLogger.Instance.LogActionData(parent, parent.OriginalOwner, parent.OriginalOwner, "Panel Grab start");
    }

    private void FixedUpdate()
    {
        if (isGrabbed)
        {
            if (photonView.IsMine)
            {
                SetPositionAndRotation();
            }
            else
            {
                interactableObject.ForceStopInteracting();
                transform.rotation = Quaternion.identity;
            }
        }
    }

    private void SetPositionAndRotation()
    {
        parent.transform.position = transform.TransformPoint(localParentPosition);
        parent.transform.rotation = transform.rotation;
    }

    private void OnHandleUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        isGrabbed = false;

        DataLogger.Instance.LogActionData(parent, parent.OriginalOwner, parent.OriginalOwner, "Panel Grab end");
    }
}
