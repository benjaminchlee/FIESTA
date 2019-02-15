using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomHSVBoxSlider : MonoBehaviour {


    [Serializable]
    public class CustomHSVBoxSliderEvent : UnityEvent<float, float> { }

    [SerializeField]
    private CustomHSVBoxSliderEvent m_OnValueChanged = new CustomHSVBoxSliderEvent();
    public CustomHSVBoxSliderEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }
}
