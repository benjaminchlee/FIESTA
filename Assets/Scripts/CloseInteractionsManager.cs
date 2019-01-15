using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRTK;
using Util;

public class CloseInteractionsManager : MonoBehaviour {

    public static CloseInteractionsManager Instance { get; private set; }

    [Header("General Parameters")]
    [SerializeField] [Tooltip("The time to wait for input on both controllers before being set to unimanual.")]
    private float bimanualTimeThreshold = 0.1f;
    
    [Header("Brush/Lasso Selection Parameters")]
    [SerializeField] [Tooltip("The width of the line drawn while brushing.")]
    private float brushWidth = 0.005f;
    [SerializeField] [Tooltip("The color of the line drawn while brushing.")]
    private Color brushDrawColor = new Color(255, 255, 255);
    [SerializeField] [Tooltip("The color of the line drawn when the user forms a lasso selection.")]
    private Color brushCompleteColor = new Color(255, 255, 0);
    [SerializeField] [Tooltip("The material of the line drawn while brushing.")]
    private Material brushMaterial;
    [SerializeField] [Tooltip("The initial distance the user has to move the controller before a lasso selection is formable.")]
    private float brushPointInitialDistance = 0.05f;
    [SerializeField] [Tooltip("The distance that the user has to move the controller before another point is added to the line.")]
    private float brushPointDistanceInterval = 0.005f;
    [SerializeField] [Tooltip("The distance from the start point that the end point has to be for it to be registered as a lasso selection.")]
    private float brushPointCompleteDistance = 0.015f;

    private LineRenderer brushRenderer;
    private bool isBrushPastInitialDistance = false;
    private bool isBrushComplete = false;
    private bool isBrushLassoValid = true;

    [Header("Rectangle Selection Parameters")]
    [SerializeField] [Tooltip("The maximum distance both controllers can be from each other for a bimanual input to be recognised as a rectangle selection.")]
    private float rectangleSelectionControllerDistance = 0.2f;
    [SerializeField] [Tooltip("The width of the line drawn while selecting.")]
    private float rectangleSelectWidth = 0.01f;
    [SerializeField] [Tooltip("The maximum vibration intensity of the controller while selecting. This scales linearly with the distance between both controllers.")]
    private float rectangleSelectMaxVibrateIntensity = 0.75f;
    [SerializeField] [Tooltip("The maximum distance between both controllers before the vibration intensity maxes out.")]
    private float rectangleSelectMaxVibrateDistance = 1f;
    [SerializeField] [Tooltip("The material of the rectangle while selectiong.")]
    private Material rectangleSelectMaterial;

    private GameObject selectionSquare;

    [Header("Screen Object References")]
    [SerializeField] [Tooltip("The game object which represents the screen.")]
    public GameObject screen;
    [SerializeField] [Tooltip("The script used to draw convex meshes of selections.")]
    public ConvexMesh convexMesh;

    public GameObject leftController;
    public GameObject rightController;
    private GameObject unimanualController;

    private VRTK_ControllerEvents leftEvents;
    private VRTK_ControllerEvents rightEvents;

    private bool leftTriggerDown = false;
    private bool rightTriggerDown = false;

    private bool hasInteractionStarted = false;
    private bool hasBimanualThresholdElapsed = false;
    private bool isBimanual = false;
    private bool isEnabled = false;
    private Coroutine waitForBimanual;
    private InteractionState activeState = InteractionState.None;
    private SelectionMode selectionMode = SelectionMode.None;

    private bool leftGripDown = false;
    private bool rightGripDown = false;

    private enum InteractionState
    {
        None,
        Brush,
        Rectangle
    }

    private enum SelectionMode
    {
        None,
        Selecting,
        Deselecting
    }

    public string ActiveState
    {
        get { return activeState.ToString(); }
    }

    public bool IsSelecting
    {
        get { return selectionMode == SelectionMode.Selecting; }
    }

