using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VRTK;
using VRTK.Controllables;
using VRTK.Controllables.PhysicsBased;

public class WorkScreenSlider : MonoBehaviour {

    private VRTK_PhysicsSlider physicsSlider;

    [SerializeField]
    private float startValue;
    [SerializeField]
    private TextMeshPro valueLabel;
    [SerializeField]
    private bool labelAsPercentage = false;

    [Serializable]
    public class WorkShelfSliderValueChangedEvent : UnityEvent<float> { }

    public WorkShelfSliderValueChangedEvent WorkShelfSliderValueChanged;

    private void Start()
    {
        physicsSlider = GetComponent<VRTK_PhysicsSlider>();

        physicsSlider.ValueChanged += OnSliderValueChanged;

        physicsSlider.SetValue((startValue - physicsSlider.stepValueRange.minimum) / (physicsSlider.stepValueRange.maximum - physicsSlider.stepValueRange.minimum) * physicsSlider.maximumLength);
    }

    private void OnSliderValueChanged(object sender, ControllableEventArgs e)
    {
        if (valueLabel != null)
        {
            if (labelAsPercentage)
                valueLabel.text = (e.value * 100) + "%";
            else
                valueLabel.text = e.value.ToString();
        }

        WorkShelfSliderValueChanged.Invoke(e.value);
    }
}