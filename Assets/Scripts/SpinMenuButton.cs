using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class SpinMenuButton : MonoBehaviourPunCallbacks {
    
    [SerializeField]
    private VRTK_InteractableObject interactableObject;
    [SerializeField]
    private TextMeshPro label;
    [SerializeField]
    private Renderer renderer;
    [SerializeField]
    private string rangedInteractionsToolName;

    [Serializable]
    public class ButtonClickedEvent : UnityEvent<SpinMenuButton> { }
    public ButtonClickedEvent ButtonClicked;

    public string Text
    {
        get { return label.text; }
        set { photonView.RPC("PropagateText", RpcTarget.All, value); }
    }

    [PunRPC]
    private void PropagateText(string value)
    {
        label.text = value;
    }

    public Color Color
    {
        get { return renderer.material.color; }
        set { photonView.RPC("PropagateColor", RpcTarget.All, value); }
    }

    [PunRPC]
    private void PropagateColor(Color value)
    {
        renderer.material.color = value;
    }
    
    public string RangedInteractionsToolName
    {
        get { return rangedInteractionsToolName;}
    }

    private void Start()
    {
        interactableObject.InteractableObjectUsed += OnSpinMenuButtonUsed;
    }

    private void OnDestroy()
    {
        interactableObject.InteractableObjectUsed -= OnSpinMenuButtonUsed;
    }

    private void OnSpinMenuButtonUsed(object sender, InteractableObjectEventArgs e)
    {
        ButtonClicked.Invoke(this);
    }

    public void AnimateToPosition(Vector3 position, Vector3 scale, float time)
    {
        transform.DOLocalMove(position, time);
        transform.DOScale(scale, time);
    }
}
