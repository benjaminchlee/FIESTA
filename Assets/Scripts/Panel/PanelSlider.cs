using IATK;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VRTK;
using VRTK.Controllables;
using VRTK.Controllables.PhysicsBased;

public class PanelSlider : MonoBehaviour {

    [SerializeField]
    private VRTK_PhysicsSlider physicsSlider;
    [SerializeField]
    private VRTK_InteractableObject interactableObject;

    [SerializeField]
    private AbstractVisualisation.PropertyType propertyType;
    [SerializeField]
    private float startValue;
    [SerializeField]
    private TextMeshPro valueLabel;
    [SerializeField]
    private bool labelAsPercentage = false;

    [Serializable]
    public class PanelSliderValueChangedEvent : UnityEvent<float> { }
    public PanelSliderValueChangedEvent PanelSliderValueChanged;

    private bool isInitialised = false;
    private float previousValue;
    private float storedValue;
    private bool setStoredValue;

    private void Start()
    {
        if (!isInitialised)
            Initialise();
    }

    private void Initialise()
    {
        physicsSlider.ValueChanged += OnSliderValueChanged;
        interactableObject.InteractableObjectGrabbed += OnSliderGrabbed;
        interactableObject.InteractableObjectUngrabbed += OnSliderUngrabbed;

        if (gameObject.activeInHierarchy)
            physicsSlider.SetValue((startValue - physicsSlider.stepValueRange.minimum) / (physicsSlider.stepValueRange.maximum - physicsSlider.stepValueRange.minimum) * physicsSlider.maximumLength);

        previousValue = startValue;

        isInitialised = true;
    }


    private void OnDestroy()
    {
        physicsSlider.ValueChanged -= OnSliderValueChanged;
        interactableObject.InteractableObjectUngrabbed -= OnSliderUngrabbed;
    }

    private void OnSliderGrabbed(object sender, InteractableObjectEventArgs e)
    {
        DataLogger.Instance.LogActionData(this, GetComponentInParent<Panel>().OriginalOwner, GetComponentInParent<Panel>().OriginalOwner, gameObject.name + " Drag start");
    }

    private void OnSliderUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        physicsSlider.SetValue(physicsSlider.GetValue());

        DataLogger.Instance.LogActionData(this, GetComponentInParent<Panel>().OriginalOwner, GetComponentInParent<Panel>().OriginalOwner, gameObject.name + " Drag end");
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

            PanelSliderValueChanged.Invoke(e.value);

            previousValue = e.value;

            physicsSlider.SetValue(physicsSlider.GetValue());
        }
    }

    public void ChartTransferred(Chart chart)
    {
        if (!isInitialised)
            Initialise();

        switch (propertyType)
        {
            case AbstractVisualisation.PropertyType.Size:
                if (chart.SizeDimension == "Undefined")
                {
                    storedValue = (chart.Size - physicsSlider.stepValueRange.minimum) / (physicsSlider.stepValueRange.maximum - physicsSlider.stepValueRange.minimum) * physicsSlider.maximumLength;
                }
                break;
        }

        // If the object is enabled, then its physics will work properly and the value can be set now
        if (gameObject.activeInHierarchy)
        {
            physicsSlider.SetValue(storedValue);
        }
        // Otherwise, store it for it to be set when the slider becomes enabled
        else
        {
            setStoredValue = true;
        }
    }

    private void OnEnable()
    {
        if (setStoredValue)
        {
            StartCoroutine(SetStoredValue());
        }
    }

    private IEnumerator SetStoredValue()
    {
        yield return null;

        physicsSlider.SetValue(storedValue);
        setStoredValue = false;
    }
}