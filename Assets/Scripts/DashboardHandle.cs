using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class DashboardHandle : Photon.MonoBehaviour {

    [SerializeField]
    private Dashboard parent;
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
        if (!photonView.isMine)
            photonView.TransferOwnership(PhotonNetwork.player);

        if (!parent.photonView.isMine)
            parent.photonView.TransferOwnership(PhotonNetwork.player);

        isGrabbed = true;
        SetPositionAndRotation();

        DataLogger.Instance.LogActionData(parent, parent.OriginalOwner, "Dashboard grab start");
    }

    private void Update()
    {
        if (isGrabbed)
        {
            if (photonView.isMine)
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

        DataLogger.Instance.LogActionData(parent, parent.OriginalOwner, "Dashboard grab end");
    }
}
