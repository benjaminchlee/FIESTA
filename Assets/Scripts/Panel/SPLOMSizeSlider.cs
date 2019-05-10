using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VRTK;
using VRTK.Controllables;
using VRTK.Controllables.PhysicsBased;

public class SPLOMSizeSlider : MonoBehaviour {

    private VRTK_PhysicsSlider physicsSlider;
    private VRTK_InteractableObject interactableObject;

    [SerializeField]
    private float startValue;
    [SerializeField]
    private TextMeshPro valueLabel;

    [Serializable]
    public class ScatterplotMatrixSizeSliderValueChangedEvent : UnityEvent<float> { }

    public ScatterplotMatrixSizeSliderValueChangedEvent ScatterplotMatrixSizeSliderValueChanged;

    private float previousValue;

    private void Start()
    {
        physicsSlider = GetComponent<VRTK_PhysicsSlider>();
        interactableObject = GetComponent<VRTK_InteractableObject>();

        physicsSlider.stepValueRange = new Limits2D(2, ChartManager.Instance.DataSource.DimensionCount);
        physicsSlider.SetValue(physicsSlider.maximumLength / (ChartManager.Instance.DataSource.DimensionCount - 2) * (startValue - 2));
        
        physicsSlider.ValueChanged += OnSizeSliderValueChanged;
        interactableObject.InteractableObjectUngrabbed += OnSizeSliderUngrabbed;

        previousValue = startValue;
    }

    private void OnSizeSliderValueChanged(object sender, ControllableEventArgs e)
    {
        if (previousValue != e.value)
        {
            if (valueLabel != null)
            {
                valueLabel.text = e.value.ToString();
            }

            ScatterplotMatrixSizeSliderValueChanged.Invoke(e.value);

            previousValue = e.value;
        }
    }

    private void OnSizeSliderUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        physicsSlider.SetValue(physicsSlider.GetValue());
    }
}
