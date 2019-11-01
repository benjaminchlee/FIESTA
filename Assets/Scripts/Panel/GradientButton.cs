using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class GradientButton : MonoBehaviour {

    [SerializeField]
    private MeshRenderer gradientMeshRenderer;
    [SerializeField]
    private TextMeshPro textMesh;
    
    [Serializable]
    public class ButtonClickedEvent : UnityEvent<GradientButton> { }
    public ButtonClickedEvent ButtonClicked;

    private VRTK_InteractableObject interactableObject;

    public string Text
    {
        get { return textMesh.text; }
        set { textMesh.text = value; }
    }

    public Texture Texture
    {
        get { return gradientMeshRenderer.material.mainTexture; }
        set { gradientMeshRenderer.material.mainTexture = value; }
    }

    public Gradient Gradient { get; set; }

    private void Start()
    {
        interactableObject = GetComponent<VRTK_InteractableObject>();
        interactableObject.InteractableObjectUsed += OnButtonClicked;
    }

    public void AnimateTowards(Vector3 targetPos, float duration, bool isLocalSpace = false, bool toDisable = false)
    {
        if (isLocalSpace)
            transform.DOLocalMove(targetPos, duration).SetEase(Ease.OutCirc).OnComplete(() => gameObject.SetActive(!toDisable));
        else
            transform.DOMove(targetPos, duration).SetEase(Ease.OutCirc).OnComplete(() => gameObject.SetActive(!toDisable));
    }

    public void SetGradientReversed(bool isReversed)
    {
        Vector3 scale = gradientMeshRenderer.transform.localScale;

        if (isReversed)
        {
            scale.x = Mathf.Min(-scale.x, scale.x);
        }
        else
        {
            scale.x = Mathf.Max(-scale.x, scale.x);
        }

        gradientMeshRenderer.transform.localScale = scale;
    }

    public void RangeClick()
    {
        ButtonClicked.Invoke(this);

        DataLogger.Instance.LogActionData(this, GetComponentInParent<Panel>().OriginalOwner, GetComponentInParent<Panel>().OriginalOwner, transform.parent.name + " Gradient Button Range Clicked", "", Text);
    }

    private void OnButtonClicked(object sender, InteractableObjectEventArgs e)
    {
        ButtonClicked.Invoke(this);

        DataLogger.Instance.LogActionData(this, GetComponentInParent<Panel>().OriginalOwner, GetComponentInParent<Panel>().OriginalOwner, transform.parent.name + " gradient button clicked", "", Text);
    }
}
