using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRTK;
using Util;

public class CloseInteractions : MonoBehaviour {
    
    [Header("General Parameters")] [SerializeField] [Tooltip("The distance the user needs to move the controller to engage the brush.")]
    private float movementDistanceThreshold = 0.02f;
    [SerializeField] [Tooltip("The duration the user needs to hold in the same spot to engage the rectangle.")]
    private float holdDurationThreshold = 0.25f;

    [Header("Brush Selection Parameters")]
    [SerializeField] [Tooltip("The prefab of the brush to use.")]
    private GameObject brushPrefab;
    [SerializeField] [Tooltip("The minimum size of the brush.")]
    private float brushMinSize = 0.01f;
    [SerializeField] [Tooltip("The maximum size of the brush.")]
    private float brushMaxSize = 0.1f;
    [SerializeField] [Tooltip("The distance from the screen until the brush is its maximum size.")]
    private float brushMaxDistance = 0.4f;

    private GameObject brushObject;
    private Rigidbody brushRigidBody;

    [Header("Rectangle Selection Parameters")]
    [SerializeField] [Tooltip("The width of the line drawn while selecting.")]
    private float rectangleSelectWidth = 0.01f;
    [SerializeField] [Tooltip("The maximum vibration intensity of the controller while selecting. This scales linearly with the distance between both controllers.")]
    private float rectangleSelectMaxVibrateIntensity = 0.75f;
    [SerializeField] [Tooltip("The maximum distance between both controllers before the vibration intensity maxes out.")]
    private float rectangleSelectMaxVibrateDistance = 1f;
    [SerializeField] [Tooltip("The material of the rectangle while selectiong.")]
    private Material rectangleSelectMaterial;

    private GameObject selectionSquare;
    private Vector3 rectangleStartPosition;

    public GameObject screen;

    private VRTK_ControllerEvents controllerEvents;

    private bool isTriggerDown = false;
    private bool isGripDown = false;

    private bool isEnabled = false;
    private Coroutine waitForMovement;
    private InteractionState activeState = InteractionState.None;
    private SelectionMode selectionMode = SelectionMode.Selecting;

    private LineRenderer lineRenderer;

    private enum InteractionState
    {
        None,
        Brush,
        Rectangle,
    }

    private enum SelectionMode
    {
        Selecting,
        Deselecting
    }

    public string ActiveState
    {
        get { return activeState.ToString(); }
    }

    private void Start() {
        controllerEvents = GetComponent<VRTK_ControllerEvents>();

        controllerEvents.TriggerClicked += OnTriggerClicked;
        controllerEvents.TriggerUnclicked += OnTriggerUnclicked;

        controllerEvents.GripClicked += OnGripClicked;
        controllerEvents.GripUnclicked += OnGripUnclicked;
        
        // Instantiate brush
        brushObject = Instantiate(brushPrefab);
        brushObject.SetActive(false);
        brushRigidBody = brushObject.GetComponent<Rigidbody>();

        // Instantiate rectangle
        selectionSquare = GameObject.CreatePrimitive(PrimitiveType.Cube);
        selectionSquare.name = "Selection Square";
        // Set it to ignore raycasts
        selectionSquare.layer = 2;
        selectionSquare.transform.SetParent(screen.transform);
        selectionSquare.GetComponent<Renderer>().material = rectangleSelectMaterial;
        LineRenderer lr = selectionSquare.AddComponent<LineRenderer>();
        lr.positionCount = 5;
        lr.SetPositions(new Vector3[] { new Vector3(0.5f, 0.5f, 0), new Vector3(-0.5f, 0.5f, 0), new Vector3(-0.5f, -0.5f, 0), new Vector3(0.5f, -0.5f, 0), new Vector3(0.5f, 0.5f, 0) });
        lr.material = rectangleSelectMaterial;
        lr.startWidth = rectangleSelectWidth;
        lr.endWidth = rectangleSelectWidth;
        lr.useWorldSpace = false;
        selectionSquare.SetActive(false);

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
    }


