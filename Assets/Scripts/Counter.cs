using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Counter : MonoBehaviourPunCallbacks {

    [SerializeField]
    private TextMeshPro counterLabel;
    [SerializeField]
    private MenuButton addButton;
    [SerializeField]
    private MenuButton subtractButton;

    [SerializeField]
    private int minValue;
    [SerializeField]
    private int maxValue;
    [SerializeField]
    private int startValue;
    [SerializeField]
    private bool maxValueIsNumDimensions = false;
    
    [Serializable]
    public class CounterValueChangedEvent : UnityEvent<int> { }

    public CounterValueChangedEvent CounterValueChanged;

    private void Start()
    {
        if (maxValueIsNumDimensions)
            maxValue = ChartManager.Instance.DataSource.DimensionCount;

        counterLabel.text = startValue.ToString();
    }

    private void OnEnable()
    {
        CounterValueChanged.Invoke(CurrentValue());
    }

    private int CurrentValue()
    {
        return int.Parse(counterLabel.text);
    }

    public void AddButtonClicked(MenuButton button)
    {
        int newValue = CurrentValue() + 1;

        if (newValue <= maxValue)
        {
            photonView.RPC("SetText", RpcTarget.All, newValue.ToString());
            
            CounterValueChanged.Invoke(newValue);
        }
    }

    public void SubtractButtonClicked(MenuButton button)
    {
        int newValue = CurrentValue() - 1;

        if (minValue <= newValue)
        {
            photonView.RPC("SetText", RpcTarget.All, newValue.ToString());

            CounterValueChanged.Invoke(newValue);
        }
    }

    [PunRPC]
    private void SetText(string value)
    {
        counterLabel.text = value;
    }
}