    public bool IsDeselecting
    {
        get { return selectionMode == SelectionMode.Deselecting; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
        //leftController = VRTK_DeviceFinder.GetControllerLeftHand();
        //rightController = VRTK_DeviceFinder.GetControllerRightHand();

        leftEvents = leftController.GetComponent<VRTK_ControllerEvents>();
        rightEvents = rightController.GetComponent<VRTK_ControllerEvents>();

        leftEvents.TriggerClicked += OnLeftTriggerClicked;
        leftEvents.TriggerUnclicked += OnLeftTriggerUnclicked;
        rightEvents.TriggerClicked += OnRightTriggerClicked;
        rightEvents.TriggerUnclicked += OnRightTriggerUnlicked;

        leftEvents.GripClicked += OnLeftGripClicked;
        leftEvents.GripUnclicked += OnLeftGripUnclicked;
        rightEvents.GripClicked += OnRightGripClicked;
        rightEvents.GripUnclicked += OnRightGripUnclicked;
       

        // Initialise brush/lasso
        brushRenderer = gameObject.AddComponent<LineRenderer>();
        brushRenderer.useWorldSpace = true;
        brushRenderer.positionCount = 0;  // Remove any anomalous default points
        brushRenderer.startWidth = brushWidth;
        brushRenderer.endWidth = brushWidth;
        brushRenderer.material = brushMaterial;
        brushRenderer.material.color = brushDrawColor;
        brushRenderer.enabled = false;

        // Initialise rectangle
        selectionSquare = GameObject.CreatePrimitive(PrimitiveType.Cube);
        selectionSquare.layer = 2;  // Set it to ignore raycasts
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
    }

    private void Update()
    {
        if (activeState == InteractionState.None)
        {
            // Do not start any new interactions if not enabled
            if (!isEnabled)
                return;

            // Wait until any of the triggers are pressed
            if (!hasInteractionStarted)
            {
                // Check for trigger (selecting) input
                if (leftTriggerDown ^ rightTriggerDown)
                {
                    hasInteractionStarted = true;

                    // If only one trigger was pressed, check to see if it will become bimanual
                    isBimanual = false;
                    hasBimanualThresholdElapsed = false;
                    waitForBimanual = StartCoroutine(WaitForTriggerBimanual());
                    selectionMode = SelectionMode.Selecting;
                }
                else if (leftTriggerDown && rightTriggerDown)
                {
                    hasInteractionStarted = true;

                    // If both triggers were pressed at exactly the same time, no checks are needed to see if it is bimanual
                    isBimanual = true;
                    hasBimanualThresholdElapsed = true;
                    selectionMode = SelectionMode.Selecting;
                }
                // Check for grip (deselecting) input
                else if (leftGripDown ^ rightGripDown)
                {
                    hasInteractionStarted = true;

                    // If only one grip was pressed, check to see if it will become bimanual
                    isBimanual = false;
                    hasBimanualThresholdElapsed = false;
                    waitForBimanual = StartCoroutine(WaitForGripBimanual());
                    selectionMode = SelectionMode.Deselecting;
                }
                else if (leftGripDown && rightGripDown)
                {
                    hasInteractionStarted = true;

                    // If both grips were pressed at exactly the same time, no checks are needed to see if it is bimanual
                    isBimanual = true;
                    hasBimanualThresholdElapsed = true;
                    selectionMode = SelectionMode.Deselecting;
                }
            }
            else
            {
                // Only determine what type of interaction it is after the specified time to wait for bimanual input has elapsed
                if (hasBimanualThresholdElapsed)
                {
                    if (isBimanual)
                    {
                        if (CalculateDistanceBetweenControllers() < rectangleSelectionControllerDistance)
                            SetInteractionState(InteractionState.Rectangle);
                        else
                            SetInteractionState(InteractionState.None);
                    }
                    else
                    {
                        if (IsSelecting)
                        {
                            unimanualController = leftTriggerDown ? leftController : rightController;
                        }
                        else
                        {
                            unimanualController = leftGripDown ? leftController : rightController;
                        }

                        SetInteractionState(InteractionState.Brush);
                    }
                }
            }
        }
        else
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
    }
    
    private void SetInteractionState(InteractionState state)
    {
        switch (state)
        {
            case InteractionState.None:
                InteractionsManager.Instance.CloseInteractionFinished();
                // Force override the triggers to be set as down so that the user has to release and click again for new interactions
                leftTriggerDown = false;
                rightTriggerDown = false;
                hasInteractionStarted = false;
                hasBimanualThresholdElapsed = false;
                selectionMode = SelectionMode.None;
                if (waitForBimanual != null)
                    StopCoroutine(waitForBimanual);
                break;

            case InteractionState.Rectangle:
                InteractionsManager.Instance.CloseInteractionStarted();
                RectangleStart();
                break;

            case InteractionState.Brush:
                InteractionsManager.Instance.CloseInteractionStarted();
                BrushStart();
                break;
        }

        activeState = state;
        Debug.Log("Close interaction state changed to " + state.ToString());
    }

    private IEnumerator WaitForTriggerBimanual()
    {
        float elapsedTime = 0;
        while (elapsedTime < bimanualTimeThreshold)
        {
            if (leftTriggerDown && rightTriggerDown)
            {
                isBimanual = true;
                hasBimanualThresholdElapsed = true;
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hasBimanualThresholdElapsed = true;
    }

    private IEnumerator WaitForGripBimanual()
    {
        float elapsedTime = 0;
        while (elapsedTime < bimanualTimeThreshold)
        {
            if (leftGripDown && rightGripDown)
            {
                isBimanual = true;
                hasBimanualThresholdElapsed = true;
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hasBimanualThresholdElapsed = true;
    }

    private float CalculateDistanceBetweenControllers()
    {
        // If either controller is disabled, the distance between them is infinity
        if (!leftController.activeSelf || !rightController.activeSelf)
            return Mathf.Infinity;
        else
            return Vector3.Distance(leftController.transform.position, rightController.transform.position);
    }

    public void Enable()
    {
        isEnabled = true;

        leftTriggerDown = false;
        rightTriggerDown = false;
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

    private void OnLeftTriggerClicked(object sender, ControllerInteractionEventArgs e)
    {
        if (isEnabled)
            leftTriggerDown = true;
    }

    private void OnLeftTriggerUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        leftTriggerDown = false;
        
        if (IsSelecting)
        {
            switch (activeState)
            {
                case InteractionState.None:
                    SetInteractionState(InteractionState.None);
                    break;

                case InteractionState.Brush:
                    BrushTriggerReleased(leftController);
                    break;

                case InteractionState.Rectangle:
                    RectangleTriggerReleased(leftController);
                    break;
            }
        }
    }

    private void OnRightTriggerClicked(object sender, ControllerInteractionEventArgs e)
    {
        if (isEnabled)
            rightTriggerDown = true;
    }

    private void OnRightTriggerUnlicked(object sender, ControllerInteractionEventArgs e)
    {
        rightTriggerDown = false;

        if (IsSelecting)
        {
            switch (activeState)
            {
                case InteractionState.None:
                    SetInteractionState(InteractionState.None);
                    break;

                case InteractionState.Brush:
                    BrushTriggerReleased(rightController);
                    break;

                case InteractionState.Rectangle:
                    RectangleTriggerReleased(rightController);
                    break;
            }
        }
    }

    private void OnLeftGripClicked(object sender, ControllerInteractionEventArgs e)
    {
        if (isEnabled)
            leftGripDown = true;
    }

    private void OnLeftGripUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        leftGripDown = false;

        if (IsDeselecting)
        {
            switch (activeState)
            {
                case InteractionState.None:
                    SetInteractionState(InteractionState.None);
                    break;

                case InteractionState.Brush:
                    BrushTriggerReleased(leftController);
                    break;

                case InteractionState.Rectangle:
                    RectangleTriggerReleased(leftController);
                    break;
            }
        }
    }

    private void OnRightGripClicked(object sender, ControllerInteractionEventArgs e)
    {
        rightGripDown = true;
    }

    private void OnRightGripUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        rightGripDown = false;

        if (IsDeselecting)
        {
            switch (activeState)
            {
                case InteractionState.None:
                    SetInteractionState(InteractionState.None);
                    break;

                case InteractionState.Brush:
                    BrushTriggerReleased(rightController);
                    break;

                case InteractionState.Rectangle:
                    RectangleTriggerReleased(rightController);
                    break;
            }
        }
    }

    private void BrushStart()
    {
        brushRenderer.enabled = true;
        brushRenderer.positionCount = 0;
        isBrushPastInitialDistance = false;
        isBrushComplete = false;
        isBrushLassoValid = true;
        brushRenderer.material.color = brushDrawColor;

        convexMesh.SetIncomplete();
    }

    private void BrushLoop()
    {
        if (!isBrushComplete)
        {
            Vector3 worldPoint = unimanualController.transform.position;
            // Translate this point such that it is resting on the same z plane as the screen
            Vector3 localPoint = screen.transform.InverseTransformPoint(worldPoint);
            localPoint.z = 0f;
            Vector3 point = screen.transform.TransformPoint(localPoint);

            // If the point is far away enough from the previous position
            int nbBrushPoints = brushRenderer.positionCount;
            if (nbBrushPoints == 0 || Vector3.Distance(point, brushRenderer.GetPosition(nbBrushPoints - 1)) >= brushPointDistanceInterval)
            {
                brushRenderer.positionCount = nbBrushPoints + 1;
                brushRenderer.SetPosition(nbBrushPoints, point);
                VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(unimanualController), 0.05f);

                // If there shapes between the previous point and this one, select/deselect them
                if (nbBrushPoints > 1)
                {
                    Vector3 prevPoint = brushRenderer.GetPosition(nbBrushPoints - 1);
                    Vector3 direction = Vector3.Normalize(point - prevPoint);
                    RaycastHit[] hits = Physics.RaycastAll(prevPoint, direction, Vector3.Distance(prevPoint, point));
                    foreach (RaycastHit hit in hits)
                    {
                        /* TODO
                        if (hit.collider.gameObject.tag == "Shape")
                        {
                            if (IsSelecting)
                                hit.collider.gameObject.GetComponent<InteractableShape>().ShapeSelected();
                            else if (IsDeselecting)
                                hit.collider.gameObject.GetComponent<InteractableShape>().ShapeDeselected();
                        }
                        */
                    }
                }
            }

            // If the brush has not intersected itself yet
            if (isBrushLassoValid)
            {
                // Create a mesh which shows the area that will be selected
                Vector3[] points = new Vector3[brushRenderer.positionCount];
                brushRenderer.GetPositions(points);
                Vector3[] points3d = points.Select(p => screen.transform.InverseTransformPoint(p)).ToArray();

                if (points.Length >= 3)
                {
                    convexMesh.CreateConvexMesh(points3d);

                    // If the line has intersected itself (normal points away from user), don't allow the lasso
                    if (!convexMesh.IsFacingUser())
                    {
                        isBrushLassoValid = false;
                        convexMesh.DestroyConvexMesh();
                    }
                    else
                    {
                        // If the lasso has not already been moved beyond the initial distance necessary to complete a lasso, check to see if it has
                        if (!isBrushPastInitialDistance)
                        {
                            if (Vector3.Distance(point, brushRenderer.GetPosition(0)) > brushPointInitialDistance)
                                isBrushPastInitialDistance = true;
                        }
                        else
                        {
                            // If it has already been moved past its initial distance, check to see if it has come back and completed the lasso
                            if (Vector3.Distance(point, brushRenderer.GetPosition(0)) <= brushPointCompleteDistance)
                            {
                                isBrushComplete = true;
                                brushRenderer.material.color = brushCompleteColor;
                                convexMesh.SetComplete();
                            }
                        }
                    }
                }
            }
        }
    }

    private void BrushTriggerReleased(GameObject controller)
    {
        if (controller == unimanualController)
        {
            // If the user formed a complete lasso, do a lasso selection
            if (isBrushComplete)
            {
                int nbLassoPoints = brushRenderer.positionCount;
                Vector3[] brushWorldSpace = new Vector3[nbLassoPoints];
                brushRenderer.GetPositions(brushWorldSpace);
                Vector2[] lassoLocalSpace = brushWorldSpace.Select(p => (Vector2)screen.transform.InverseTransformPoint(p)).ToArray();

                List<int> indicesToSelect = new List<int>();

                /* TODO
                foreach (GameObject shape in ScreenManager.Instance.Shapes)
                {
                    Vector2 point = screen.transform.InverseTransformPoint(shape.transform.position);
                    if (ContainsPoint(lassoLocalSpace, point))
                        indicesToSelect.Add(shape.GetComponent<InteractableShape>().Index);
                }

                if (IsSelecting)
                    ScreenManager.Instance.ShapesSelected(indicesToSelect.ToArray());
                else if (IsDeselecting)
                    ScreenManager.Instance.ShapesDeselected(indicesToSelect.ToArray());
                    */
            }

            convexMesh.DestroyConvexMesh();
            brushRenderer.enabled = false;

            SetInteractionState(InteractionState.None);
        }
    }

    private void RectangleStart()
    {
        return;
    }

    private void RectangleLoop()
    {
        if (!selectionSquare.activeSelf)
        {
            selectionSquare.SetActive(true);
        }

        Vector3 leftPos = leftController.transform.position;
        Vector3 rightPos = rightController.transform.position;

        Vector3 localStart = screen.transform.InverseTransformPoint(leftPos);
        Vector3 localEnd = screen.transform.InverseTransformPoint(rightPos);

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
        float distance = CalculateDistanceBetweenControllers();
        float vibrateStrength = rectangleSelectMaxVibrateIntensity * Mathf.Min((distance / rectangleSelectMaxVibrateDistance), 1);

        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(leftController), vibrateStrength);
        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(rightController), vibrateStrength);
    }

