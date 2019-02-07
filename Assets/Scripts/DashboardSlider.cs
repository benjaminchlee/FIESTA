using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VRTK;
using VRTK.Controllables;
using VRTK.Controllables.PhysicsBased;

public class DashboardSlider : MonoBehaviour {

    private VRTK_PhysicsSlider physicsSlider;
    private VRTK_InteractableObject interactableObject;

    [SerializeField]
    private float startValue;
    [SerializeField]
    private TextMeshPro valueLabel;
    [SerializeField]
    private bool labelAsPercentage = false;

    [Serializable]
    public class DashboardSliderValueChangedEvent : UnityEvent<float> { }

    public DashboardSliderValueChangedEvent DashboardSliderValueChanged;

    private float previousValue;

    private void Start()
    {
        physicsSlider = GetComponent<VRTK_PhysicsSlider>();
        interactableObject = GetComponent<VRTK_InteractableObject>();

        physicsSlider.ValueChanged += OnSliderValueChanged;
        interactableObject.InteractableObjectUngrabbed += OnSliderUngrabbed;
        
        physicsSlider.SetValue((startValue - physicsSlider.stepValueRange.minimum) / (physicsSlider.stepValueRange.maximum - physicsSlider.stepValueRange.minimum) * physicsSlider.maximumLength);

        previousValue = startValue;
    }

    private void OnSliderValueChanged(object sender, ControllableEventArgs e)
    {
        if (previousValue != e.value)
        {
            if (valueLabel != null)
            {
                if (labelAsPercentage)
                    valueLabel.text = (e.value * 100) + "%";
                else
                    valueLabel.text = e.value.ToString();
            }

            DashboardSliderValueChanged.Invoke(e.value);

            previousValue = e.value;
        }
    }

    private void OnSliderUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        physicsSlider.SetValue(physicsSlider.GetValue());
    }
}