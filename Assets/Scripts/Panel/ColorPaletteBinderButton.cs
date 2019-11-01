using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class ColorPaletteBinderButton : MonoBehaviour {

    [SerializeField]
    private TextMeshPro textMesh;
    [SerializeField]
    private Renderer renderer;

    [Serializable]
    public class ButtonClickedEvent : UnityEvent<ColorPaletteBinderButton> { }
    public ButtonClickedEvent ButtonClicked;

    private VRTK_InteractableObject interactableObject;

    public string Text
    {
        get { return textMesh.text; }
        set { textMesh.text = value; }
    }
    
    private Color color;
    public Color Color
    {
        get { return color; }
        set
        {
            color = value;
            renderer.material.SetColor("_Color", value);
        }
    }

    public float NormalisedValue { get; set; }

    private void Start()
    {
        interactableObject = GetComponent<VRTK_InteractableObject>();
        interactableObject.InteractableObjectUsed += OnButtonClicked;
    }
    
    public void RangeClick()
    {
        ButtonClicked.Invoke(this);

        DataLogger.Instance.LogActionData(this, GetComponentInParent<Panel>().OriginalOwner, GetComponentInParent<Panel>().OriginalOwner, transform.parent.name + " Color Palette Button Range Clicked", "", Text);
    }

    private void OnButtonClicked(object sender, InteractableObjectEventArgs e)
    {
        ButtonClicked.Invoke(this);

        DataLogger.Instance.LogActionData(this, GetComponentInParent<Panel>().OriginalOwner, GetComponentInParent<Panel>().OriginalOwner, transform.parent.name + " Color Palette Button Clicked", "", Text);
    }
}
