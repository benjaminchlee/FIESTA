using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class SPLOMButton : MonoBehaviour {

    [SerializeField]
    private TextMeshPro textMesh;

    [Serializable]
    public class SPLOMButtonClickedEvent : UnityEvent<SPLOMButton> { }
    public SPLOMButtonClickedEvent SPLOMButtonClicked;

    private List<string> dimensions;

    public string Text
    {
        get { return textMesh.text; }
        set
        {
            textMesh.text = value;
        }
    }

    private VRTK_InteractableObject interactableObject;

    private void Start()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        interactableObject = GetComponent<VRTK_InteractableObject>();
        interactableObject.InteractableObjectUsed += OnSPLOMButtonClicked;

        dimensions = GetAttributesList();
    }

    private void OnSPLOMButtonClicked(object sender, InteractableObjectEventArgs e)
    {
        int index = dimensions.IndexOf(Text);
        index = (index + 1) % dimensions.Count;

        textMesh.text = dimensions[index];

        SPLOMButtonClicked.Invoke(this);
    }

    private List<string> GetAttributesList()
    {
        List<string> dimensions = new List<string>();
        for (int i = 0; i < ChartManager.Instance.DataSource.DimensionCount; ++i)
        {
            dimensions.Add(ChartManager.Instance.DataSource[i].Identifier);
        }
        return dimensions;
    }
}
