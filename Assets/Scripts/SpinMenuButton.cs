using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class SpinMenuButton : VRTK_InteractableObject {
    
    [Tooltip("The function to be called when the user clicks on this button with a controller.")]
    public UnityEvent OnClick = new UnityEvent();

    private SpinMenu menu;
    private Coroutine activeCoroutine;

    private void Start()
    {
        menu = GetComponentInParent<SpinMenu>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        InteractableObjectUsed += OnSpinMenuButtonUsed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        InteractableObjectUsed -= OnSpinMenuButtonUsed;
    }

    private void OnSpinMenuButtonUsed(object sender, InteractableObjectEventArgs e)
    {
        InteractionsManager.Instance.RangedMenuFinished();
        OnClick.Invoke();
        menu.ActiveButtonChanged(this);
    }

    public void AnimateToPosition(Vector3 position, Vector3 scale, float time)
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine = StartCoroutine(AnimateTowardsPosition(position, scale, time));
    }

    private IEnumerator AnimateTowardsPosition(Vector3 position, Vector3 scale, float time)
    {
        Vector3 startPosition = transform.localPosition;
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            transform.localPosition = Vector3.Lerp(startPosition, position, elapsedTime / time);
            transform.localScale = Vector3.Lerp(startScale, scale, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        // Force the object to end at the target
        transform.localPosition = position;
        transform.localScale = scale;
    }
}
