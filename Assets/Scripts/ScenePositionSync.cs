using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ScenePositionSync : MonoBehaviour {

    private GameObject sdkManagerGameObject;
    private VRTK_ControllerEvents leftControllerEvents;
    private VRTK_ControllerEvents rightControllerEvents;

    private bool isLeftControllerDown = false;
    private bool isRightControllerDown = false;

    private void Start()
    {
        sdkManagerGameObject = FindObjectOfType<VRTK_SDKManager>().gameObject;
    }

    private void Update()
    {
        // If controllers havent been found yet
        if (leftControllerEvents == null || rightControllerEvents == null)
        {
            // If the controllers finally exist
            if (VRTK_DeviceFinder.GetControllerLeftHand() != null && VRTK_DeviceFinder.GetControllerRightHand() != null)
            {
                leftControllerEvents = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_ControllerEvents>();
                rightControllerEvents = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_ControllerEvents>();

                RegisterEvents();
            }
        }
    }

    private void OnDestroy()
    {
        DeregisterEvents();
    }

    private void RegisterEvents()
    {
        leftControllerEvents.GripClicked += OnLeftControllerGripClicked;
        leftControllerEvents.GripUnclicked += OnLeftControllerGripUnclicked;
    
        rightControllerEvents.GripClicked += OnRightControllerGripClicked;
        rightControllerEvents.GripUnclicked += OnRightControllerGripUnclicked;
    }

    private void DeregisterEvents()
    {
        if (leftControllerEvents != null)
        {
            leftControllerEvents.GripClicked -= OnLeftControllerGripClicked;
            leftControllerEvents.GripUnclicked -= OnLeftControllerGripUnclicked;
        }

        if (rightControllerEvents != null)
        {
            rightControllerEvents.GripClicked -= OnRightControllerGripClicked;
            rightControllerEvents.GripUnclicked -= OnRightControllerGripUnclicked;
        }
    }

    private void OnLeftControllerGripClicked(object sender, ControllerInteractionEventArgs e)
    {
        isLeftControllerDown = true;

        if (isLeftControllerDown && isRightControllerDown)
            SyncPosition();
    }

    private void OnLeftControllerGripUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        isLeftControllerDown = false;
    }

    private void OnRightControllerGripClicked(object sender, ControllerInteractionEventArgs e)
    {
        isRightControllerDown = true;

        if (isLeftControllerDown && isRightControllerDown)
            SyncPosition();
    }

    private void OnRightControllerGripUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        isRightControllerDown = false;
    }

    private void SyncPosition()
    {
        Transform headset = VRTK_DeviceFinder.HeadsetTransform();

        Vector3 newRotation = Vector3.up * headset.transform.eulerAngles.y;

        sdkManagerGameObject.transform.eulerAngles = newRotation;

        // Vector between headset position and origin is just the headset position

        // Translate the SDK manager object by this vector
        sdkManagerGameObject.transform.position = sdkManagerGameObject.transform.position - VRTK_DeviceFinder.HeadsetTransform().position;
    }
}
