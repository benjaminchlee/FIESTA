using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Oculus.Avatar;
using VRTK;

public class CustomAvatarLocalDriver : OvrAvatarDriver
{

    ControllerPose GetMalibuControllerPose(OVRInput.Controller controller)
    {
        ovrAvatarButton buttons = 0;
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller)) buttons |= ovrAvatarButton.One;

        return new ControllerPose
        {
            buttons = buttons,
            touches = OVRInput.Get(OVRInput.Touch.PrimaryTouchpad) ? ovrAvatarTouch.One : 0,
            joystickPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, controller),
            indexTrigger = 0f,
            handTrigger = 0f,
            isActive = (OVRInput.GetActiveController() & controller) != 0,
        };
    }

    float voiceAmplitude = 0.0f;
    ControllerPose GetControllerPose(VRTK_ControllerEvents controller)
    {
        ovrAvatarButton buttons = 0;
        //if (OVRInput.Get(OVRInput.Button.One, controller)) buttons |= ovrAvatarButton.One;
        //if (OVRInput.Get(OVRInput.Button.Two, controller)) buttons |= ovrAvatarButton.Two;
        if (controller.startMenuPressed) buttons |= ovrAvatarButton.Three;
        if (controller.touchpadPressed) buttons |= ovrAvatarButton.Joystick;

        ovrAvatarTouch touches = 0;
        //if (OVRInput.Get(OVRInput.Touch.One, controller)) touches |= ovrAvatarTouch.One;
        //if (OVRInput.Get(OVRInput.Touch.Two, controller)) touches |= ovrAvatarTouch.Two;
        //if (controller.touchpadPressed) touches |= ovrAvatarTouch.Joystick;
        //if (OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, controller)) touches |= ovrAvatarTouch.ThumbRest;
        if (controller.triggerPressed) touches |= ovrAvatarTouch.Index;
        if (controller.gripPressed) touches |= ovrAvatarTouch.Joystick;
        if (controller.touchpadPressed) touches |= ovrAvatarTouch.Pointing;
        if (controller.triggerPressed && controller.gripPressed) touches = ovrAvatarTouch.ThumbUp;
        //if (!OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, controller)) touches |= ovrAvatarTouch.ThumbUp;

        return new ControllerPose
        {
            buttons = buttons,
            touches = touches,
            joystickPosition = controller.GetTouchpadAxis(),
            indexTrigger = controller.GetGripAxis(),
            handTrigger = controller.GetTriggerAxis(),
            isActive = true
        };
    }

    private void CalculateCurrentPose()
    {
        Transform head = VRTK_DeviceFinder.HeadsetTransform();
        Transform left = VRTK_DeviceFinder.GetControllerLeftHand().transform;
        Transform right = VRTK_DeviceFinder.GetControllerRightHand().transform;

        CurrentPose = new PoseFrame
        {
            voiceAmplitude = voiceAmplitude,
            headPosition = head.position,

            headRotation = head.rotation,
            
            handLeftPosition = left.TransformPoint(new Vector3(0.025f, 0, -0.04f)),
            handLeftRotation = left.rotation,
            handRightPosition = right.TransformPoint(new Vector3(-0.025f, 0, -0.04f)),
            handRightRotation = right.rotation,

            controllerLeftPose = GetControllerPose(left.GetComponent<VRTK_ControllerEvents>()),
            controllerRightPose = GetControllerPose(right.GetComponent<VRTK_ControllerEvents>())
        };
    }

    public override void UpdateTransforms(IntPtr sdkAvatar)
    {
        CalculateCurrentPose();
        UpdateTransformsFromPose(sdkAvatar);
    }
}
