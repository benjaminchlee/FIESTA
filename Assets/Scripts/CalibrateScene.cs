using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CalibrateScene : MonoBehaviour
{
    private GameObject sdkManagerGameObject;

    private void Start()
    {
        sdkManagerGameObject = FindObjectOfType<VRTK_SDKManager>().gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            RecenterPlayer();
        }
    }

    public void RecenterPlayer()
    {
        Transform headset = VRTK_DeviceFinder.HeadsetTransform();

        Vector3 newRotation = Vector3.up * headset.transform.eulerAngles.y;

        sdkManagerGameObject.transform.eulerAngles = newRotation;

        // Vector between headset position and origin is just the headset position

        // Translate the SDK manager object by this vector
        Vector3 newPos = sdkManagerGameObject.transform.position - VRTK_DeviceFinder.HeadsetTransform().position;
        newPos.y = sdkManagerGameObject.transform.position.y;
        sdkManagerGameObject.transform.position = newPos;
    }
}
