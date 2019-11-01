using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using TMPro;
using System;
using DG.Tweening;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour {

    [SerializeField]
    private TextMeshPro textMesh;

    [Serializable]
    public class ButtonClickedEvent : UnityEvent<MenuButton> {}
    public ButtonClickedEvent ButtonClicked;

    public string Text
    {
        get { return textMesh.text; }
        set { textMesh.text = value; }
    }

    private VRTK_InteractableObject interactableObject;

    private void Start()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

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
    
    public void RangeClick()
    {
        ButtonClicked.Invoke(this);
        
        DataLogger.Instance.LogActionData(this, GetComponentInParent<Panel>().OriginalOwner, GetComponentInParent<Panel>().OriginalOwner, transform.parent.name + " Button Range Clicked", "", Text);
    }

    private void OnButtonClicked(object sender, InteractableObjectEventArgs e)
    {
        ButtonClicked.Invoke(this);

        DataLogger.Instance.LogActionData(this, GetComponentInParent<Panel>().OriginalOwner, GetComponentInParent<Panel>().OriginalOwner, transform.parent.name + " Button Clicked", "", Text);
    }
}
