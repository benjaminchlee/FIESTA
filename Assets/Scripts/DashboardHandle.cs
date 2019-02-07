using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class DashboardHandle : MonoBehaviour {

    private Transform parent;
    private VRTK_InteractableObject interactableObject;

    private void Start()
    {
        parent = transform.parent;

        interactableObject = GetComponent<VRTK_InteractableObject>();

        interactableObject.InteractableObjectGrabbed += OnHandleGrabbed;
        interactableObject.InteractableObjectUngrabbed += OnHandleUngrabbed;
    }

    private void OnHandleGrabbed(object sender, InteractableObjectEventArgs e)
    {
        parent.gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player);

        transform.parent = null;
        parent.SetParent(transform);
    }

    private void OnHandleUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        parent.transform.parent = null;
        transform.SetParent(parent);
    }
}
