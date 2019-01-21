using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;
using VRTK.Controllables;
using VRTK.Controllables.PhysicsBased;

public class WorkScreenSlider : MonoBehaviour {

    private VRTK_PhysicsSlider physicsSlider;

    [SerializeField]
    private float startValue;

    [Serializable]
    public class WorkShelfSliderValueChangedEvent : UnityEvent<float> { }

    public WorkShelfSliderValueChangedEvent WorkShelfSliderValueChanged;

    private void Start()
    {
        physicsSlider = GetComponent<VRTK_PhysicsSlider>();

        physicsSlider.SetValue((startValue - physicsSlider.stepValueRange.minimum) / (physicsSlider.stepValueRange.maximum - physicsSlider.stepValueRange.minimum) * physicsSlider.maximumLength);

        physicsSlider.ValueChanged += OnSizeSliderValueChanged;
    }

    private void OnSizeSliderValueChanged(object sender, ControllableEventArgs e)
    {
        WorkShelfSliderValueChanged.Invoke(e.value);
    }
}