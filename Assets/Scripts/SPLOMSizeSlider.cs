using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;
using VRTK.Controllables;
using VRTK.Controllables.PhysicsBased;

public class SPLOMSizeSlider : MonoBehaviour {

    private VRTK_PhysicsSlider physicsSlider;

    [SerializeField]
    private float startValue;

    [Serializable]
    public class ScatterplotMatrixSizeSliderValueChangedEvent : UnityEvent<float> { }

    public ScatterplotMatrixSizeSliderValueChangedEvent ScatterplotMatrixSizeSliderValueChanged;

    private void Start()
    {
        physicsSlider = GetComponent<VRTK_PhysicsSlider>();

        physicsSlider.stepValueRange = new Limits2D(2, ChartManager.Instance.DataSource.DimensionCount);
        physicsSlider.SetValue(physicsSlider.maximumLength / (ChartManager.Instance.DataSource.DimensionCount - 2) * (startValue - 2));
        
        physicsSlider.ValueChanged += OnSizeSliderValueChanged;
    }

    private void OnSizeSliderValueChanged(object sender, ControllableEventArgs e)
    {
        ScatterplotMatrixSizeSliderValueChanged.Invoke(e.value);
    }
}