    private void Update()
    {
        if (activeState != InteractionState.None)
        {
            lineRenderer.enabled = true;

            Vector3 localPos = screen.transform.InverseTransformPoint(transform.position);
            localPos.z = 0;
            Vector3 worldPos = screen.transform.TransformPoint(localPos);

            lineRenderer.SetPositions(new Vector3[] { transform.position, worldPos });
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        switch (activeState)
        {
            case InteractionState.Brush:
                BrushLoop();
                break;

            case InteractionState.Rectangle:
                RectangleLoop();
                break;
        }
    }
    
    private void SetInteractionState(InteractionState state)
    {
        switch (state)
        {
            case InteractionState.None:
                // Force override the triggers to be set as down so that the user has to release and click again for new interactions
                isTriggerDown = false;
                isGripDown = false;
                if (waitForMovement != null)
                    StopCoroutine(waitForMovement);
                break;

            case InteractionState.Brush:
                BrushStart();
                break;

            case InteractionState.Rectangle:
                RectangleStart();
                break;
        }

        activeState = state;
        Debug.Log("Close interaction state changed to " + state.ToString());

    }

    private void SetSelectionMode(SelectionMode mode)
    {
        selectionMode = mode;

        switch (activeState)
        {
            case InteractionState.Brush:
                BrushModeChanged();
                break;

            case InteractionState.Rectangle:
                RectangleModeChanged();
                break;
        }
    }

    private IEnumerator WaitForMovement()
    {
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        
        while (elapsedTime < holdDurationThreshold)
        {
            // If at any point no input is being held down, stop immediately
            if (!isTriggerDown && !isGripDown)
            {
                yield break;
            }

            // If the controller has been moved enough from its starting position
            if (Vector3.Distance(startPos, transform.position) > movementDistanceThreshold)
            {
                SetInteractionState(InteractionState.Brush);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // If enough time has elapsed
        SetInteractionState(InteractionState.Rectangle);
    }

    public void Enable()
    {
        isEnabled = true;

        isTriggerDown = false;
        isGripDown = false;
    }

    public void Disable()
    {
        isEnabled = false;
    }

    public void DisableAndInterrupt()
    {
        if (isEnabled)
        {
            Disable();
            SetInteractionState(InteractionState.None);
        }
    }

    private void OnTriggerClicked(object sender, ControllerInteractionEventArgs e)
    {
        if (isEnabled)
        {
            isTriggerDown = true;
            SetSelectionMode(SelectionMode.Selecting);

            // If there is no interaction already ongoing, start a new one
            if (activeState == InteractionState.None)
            {
                // Stop any already running coroutine to override it with this new one
                if (waitForMovement != null)
                    StopCoroutine(waitForMovement);
                waitForMovement = StartCoroutine(WaitForMovement());
            }
        }
    }

    private void OnTriggerUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        isTriggerDown = false;
        
        // If the grip is still held down, swap to deselection
        if (isGripDown)
        {
            SetSelectionMode(SelectionMode.Deselecting);
        }
        else
        {
            switch (activeState)
            {
                case InteractionState.None:
                    SetInteractionState(InteractionState.None);
                    break;

                case InteractionState.Brush:
                    if (selectionMode == SelectionMode.Selecting)
                        BrushEnd();
                    break;

                case InteractionState.Rectangle:
                    if (selectionMode == SelectionMode.Selecting)
                        RectangleEnd();
                    break;
            }
        }
    }

    private void OnGripClicked(object sender, ControllerInteractionEventArgs e)
    {
        if (isEnabled)
        {
            isGripDown = true;
            SetSelectionMode(SelectionMode.Deselecting);

            // If there is no interaction already ongoing, start a new one
            if (activeState == InteractionState.None)
            {
                // Stop any already running coroutine to override it with this new one
                if (waitForMovement != null)
                    StopCoroutine(waitForMovement);
                waitForMovement = StartCoroutine(WaitForMovement());
            }
        }
    }

    private void OnGripUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        isGripDown = false;
        
        // If the trigger is still held down, swap to selection
        if (isTriggerDown)
        {
            SetSelectionMode(SelectionMode.Selecting);
        }
        else
        {
            switch (activeState)
            {
                case InteractionState.None:
                    SetInteractionState(InteractionState.None);
                    break;

                case InteractionState.Brush:
                    if (selectionMode == SelectionMode.Deselecting)
                        BrushEnd();
                    break;

                case InteractionState.Rectangle:
                    if (selectionMode == SelectionMode.Deselecting)
                        RectangleEnd();
                    break;
            }
        }
    }
    
    private void BrushStart()
    {
        return;
    }

    private void BrushLoop()
    {
        if (!brushObject.activeSelf)
        {
            brushObject.SetActive(true);
            BrushModeChanged();
        }

        Vector3 localPos = screen.transform.InverseTransformPoint(transform.position);
        localPos.z = 0;
        Vector3 worldPos = screen.transform.TransformPoint(localPos);
        brushRigidBody.MovePosition(worldPos);

        float distance = Vector3.Distance(transform.position, worldPos);

        float size = Mathf.Lerp(brushMinSize, brushMaxSize, Mathf.Min(distance / brushMaxDistance, 1));
        brushObject.transform.localScale = Vector3.one * size;
    }

    private void BrushEnd()
    {
        SetInteractionState(InteractionState.None);

        brushObject.SetActive(false);
    }
    
    private void BrushModeChanged()
    {
        float radius = brushObject.transform.localScale.x / 2;
        /*
        // Instantly change the selection mode of shapes the brush is currently over
        Collider[] colliders = Physics.OverlapSphere(brushObject.transform.position, radius);
        foreach (Collider col in colliders)
        {
            if (col.gameObject.tag == "Shape")
            {
                InteractableShape shapeScript = col.gameObject.GetComponent<InteractableShape>();

                if (!shapeScript.IsSelected && selectionMode == SelectionMode.Selecting)
                    shapeScript.ShapeSelected();
                else if (shapeScript.IsSelected && selectionMode == SelectionMode.Deselecting)
                    shapeScript.ShapeDeselected();
            }
        }
        */
        brushObject.GetComponent<Brush>().SetBrushMode(selectionMode == SelectionMode.Selecting ? Brush.BrushMode.Select : Brush.BrushMode.Deselect);
    }

    private void RectangleStart()
    {
        rectangleStartPosition = transform.position;
    }

    private void RectangleLoop()
    {
        if (!selectionSquare.activeSelf)
        {
            selectionSquare.SetActive(true);
        }
        selectionSquare.SetActive(true);
        Vector3 rectangleEndPosition = transform.position;

        Vector3 localStart = screen.transform.InverseTransformPoint(rectangleStartPosition);
        Vector3 localEnd = screen.transform.InverseTransformPoint(rectangleEndPosition);

        Vector3 localScale = localStart - localEnd;

        localScale.x = Mathf.Abs(localScale.x);
        localScale.y = Mathf.Abs(localScale.y);
        localScale.z = 0.02f;
        Vector3 localCenter = (localStart + localEnd) / 2;
        localCenter.z = 0;
        selectionSquare.transform.localPosition = localCenter;
        selectionSquare.transform.localScale = localScale;
        selectionSquare.transform.rotation = screen.transform.rotation;

        // Vibrate the controllers based on distance from each other
        float distance = Vector3.Distance(localStart, localEnd);
        float vibrateStrength = rectangleSelectMaxVibrateIntensity * Mathf.Min((distance / rectangleSelectMaxVibrateDistance), 1);

        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(gameObject), vibrateStrength);
    }

    private void RectangleEnd()
    {
        SetInteractionState(InteractionState.None);
        /*
        selectionSquare.transform.parent = null;
        // Increase the area where the selection is done so that it catches shapes hovering away from the screen
        Vector3 halfExtents = selectionSquare.transform.localScale / 2;
        halfExtents.z += 0.3f;

        Collider[] colliders = Physics.OverlapBox(selectionSquare.transform.position, halfExtents, selectionSquare.transform.rotation);
        List<int> indices = new List<int>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Shape")
            {
                InteractableShape shapeScript = collider.gameObject.GetComponent<InteractableShape>();
                indices.Add(shapeScript.Index);
            }
        }
        if (selectionMode == SelectionMode.Selecting)
            ScreenManager.Instance.ShapesSelected(indices.ToArray());
        else
            ScreenManager.Instance.ShapesDeselected(indices.ToArray());

        selectionSquare.transform.parent = screen.transform;
        */
        selectionSquare.SetActive(false);
    }

    private void RectangleModeChanged()
    {

    }
}
