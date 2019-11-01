using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class HueSlider : MonoBehaviour {

    [Serializable]
    public class HueChangedEvent : UnityEvent<float> { }
    public HueChangedEvent HueChanged;

    [SerializeField]
    private float minClamp = -0.5f;
    [SerializeField]
    private float maxClamp = 0.5f;

    private float previousX;

    [SerializeField]
    private VRTK_InteractableObject interactableObject;

    private void Awake()
    {
        interactableObject.InteractableObjectGrabbed += SliderGrabbed;
        interactableObject.InteractableObjectUngrabbed += SliderUngrabbed;
    }

    private void SliderGrabbed(object sender, InteractableObjectEventArgs e)
    {
        DataLogger.Instance.LogActionData(this, GetComponentInParent<Panel>().OriginalOwner, GetComponentInParent<Panel>().OriginalOwner, gameObject.name + " Drag start");
    }

    private void SliderUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        DataLogger.Instance.LogActionData(this, GetComponentInParent<Panel>().OriginalOwner, GetComponentInParent<Panel>().OriginalOwner, gameObject.name + " Drag end");
    }

    public void SetHue(float value)
    {
        value = 1 - value;

        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Lerp(minClamp, maxClamp, value);
        transform.localPosition = pos;

        previousX = pos.x;

        HueChanged.Invoke(GetHue());
    }

    public float GetHue()
    {
        return 1 - (transform.localPosition.x - minClamp) / (maxClamp - minClamp);
    }

    private void FixedUpdate()
    {
        Vector3 pos = transform.localPosition;

        pos.x = Mathf.Clamp(transform.localPosition.x, minClamp, maxClamp);
        pos.y = 0;
        pos.z = 0;

        transform.localPosition = pos;

        if (pos.x != previousX)
        {
            previousX = pos.x;
            HueChanged.Invoke(GetHue());
        }
    }
}
