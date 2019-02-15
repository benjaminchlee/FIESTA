using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HueSlider : MonoBehaviour {

    [Serializable]
    public class HueChangedEvent : UnityEvent<float> { }
    public HueChangedEvent HueChanged;

    [SerializeField]
    private float minClamp = -0.5f;
    [SerializeField]
    private float maxClamp = 0.5f;

    private float previousX;

    public void SetHue(float value)
    {
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
