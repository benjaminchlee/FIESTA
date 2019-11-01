using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IATK;
using UnityEngine;
using VRTK;

public class AxisNormaliser : MonoBehaviour {

    [SerializeField]
    private VRTK_InteractableObject interactableObject;
    [SerializeField]
    private bool isMinHandle;  // true = min, false = max
    
    private Axis parentAxis;
    private Vector3 initialScale = Vector3.one;
    private Vector3 rescaled = Vector3.one;
    private float initX = 0f;
    private int myDirection; // 1=x, 2=y, 3=z
    private Vector3 storedPosition;
    private Quaternion storedRotation;
    private Chart parentChart;

    private bool isLeftTouched = false;
    private bool isRightTouched = false;

    private void Awake()
    {
        interactableObject.InteractableObjectTouched += NormaliserTouched;
        interactableObject.InteractableObjectUntouched += NormaliserUntouched;

        interactableObject.InteractableObjectGrabbed += NormaliserGrabbed;
        interactableObject.InteractableObjectUngrabbed += NormaliserUngrabbed;
    }

    private void Start()
    {
        parentAxis = GetComponentInParent<Axis>();
        initialScale = transform.localScale;
        rescaled = initialScale;
        rescaled *= 2f;

        initX = transform.localPosition.x;

        myDirection = parentAxis.MyDirection;

        storedRotation = transform.localRotation;

        parentChart = GetComponentInParent<Chart>();
    }

    private void OnDestroy()
    {
        interactableObject.InteractableObjectNearTouched -= NormaliserTouched;
        interactableObject.InteractableObjectNearUntouched -= NormaliserUntouched;

        interactableObject.InteractableObjectUngrabbed -= NormaliserUngrabbed;
    }

    private void Update()
    {
        SetNormaliserPosition();
    }

    private void SetNormaliserPosition()
    {
        if (interactableObject.IsGrabbed())
        {
            float offset = parentAxis.CalculateLinearMapping(interactableObject.GetGrabbingObject().transform);
            offset = Mathf.Clamp(offset, 0, 1);

            if (myDirection == 1)
            {
                if (isMinHandle)
                    parentChart.XNormaliser = new Vector2(offset, parentChart.XNormaliser.y);
                else
                    parentChart.XNormaliser = new Vector2(parentChart.XNormaliser.x, offset);
            }
            else if (myDirection == 2)
            {
                if (isMinHandle)
                    parentChart.YNormaliser = new Vector2(offset, parentChart.YNormaliser.y);
                else
                    parentChart.YNormaliser = new Vector2(parentChart.YNormaliser.x, offset);
            }
            else if (myDirection == 3)
            {
                if (isMinHandle)
                    parentChart.ZNormaliser = new Vector2(offset, parentChart.ZNormaliser.y);
                else
                    parentChart.ZNormaliser = new Vector2(parentChart.ZNormaliser.x, offset);
            }

            Vector3 newPos = Vector3.Lerp(parentAxis.MinPosition, parentAxis.MaxPosition, offset);
            newPos.x = initX;
            newPos.z = 0;
            transform.localPosition = newPos;
            transform.localRotation = storedRotation;

            storedPosition = newPos;

            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(interactableObject.GetGrabbingObject()), 0.075f);
        }
    }

    private void NormaliserGrabbed(object sender, InteractableObjectEventArgs e)
    {
        DataLogger.Instance.LogActionData(parentChart, parentChart.OriginalOwner, parentChart.photonView.Owner, "Vis Normalise start", parentChart.ID);
    }

    private void NormaliserUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        transform.localPosition = storedPosition;
        transform.localRotation = storedRotation;

        DataLogger.Instance.LogActionData(parentChart, parentChart.OriginalOwner, parentChart.photonView.Owner, "Vis Normalise end", parentChart.ID);
    }

    //public void OnEnter(WandController controller)
    //{
    //    OnEntered.Invoke();

    //    transform.localScale = rescaled;
    //}

    //public void OnExit(WandController controller)
    //{
    //    OnExited.Invoke();
    //    transform.localScale = initialScale;

    //}
    //public void OnDrag(WandController controller)
    //{
    //    float offset = parentAxis.CalculateLinearMapping(controller.transform);
    //    Vector3 newP = Vector3.Lerp(parentAxis.MinPosition, parentAxis.MaxPosition, offset);
    //    transform.position = newP;
    //}

    private void NormaliserTouched(object sender, InteractableObjectEventArgs e)
    {
        if (VRTK_DeviceFinder.IsControllerLeftHand(e.interactingObject))
            isLeftTouched = true;
        else
            isRightTouched = true;

        if (isLeftTouched ^ isRightTouched)
            transform.DOScale(rescaled, 0.35f);
    }

    private void NormaliserUntouched(object sender, InteractableObjectEventArgs e)
    {
        if (VRTK_DeviceFinder.IsControllerLeftHand(e.interactingObject))
            isLeftTouched = false;
        else
            isRightTouched = false;

        if (!isLeftTouched && !isRightTouched)
            transform.DOScale(initialScale, 0.35f);
    }
}