    private void RectangleTriggerReleased(GameObject controller)
    {
        selectionSquare.transform.parent = null;
        // Increase the area where the selection is done so that it catches shapes hovering away from the screen
        Vector3 halfExtents = selectionSquare.transform.localScale / 2;
        halfExtents.z += 0.3f;

        selectionSquare.transform.parent = screen.transform;

        Collider[] colliders = Physics.OverlapBox(selectionSquare.transform.position, halfExtents, selectionSquare.transform.rotation);
        List<int> indices = new List<int>();

        /* TODO
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Shape")
            {
                InteractableShape shapeScript = collider.gameObject.GetComponent<InteractableShape>();
                indices.Add(shapeScript.Index);
            }
        }

        if (IsSelecting)
            ScreenManager.Instance.ShapesSelected(indices.ToArray());
        else if (IsDeselecting)
            ScreenManager.Instance.ShapesDeselected(indices.ToArray());
        */
        selectionSquare.SetActive(false);
        SetInteractionState(InteractionState.None);
    }

    // Checks if a point is within a specified polygon that is defined by an array of points
    private bool ContainsPoint(Vector2[] polygon, Vector2 point)
    {
        int polygonLength = polygon.Length, i = 0;
        bool inside = false;
        // x, y for tested point.
        float pointX = point.x, pointY = point.y;
        // start / end point for the current polygon segment.
        float startX, startY, endX, endY;
        Vector2 endPoint = polygon[polygonLength - 1];
        endX = endPoint.x;
        endY = endPoint.y;
        while (i < polygonLength)
        {
            startX = endX; startY = endY;
            endPoint = polygon[i++];
            endX = endPoint.x; endY = endPoint.y;
            //
            inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                      && /* if so, test if it is under the segment */
                      ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
        }
        return inside;
    }
}
