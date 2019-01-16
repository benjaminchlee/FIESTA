using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using TMPro;
using System;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour {

    [SerializeField]
    private TextMeshPro textMesh;

    [System.Serializable]
    public class ButtonClickedEvent : UnityEvent<MenuButton> {}

    public ButtonClickedEvent ButtonClicked;

    public string Text
    {
        get { return textMesh.text; }
        set { textMesh.text = value; }
    }

    private VRTK_InteractableObject interactableObject;
    private Coroutine activeCoroutine;

    private void Start()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        interactableObject = GetComponent<VRTK_InteractableObject>();
        interactableObject.InteractableObjectUsed += OnButtonClicked;
    }

    public void AnimateTowards(Vector3 targetPos, float duration, bool toDisable = false)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(AnimateTo(targetPos, duration, toDisable));
    }

    private IEnumerator AnimateTo(Vector3 targetPos, float duration, bool toDisable = false)
    {
        float time = 0;
        Vector3 startPos = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        if (toDisable)
            gameObject.SetActive(false);
    }

    private void OnButtonClicked(object sender, InteractableObjectEventArgs e)
    {
        ButtonClicked.Invoke(this);
    }
}
