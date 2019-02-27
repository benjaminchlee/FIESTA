using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IATK;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;
using Util;
using VRTK;
using VRTK.Controllables.PhysicsBased;

public class RangedInteractions : VRTK_StraightPointerRenderer {

    public GameObject screen;

    [Header("Sprite Parameters")]
    [SerializeField] [Tooltip("The sprite renderer that renders the sprites for the selected ranged tool.")]
    private NetworkedSprite selectedInteractionRenderer;
    [SerializeField] [Tooltip("The sprite used to represent the ranged brush tool.")]
    private Sprite rangedBrushSprite;
    [SerializeField] [Tooltip("The sprite used to represent the lasso selection tool.")]
    private Sprite lassoSelectionSprite;
    [SerializeField] [Tooltip("The sprite used to represent the rectangle selection tool.")]
    private Sprite rectangleSelectionSprite;
    [SerializeField] [Tooltip("The sprite used to represent the ranged interaction tool.")]
    private Sprite rangedInteractionSprite;
    [SerializeField] [Tooltip("The sprite used to represent the private brush tool.")]
    private Sprite privateBrushSprite;
    [SerializeField] [Tooltip("The sprite used to represent the shared brush tool.")]
    private Sprite sharedBrushSprite;

    [Header("Ranged Brush Parameters")]
    [SerializeField] [Tooltip("The prefab of the brush to use.")]
    private GameObject brushPrefab;
    [SerializeField] [Tooltip("The factor which affects the rate that the brush is resized.")]
    private float rangedBrushScaleFactor = 0.5f;
    [SerializeField] [Tooltip("The minimum size of the ranged brush.")]
    private float rangedBrushMin = 0.01f;
    [SerializeField] [Tooltip("The maximum size of the ranged brush.")]
    private float rangedBrushMax = 0.1f;

    private GameObject rangedBrush;
    private float angle;

    [Header("Lasso Selection Parameters")]
    [SerializeField] [Tooltip("The width of the line drawn while lassoing.")]
    private float lassoWidth = 0.005f;
    [SerializeField] [Tooltip("The color of the line drawn while lassoing.")]
    private Color lassoDrawColor = new Color(255, 255, 255);
    [SerializeField] [Tooltip("The color of the line drawn when the user completes the lasso.")]
    private Color lassoCompleteColor = new Color(255, 255, 0);
    [SerializeField] [Tooltip("The material of the line darwn while lassoing.")]
    private Material lassoMaterial;
    [SerializeField] [Tooltip("The initial distance the user has to move the controller before a lasso selection is formable.")]
    private float lassoPointInitialDistance = 0.05f;
    [SerializeField] [Tooltip("The distance that the user has to move the controller before another point is added to the line.")]
    private float lassoPointDistanceInterval = 0.005f;
    [SerializeField] [Tooltip("The distance from the start point that the end point has to be for it to be registered as a lasso selection.")]
    private float lassoPointCompleteDistance = 0.015f;
    [SerializeField] [Tooltip("The script used to draw convex meshes of selections.")]
    public ConvexMesh convexMesh;

    private LineRenderer lassoRenderer;
    private bool isLassoPastInitialDistance = false;
    private bool isLassoComplete = false;

    [Header("Rectangle Selection Parameters")]
    [SerializeField] [Tooltip("The width of the line drawn while selecting.")]
    private float rectangleSelectWidth = 0.01f;
    [SerializeField] [Tooltip("The material of the rectangle while selectiong.")]
    private Material rectangleSelectMaterial;

    private Vector3 rectangleStart;
    private Vector3 rectangleEnd;
    private GameObject selectionSquare;
    private Transform rectangleTransform1;
    private Transform rectangleTransform2;

    [Header("Ranged Selection Parameters")]
    [SerializeField] [Tooltip("The distance that the controller needs to be moved until an object begins being pulled from the screen.")]
    private float rangedPullStartThreshold = 0.025f;
    [SerializeField] [Tooltip("The distance that the controller needs to be moved until an object finishes being pulled from the screen.")]
    private float rangedPullCompleteThreshold = 0.2f;

    private Vector3 rangedPullControllerStartPosition;
    private Vector3 rangedPullObjectStartPosition;
    private Quaternion rangedPullObjectStartRotation;
    private GameObject rangedPullGameObject;
    private bool isPullable;
    private bool isDraggable;
    private bool pullObjectIsPrototype;

    [Header("Details on Demand Parameters")]
    [SerializeField] [Tooltip("The line renderer that acts as the leader line for details on demand.")]
    private LineRenderer leaderLineRenderer;
    
    [SerializeField] [Tooltip("The gameobject that acts as the panel for the details on demand.")]
    private GameObject detailsOnDemandGameObject;
    [SerializeField] [Tooltip("The textmesh which displays the details on demand.")]
    private TextMeshPro detailsOnDemandTextMesh;

    private bool isTouchpadDown = false;
    private Visualisation visualisationToInspect;
    private DetailsOnDemandLabel detailsOnDemandLabel;

    private VRTK_ControllerEvents controllerEvents;
    private VRTK_Pointer vrtkPointer;
    //private GameObject screen;
    //private GameObject chart;

    private InteractionState activeState = InteractionState.None;
    private InteractionState previousState = InteractionState.None;
    private InteractionState hiddenState = InteractionState.None;

    private SelectionMode selectionMode = SelectionMode.None;

    private bool isEnabled = false;
    private bool isControllerSelecting = true;
    private bool toolJustActivated = false;

    /// <summary>
    /// Brushing and linking variables
    /// </summary>
    private BrushingAndLinking brushingAndLinking;
    private Transform brushingInput1;
    private Transform brushingInput2;

    /// <summary>
    /// Networked objects
    /// </summary>
    private NetworkedTrackedObject networkedTracer;
    private NetworkedTrackedObject networkedBrush;

    /// <summary>
    /// The state of interaction the user is currently in. Note that this scope only extends to that of touchpad interactions, and not
    /// to other forms of interaction
    /// </summary>
    private enum InteractionState
    {
        None,
        RangedBrush,
        LassoSelection,
        RectangleSelection,
        RangedInteraction,
        RangedBrushing,
        LassoSelecting,
        RectangleSelecting,
        RangedInteracting,
        RangedPulling,
        DetailsOnDemand
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

    private void Start()
    {
        controllerEvents = GetComponent<VRTK_ControllerEvents>();
        controllerEvents.TouchpadPressed += OnTouchpadStart;
        controllerEvents.TouchpadAxisChanged += OnTouchpadAxisChange;
        controllerEvents.TouchpadReleased += OnTouchpadEnd;
        controllerEvents.TriggerClicked += OnTriggerStart;
        controllerEvents.TriggerUnclicked += OnTriggerEnd;
        controllerEvents.GripClicked += OnGripStart;
        controllerEvents.GripUnclicked += OnGripEnd;

        vrtkPointer = GetComponent<VRTK_Pointer>();
        
        // Instantiate rectangle selection transforms
        rectangleTransform1 = new GameObject().transform;
        rectangleTransform2 = new GameObject().transform;

        // Keep pointer on scene change
        DontDestroyOnLoad(actualCursor.transform.root.gameObject);
        DontDestroyOnLoad(actualTracer.transform.root.gameObject);

        SceneManager.sceneLoaded += InstantiatePhotonObjects;
    }

    private void InstantiatePhotonObjects(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "MainScene")
        {
            // Instantiate spin menu
            GameObject spinMenu = PhotonNetwork.Instantiate("SpinMenu", Vector3.zero, Quaternion.identity, 0);
            spinMenu.GetComponent<SpinMenu>().SpinMenuToolChanged.AddListener(InteractionToolChanged);

            // Put spin menu on other controller
            spinMenu.transform.SetParent(VRTK_DeviceFinder.IsControllerLeftHand(gameObject) ? VRTK_DeviceFinder.GetControllerRightHand().transform : VRTK_DeviceFinder.GetControllerLeftHand().transform);
            spinMenu.transform.localPosition = Vector3.zero;
            spinMenu.transform.localRotation = Quaternion.identity;

            // Instantiate sprite renderer
            GameObject spriteRenderer =  PhotonNetwork.Instantiate("InteractionModeSprite", Vector3.zero, Quaternion.identity, 0);
            selectedInteractionRenderer = spriteRenderer.GetComponent<NetworkedSprite>();
            spriteRenderer.transform.SetParent(transform);
            spriteRenderer.transform.localPosition = Vector3.up * 0.06f;
            spriteRenderer.transform.localRotation = Quaternion.identity;

            // Instantiate ranged brush
            rangedBrush = (GameObject) Instantiate(Resources.Load("BrushSphere"));
            rangedBrush.SetActive(false);

            // Create brushing and linking object
            GameObject bal = PhotonNetwork.Instantiate("BrushingAndLinking", Vector3.zero, Quaternion.identity, 0);
            brushingAndLinking = bal.GetComponent<BrushingAndLinking>();
            brushingInput1 = brushingAndLinking.input1;
            brushingInput2 = brushingAndLinking.input2;
            brushingAndLinking.NearestDistancesComputed.AddListener(DetailsOnDemandUpdated);

            // Instantiate networked tracer
            GameObject tracer = PhotonNetwork.Instantiate("NetworkedTracer", Vector3.zero, Quaternion.identity, 0);
            networkedTracer = tracer.GetComponent<NetworkedTrackedObject>();
            networkedTracer.SetTrackedObject(actualTracer);
            networkedTracer.SetColor(PlayerPreferencesManager.Instance.AvatarColor);
            // Hide the mesh for the owner (this does not get propagated)
            networkedTracer.GetComponent<Renderer>().enabled = false;

            // Instantiate networked brush
            GameObject brush = PhotonNetwork.Instantiate("NetworkedBrush", Vector3.zero, Quaternion.identity, 0);
            networkedBrush = brush.GetComponent<NetworkedTrackedObject>();
            networkedBrush.SetTrackedObject(rangedBrush);
            networkedBrush.SetColor(PlayerPreferencesManager.Instance.AvatarColor);
            // Hide the mesh for the owner (this does not get propagated)
            networkedBrush.GetComponent<Renderer>().enabled = false;
        }
    }

    public void Enable()
    {
        isEnabled = true;
    }

    public void Disable()
    {
        isEnabled = false;

        if (!IsInteracting())
        {
            SetInteractionState(InteractionState.None);
        }
    }

    public void Hide()
    {
        hiddenState = activeState;
        SetInteractionState(InteractionState.None);
    }

    public void Show()
    {
        SetInteractionState(hiddenState);
        hiddenState = InteractionState.None;
    }

    /// <summary>
    /// Changes the interaction tool that is used on the controller. This is designed to be called by functions outside of this script.
    /// </summary>
    /// <param name="interactionType"></param>
    public void InteractionToolChanged(string interactionType)
    {
        // Only change the tool that is used if the active state is a default one
        if (!IsInteracting())
        {
            switch (interactionType.ToLower())
            {
                case "none":
                    SetInteractionState(InteractionState.None);
                    break;

                case "rangedbrush":
                    toolJustActivated = true;
                    SetInteractionState(InteractionState.RangedBrush);
                    break;

                case "lassoselection":
                    toolJustActivated = true;
                    SetInteractionState(InteractionState.LassoSelection);
                    break;

                case "rectangleselection":
                    toolJustActivated = true;
                    SetInteractionState(InteractionState.RectangleSelection);
                    break;

                case "rangedinteraction":
                    toolJustActivated = true;
                    SetInteractionState(InteractionState.RangedInteraction);
                    break;

                case "privaterangedbrush":
                    toolJustActivated = true;
                    brushingAndLinking.shareBrushing = false;
                    SetInteractionState(InteractionState.RangedBrush);
                    break;

                case "sharedrangedbrush":
                    toolJustActivated = true;
                    brushingAndLinking.shareBrushing = true;
                    SetInteractionState(InteractionState.RangedBrush);
                    break;

                case "detailsondemand":
                    break;
            }

            TriggerControllerVibration(0.3f);
        }
        //// Otherwise this would've interrupted a user's active interaction, therefore vibrate hard to warn them of this
        //else
        //{
        //    VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(gameObject), 0.75f, 0.05f, 0.005f);
        //}
    }

    /// <summary>
    /// This method changes the InteractionState to the one specified. Note that this does not check for any pre-conditions before switching the state
    /// and should only be called where allowed.
    /// </summary>
    /// <param name="state">The InteractionState to change to</param>
    private void SetInteractionState(InteractionState state)
    {
        // If ranged interactions are disabled, don't allow any interaction changes (mainly when finishing an existing interaction)
        if (!isEnabled)
            state = InteractionState.None;

        switch (state)
        {
            case InteractionState.None:
                previousState = InteractionState.None;
                selectedInteractionRenderer.Sprite = null;
                tracerVisibility = VisibilityStates.AlwaysOff;
                selectionMode = SelectionMode.None;
                break;

            case InteractionState.RangedBrush:
                // TODO: make a bit cleaner
                if (brushingAndLinking.shareBrushing)
                    selectedInteractionRenderer.Sprite = sharedBrushSprite;
                else
                    selectedInteractionRenderer.Sprite = privateBrushSprite;
                tracerVisibility = VisibilityStates.AlwaysOn;
                break;

            case InteractionState.RangedBrushing:
                break;

            case InteractionState.LassoSelection:
                selectedInteractionRenderer.Sprite = lassoSelectionSprite;
                tracerVisibility = VisibilityStates.AlwaysOn;
                break;

            case InteractionState.LassoSelecting:
                break;

            case InteractionState.RectangleSelection:
                selectedInteractionRenderer.Sprite = rectangleSelectionSprite;
                tracerVisibility = VisibilityStates.AlwaysOn;
                break;

            case InteractionState.RectangleSelecting:
                break;

            case InteractionState.RangedInteraction:
                selectedInteractionRenderer.Sprite = rangedInteractionSprite;
                tracerVisibility = VisibilityStates.AlwaysOn;
                break;

            case InteractionState.RangedInteracting:
                break;

            case InteractionState.RangedPulling:
                break;
        }

        activeState = state;
        Debug.Log("Ranged interaction state changed to " + state.ToString());
    }

    /// <summary>
    /// Checks to see if there is an interaction tool that is selected.
    /// </summary>
    /// <returns>True if there is currently an interaction tool selected, otherwise returns false</returns>
    private bool IsInteractionToolActive()
    {
        return (activeState != InteractionState.None);
    }

    /// <summary>
    /// Checks to see if there is an ongoing interaction. Note that this is specifically for when the user is performing an interaction, not when they only have it selected.
    /// </summary>
    /// <returns>True if the user is currently performing an interaction, otherwise returns false</returns>
    private bool IsInteracting()
    {
        return new string[] { "rangedbrushing", "lassoselecting", "rectangleselecting", "rangedinteracting", "rangedpulling", "detailsondemand" }.Contains(activeState.ToString().ToLower());
    }

    /// <summary>
    /// Called when the user presses the trigger enough until it clicks. This will call the respective function depending on which
    /// interaction tool was originally selected by the user.
    /// </summary>
    private void OnTriggerStart(object sender, ControllerInteractionEventArgs e)
    {
        if (IsInteractionToolActive() && !IsInteracting() && !toolJustActivated)
        {
            selectionMode = SelectionMode.Selecting;

            InteractionsManager.Instance.RangedInteractionStarted();

            RaycastHit pointerCollidedWith;
            GameObject collidedObject = GetCollidedObject(out pointerCollidedWith);

            if (collidedObject != null && (collidedObject.CompareTag("MenuButton") || collidedObject.CompareTag("PhysicsMenuButton") || collidedObject.CompareTag("SPLOMButton")))
            {
                previousState = activeState;
                RangedInteractionTriggerStart(e);
            }
            else
            {
                switch (activeState)
                {
                    case InteractionState.RangedBrush:
                        RangedBrushTriggerStart(e);
                        break;

                    case InteractionState.LassoSelection:
                        LassoSelectionTriggerStart(e);
                        break;

                    case InteractionState.RectangleSelection:
                        RectangleSelectionTriggerStart(e);
                        break;

                    case InteractionState.RangedInteraction:
                        RangedInteractionTriggerStart(e);
                        break;
                }
            }
        }

        toolJustActivated = false;
    }

    private void OnTriggerEnd(object sender, ControllerInteractionEventArgs e)
    {
        if (IsInteractionToolActive() && IsSelecting)
        {
            InteractionsManager.Instance.RangedInteractionFinished();

            switch (activeState)
            {
                case InteractionState.RangedBrushing:
                    RangedBrushTriggerEnd(e);
                    break;

                case InteractionState.LassoSelecting:
                    LassoSelectionTriggerEnd(e);
                    break;

                case InteractionState.RectangleSelecting:
                    RectangleSelectionTriggerEnd(e);
                    break;

                case InteractionState.RangedInteracting:
                    RangedInteractionTriggerEnd(e);
                    break;

                case InteractionState.RangedPulling:
                    RangedPullTriggerEnd(e);
                    break;
            }
        }
    }

    private void OnGripStart(object sender, ControllerInteractionEventArgs e)
    {
        if (IsInteractionToolActive() && !IsInteracting())
        {
            selectionMode = SelectionMode.Deselecting;

            InteractionsManager.Instance.RangedInteractionStarted();
            
            switch (activeState)
            {
                case InteractionState.RangedBrush:
                    RangedBrushTriggerStart(e);
                    break;

                case InteractionState.LassoSelection:
                    LassoSelectionTriggerStart(e);
                    break;

                case InteractionState.RectangleSelection:
                    RectangleSelectionTriggerStart(e);
                    break;
            }
        }
    }

    private void OnGripEnd(object sender, ControllerInteractionEventArgs e)
    {
        if (IsInteractionToolActive() && IsDeselecting)
        {
            InteractionsManager.Instance.RangedInteractionFinished();

            switch (activeState)
            {
                case InteractionState.RangedBrushing:
                    RangedBrushTriggerEnd(e);
                    break;

                case InteractionState.LassoSelecting:
                    LassoSelectionTriggerEnd(e);
                    break;

                case InteractionState.RectangleSelecting:
                    RectangleSelectionTriggerEnd(e);
                    break;

                case InteractionState.RangedInteracting:
                    RangedInteractionTriggerEnd(e);
                    break;

                case InteractionState.RangedPulling:
                    RangedPullTriggerEnd(e);
                    break;
            }
        }
    }

    private void OnTouchpadStart(object sender, ControllerInteractionEventArgs e)
    {
        angle = e.touchpadAngle;

        isTouchpadDown = true;
    }

    private void OnTouchpadAxisChange(object sender, ControllerInteractionEventArgs e)
    {
        if (activeState == InteractionState.RangedBrushing)
        {
            float delta = e.touchpadAngle - angle;
            // Instance where touch crosses over from 0 to 359 degrees
            if (delta >= 180)
                delta = -(360 - delta);
            // Instance where touch crosses over from 359 to 0 degrees
            else if (delta < -180)
                delta = 360 + delta;

            float newCursorScaleMultiplier = cursorScaleMultiplier + delta * rangedBrushScaleFactor;
            Vector3 currentScale = rangedBrush.transform.localScale;
            rangedBrush.transform.localScale = Vector3.one * (scaleFactor * newCursorScaleMultiplier);

            float newSize = rangedBrush.transform.localScale.x;

            if (newSize >= rangedBrushMin && newSize <= rangedBrushMax)
                cursorScaleMultiplier = newCursorScaleMultiplier;
            else
                rangedBrush.transform.localScale = currentScale;

            angle = e.touchpadAngle;
        }
    }

    private void OnTouchpadEnd(object sender, ControllerInteractionEventArgs e)
    {
        isTouchpadDown = false;
    }

    private void Update()
    {
        if (networkedTracer != null)
        {
            if (IsTracerVisible())
            {
                // Set the size of the tracer accordingly
                networkedTracer.transform.position = actualTracer.transform.position;
                networkedTracer.transform.rotation = actualTracer.transform.rotation;
                networkedTracer.transform.localScale = actualTracer.transform.localScale;
            }
            else
            {
                // Hide the networked tracer by scaling it to zero
                networkedTracer.transform.localScale = Vector3.zero;
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        switch (activeState)
        {
            case InteractionState.RangedBrushing:
                RangedBrushLoop();
                break;

            case InteractionState.LassoSelecting:
                LassoSelectionLoop();
                break;

            case InteractionState.RectangleSelecting:
                RectangleSelectionLoop();
                break;

            case InteractionState.RangedInteracting:
                RangedInteractionLoop();
                break;

            case InteractionState.RangedPulling:
                RangedPullLoop();
                break;
        }

        //Details on demand
        if (brushingAndLinking != null)
        {
            if (IsInteractionToolActive() && isTouchpadDown)
            {
                RaycastHit hit;
                GameObject collidedObject = GetCollidedObject(out hit);

                if (collidedObject != null && collidedObject.CompareTag("ChartRaycastCollider"))
                {
                    brushingInput1.position = hit.point;
                    brushingAndLinking.inspectButtonController = true;
                    visualisationToInspect = collidedObject.GetComponentInParent<Chart>().Visualisation;
                    brushingAndLinking.visualisationToInspect = visualisationToInspect;
                    brushingAndLinking.radiusInspector = 0.2f;
                }
            }
            else if (!isTouchpadDown)
            {
                brushingAndLinking.visualisationToInspect = null;
                brushingAndLinking.inspectButtonController = false;
                leaderLineRenderer.enabled = false;
                if (detailsOnDemandLabel != null)
                    detailsOnDemandLabel.gameObject.SetActive(false);
            }
        }
    }

    private void RangedBrushTriggerStart(ControllerInteractionEventArgs e)
    {
        SetInteractionState(InteractionState.RangedBrushing);

        brushingInput1.position = rangedBrush.transform.position;

        if (IsSelecting)
            brushingAndLinking.SELECTION_TYPE = BrushingAndLinking.SelectionType.ADDITIVE;
        else if (IsDeselecting)
            brushingAndLinking.SELECTION_TYPE = BrushingAndLinking.SelectionType.SUBTRACTIVE;
    }

    private void RangedBrushLoop()
    {
        brushingAndLinking.radiusSphere = rangedBrush.transform.lossyScale.x / 2;

        RaycastHit hit;
        GameObject collidedObject = GetCollidedObject(out hit);
        if (collidedObject != null)
        {
            if (!rangedBrush.activeSelf)
                rangedBrush.SetActive(true);

            rangedBrush.transform.position = hit.point;
            brushingInput1.position = hit.point;
            brushingAndLinking.brushButtonController = true;

            TriggerControllerVibration(0.05f);
        }
        else
        {
            rangedBrush.SetActive(false);
            brushingAndLinking.brushButtonController = false;
        }

        if (IsValidCollision())
            rangedBrush.GetComponent<Renderer>().material.color = validCollisionColor;
        else
            rangedBrush.GetComponent<Renderer>().material.color = invalidCollisionColor;
    }

    private void RangedBrushTriggerEnd(ControllerInteractionEventArgs e)
    {
        SetInteractionState(InteractionState.RangedBrush);

        brushingAndLinking.brushButtonController = false;

        rangedBrush.SetActive(false);
    }

    private void LassoSelectionTriggerStart(ControllerInteractionEventArgs e)
    {
        SetInteractionState(InteractionState.LassoSelecting);

        lassoRenderer = gameObject.AddComponent<LineRenderer>();
        lassoRenderer.useWorldSpace = true;
        // Remove any anomalous default points
        lassoRenderer.positionCount = 0;
        lassoRenderer.startWidth = lassoWidth;
        lassoRenderer.endWidth = lassoWidth;
        lassoRenderer.material = lassoMaterial;
        lassoRenderer.material.color = lassoDrawColor;
        isLassoPastInitialDistance = false;
        isLassoComplete = false;

        convexMesh.SetIncomplete();
    }

    private void LassoSelectionLoop()
    {
        if (!isLassoComplete)
        {
            RaycastHit pointerCollidedWith;
            GameObject collidedObject = GetCollidedObject(out pointerCollidedWith);

            if (collidedObject != null)
            {
                if (collidedObject == screen || collidedObject.CompareTag("DisplayScreen"))
                {
                    int nbLassoPoints = lassoRenderer.positionCount;

                    // If the point is far away enough from the previous position
                    if (nbLassoPoints == 0 || Vector3.Distance(pointerCollidedWith.point, lassoRenderer.GetPosition(nbLassoPoints - 1)) >= lassoPointDistanceInterval)
                    {
                        lassoRenderer.positionCount = nbLassoPoints + 1;
                        lassoRenderer.SetPosition(nbLassoPoints, pointerCollidedWith.point);
                        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(gameObject), 0.05f);

                        // Update the mesh which shows the area that will be selected
                        Vector3[] points = new Vector3[lassoRenderer.positionCount];
                        lassoRenderer.GetPositions(points);
                        Vector3[] points3d = points.Select(p => screen.transform.InverseTransformPoint(p)).ToArray();
                        convexMesh.CreateConvexMesh(points3d);
                    }

                    // If the lasso has not already been moved beyond the initial distance necessary to complete a lasso, check to see if it has
                    if (!isLassoPastInitialDistance)
                    {
                        if (Vector3.Distance(pointerCollidedWith.point, lassoRenderer.GetPosition(0)) > lassoPointInitialDistance)
                            isLassoPastInitialDistance = true;
                    }
                    else
                    {
                        // If it has already been moved past its initial distance, check to see if it has come back and completed the lasso
                        if (Vector3.Distance(pointerCollidedWith.point, lassoRenderer.GetPosition(0)) <= lassoPointCompleteDistance)
                        {
                            isLassoComplete = true;
                            lassoRenderer.material.color = lassoCompleteColor;
                            convexMesh.SetComplete();
                        }
                    }
                }
            }
        }
    }

    private void LassoSelectionTriggerEnd(ControllerInteractionEventArgs e)
    {
        SetInteractionState(InteractionState.LassoSelection);

        if (isLassoComplete)
        {
            /* TODO
            List<int> indicesToSelect = new List<int>();

            int nbLassoPoints = lassoRenderer.positionCount;
            Vector3[] lassoWorldSpace = new Vector3[nbLassoPoints];
            lassoRenderer.GetPositions(lassoWorldSpace);
            Vector2[] lassoLocalSpace = lassoWorldSpace.Select(p => (Vector2)screen.transform.InverseTransformPoint(p)).ToArray();

            foreach (GameObject shape in ScreenManager.Instance.Shapes)
            {
                Vector2 point = screen.transform.InverseTransformPoint(shape.transform.position);
                if (ContainsPoint(lassoLocalSpace, point))
                {
                    indicesToSelect.Add(shape.GetComponent<InteractableShape>().Index);
                }

            }

            // Select any shapes which the line is touching
            foreach (Vector3 point in lassoWorldSpace)
            {
                Collider[] colliders = Physics.OverlapSphere(point, 0.01f);

                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.tag == "Shape")
                    {
                        indicesToSelect.Add(collider.gameObject.GetComponent<InteractableShape>().Index);
                    }
                }
            }

            if (IsSelecting)
                ScreenManager.Instance.ShapesSelected(indicesToSelect.ToArray());
            else if (IsDeselecting)
                ScreenManager.Instance.ShapesDeselected(indicesToSelect.ToArray());
            */
        }

        convexMesh.DestroyConvexMesh();
        Destroy(lassoRenderer);
    }

    private void RectangleSelectionTriggerStart(ControllerInteractionEventArgs e)
    {
        RaycastHit pointerCollidedWith;
        GameObject collidedObject = GetCollidedObject(out pointerCollidedWith);

        if (collidedObject != null)
        {
            SetInteractionState(InteractionState.RectangleSelecting);
            rectangleStart = pointerCollidedWith.point;
            //if (collidedObject == screen || collidedObject.CompareTag("Shape"))
            //{
            //    SetInteractionState(InteractionState.RectangleSelecting);
            //    rectangleStart = pointerCollidedWith.point;
            //}
        }
    }

    private void RectangleSelectionLoop()
    {
        RaycastHit pointerCollidedWith;
        GameObject collidedObject = GetCollidedObject(out pointerCollidedWith);

        if (collidedObject != null && collidedObject.transform.parent != null && collidedObject.transform.parent.CompareTag("Chart"))
        {
            /*
            // Only draw the shape if the pointer is targeting at the screen
            if (collidedObject == screen)
            {
                // Create the square to be used for the selection if it does not already exist
                if (selectionSquare == null)
                {
                    selectionSquare = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // Set it to ignore raycasts
                    selectionSquare.layer = 2;
                    selectionSquare.GetComponent<Renderer>().material = rectangleSelectMaterial;
                    selectionSquare.transform.SetParent(screen.transform);
                    LineRenderer lr = selectionSquare.AddComponent<LineRenderer>();
                    lr.positionCount = 5;
                    lr.SetPositions(new Vector3[] { new Vector3(0.5f, 0.5f, 0), new Vector3(-0.5f, 0.5f, 0), new Vector3(-0.5f, -0.5f, 0), new Vector3(0.5f, -0.5f, 0), new Vector3(0.5f, 0.5f, 0) });
                    lr.material = rectangleSelectMaterial;
                    lr.startWidth = rectangleSelectWidth;
                    lr.endWidth = rectangleSelectWidth;
                    lr.useWorldSpace = false;
                }

                rectangleEnd = pointerCollidedWith.point;

                Vector3 localStart = screen.transform.InverseTransformPoint(rectangleStart);
                Vector3 localEnd = screen.transform.InverseTransformPoint(rectangleEnd);

                Vector3 localScale = localStart - localEnd;
                localScale.x = Mathf.Abs(localScale.x);
                localScale.y = Mathf.Abs(localScale.y);
                localScale.z = Mathf.Abs(localScale.z) + 0.02f;  // Add a bit extra thickness to prevent Z-fighting
                Vector3 localCenter = (localStart + localEnd) / 2;

                selectionSquare.transform.localPosition = localCenter;
                selectionSquare.transform.localScale = localScale;
                selectionSquare.transform.rotation = screen.transform.rotation;
            }
            */

            // Create the square to be used for the selection if it does not already exist
            if (selectionSquare == null)
            {
                selectionSquare = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // Set it to ignore raycasts
                selectionSquare.layer = 2;
                selectionSquare.GetComponent<Renderer>().material = rectangleSelectMaterial;
                //LineRenderer lr = selectionSquare.AddComponent<LineRenderer>();
                //lr.positionCount = 5;
                //lr.SetPositions(new Vector3[] { new Vector3(0.5f, 0.5f, 0), new Vector3(-0.5f, 0.5f, 0), new Vector3(-0.5f, -0.5f, 0), new Vector3(0.5f, -0.5f, 0), new Vector3(0.5f, 0.5f, 0) });
                //lr.material = rectangleSelectMaterial;
                //lr.startWidth = rectangleSelectWidth;
                //lr.endWidth = rectangleSelectWidth;
                //lr.useWorldSpace = false;
            }

            rectangleEnd = pointerCollidedWith.point;

            Vector3 scale = rectangleEnd - rectangleStart;
            scale.x = Mathf.Abs(scale.x);
            scale.y = Mathf.Abs(scale.y);
            scale.z = Mathf.Abs(scale.z);

            Vector3 center = (rectangleStart + rectangleEnd) / 2;

            selectionSquare.transform.position = center;
            selectionSquare.transform.localScale = scale;
            selectionSquare.transform.rotation = collidedObject.transform.rotation;
            selectionSquare.transform.Rotate(Vector3.up, 90f);
        }
    }

    private void RectangleSelectionTriggerEnd(ControllerInteractionEventArgs e)
    {
        SetInteractionState(InteractionState.RectangleSelection);
        
        brushingAndLinking.input1 = rectangleTransform1;
        brushingAndLinking.input2 = rectangleTransform2;
        rectangleTransform1.position = rectangleStart;
        rectangleTransform2.position = rectangleEnd;

        brushingAndLinking.brushButtonController = true;

        StartCoroutine(StopRectangleSelection());
    }

    private IEnumerator StopRectangleSelection()
    {
        yield return null;

        brushingAndLinking.brushButtonController = false;
        Destroy(selectionSquare);
    }

    private void RangedInteractionTriggerStart(ControllerInteractionEventArgs e)
    {
        GameObject collidedObject = GetCollidedObject();
        if (collidedObject != null)
        {
            // If the object that is being pointed at is a chart, it is pullable
            if (collidedObject.transform.parent != null && collidedObject.CompareTag("ChartRaycastCollider") && collidedObject.transform.parent.CompareTag("Chart"))
            {
                isPullable = true;
                rangedPullGameObject = collidedObject.transform.parent.gameObject;
                rangedPullControllerStartPosition = transform.position;
                rangedPullObjectStartPosition = rangedPullGameObject.transform.position;
                rangedPullObjectStartRotation = rangedPullGameObject.transform.rotation;
                SetInteractionState(InteractionState.RangedInteracting);

                pullObjectIsPrototype = rangedPullGameObject.GetComponent<Chart>().IsPrototype;
            }
            // Otherwise, if it is a menu button, click it
            else if (collidedObject.CompareTag("MenuButton"))
            {
                if (collidedObject.GetComponent<MenuButton>() != null)
                {
                    collidedObject.GetComponent<MenuButton>().Click();
                }
                else if (collidedObject.GetComponent<GradientButton>() != null)
                {
                    collidedObject.GetComponent<GradientButton>().Click();
                }
                else if (collidedObject.GetComponent<ColorPaletteBinderButton>() != null)
                {
                    collidedObject.GetComponent<ColorPaletteBinderButton>().Click();
                }

                TriggerControllerVibration(0.3f);
            }
            else if (collidedObject.CompareTag("SPLOMButton"))
            {
                collidedObject.GetComponent<SPLOMButton>().Click(); ;

                TriggerControllerVibration(0.3f);
            }
            // Otherwise, if it is a physics based menu button, start to drag it
            else if (collidedObject.CompareTag("PhysicsMenuButton"))
            {
                isDraggable = true;
                rangedPullGameObject = collidedObject;

                // As we will be sending updates on it, we need to take ownership of it
                if (!rangedPullGameObject.GetComponent<PhotonView>().isMine)
                    rangedPullGameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player);

                rangedPullGameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

                SetInteractionState(InteractionState.RangedInteracting);
            }
        }
    }

    private void RangedInteractionLoop()
    {
        if (isPullable)
        {
            float distance = Vector3.Distance(rangedPullControllerStartPosition, transform.position);

            // Vibrate the controller based on how far away it is from the origin
            float vibrateAmount = 0.75f * (distance / rangedPullCompleteThreshold);
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(gameObject), vibrateAmount);

            if (distance > rangedPullStartThreshold)
            {
                isPullable = false;
                SetInteractionState(InteractionState.RangedPulling);

                // If the object is a chart and it is a prototype, create a duplicate and use that instead
                if (pullObjectIsPrototype)
                {
                    Chart chart = ChartManager.Instance.DuplicateVisualisation(rangedPullGameObject.GetComponent<Chart>());
                    rangedPullGameObject = chart.gameObject;
                }
            }
        }

        if (isDraggable)
        {
            RaycastHit hit = GetDestinationHit();
            rangedPullGameObject.GetComponent<Rigidbody>().MovePosition(hit.point);

            TriggerControllerVibration(0.1f);
        }
    }

    private void RangedInteractionTriggerEnd(ControllerInteractionEventArgs e)
    {
        if (isDraggable && rangedPullGameObject != null)
        {
            rangedPullGameObject.layer = LayerMask.NameToLayer("Default");
            rangedPullGameObject = null;
        }

        if (previousState != InteractionState.None)
        {
            SetInteractionState(previousState);
            previousState = InteractionState.None;
        }
        else
        {
            SetInteractionState(InteractionState.RangedInteraction);
        }

        isPullable = false;
        isDraggable = false;
    }

    private void RangedPullLoop()
    {
        float distance = Vector3.Distance(rangedPullControllerStartPosition, transform.position);

        // Vibrate the controller based on how far away it is from the origin
        float vibrateAmount = 0.75f * (distance / rangedPullCompleteThreshold);
        TriggerControllerVibration(vibrateAmount);

        // If the object has been pulled sufficiently far, grab it
        if (distance > rangedPullCompleteThreshold)
        {
            SetInteractionState(InteractionState.None);

            rangedPullGameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            rangedPullGameObject.transform.position = transform.position;

            GetComponent<VRTK_InteractTouch>().ForceTouch(rangedPullGameObject);
            GetComponent<VRTK_InteractGrab>().AttemptGrab();
        }
        else
        {
            rangedPullGameObject.transform.position = Vector3.Lerp(rangedPullObjectStartPosition, transform.position, distance / rangedPullCompleteThreshold);

            // Lock LookAt rotation to only rotate along the y axis
            //Vector3 targetPosition = new Vector3(Camera.main.transform.position.x, rangedPullGameObject.transform.position.y, Camera.main.transform.position.z);
            //rangedPullGameObject.transform.LookAt(targetPosition);
            rangedPullGameObject.transform.rotation = Quaternion.LookRotation(transform.position - VRTK_DeviceFinder.HeadsetTransform().position);
        }
    }

    private void RangedPullTriggerEnd(ControllerInteractionEventArgs e)
    {
        if (previousState != InteractionState.None)
        {
            SetInteractionState(previousState);
            previousState = InteractionState.None;
        }
        else
        {
            SetInteractionState(InteractionState.RangedInteraction);
        }

        rangedPullGameObject.GetComponent<Chart>().AnimateTowards(rangedPullObjectStartPosition, rangedPullObjectStartRotation, 0.1f, pullObjectIsPrototype);
    }
    
    private void DetailsOnDemandUpdated(List<float> nearestDistances)
    {
        if (IsInteractionToolActive())
        {
            // Get list of indices that share the closest distance
            List<int> nearestIndices = new List<int>();
            float minDistance = 100;
            
            for (int i = 0; i < nearestDistances.Count; i++)
            {
                if (nearestDistances[i] < minDistance)
                {
                    nearestIndices.Clear();
                    nearestIndices.Add(i);
                    minDistance = nearestDistances[i];
                }
                else if (IsFloatEqual(nearestDistances[i], minDistance, 0.0001f))
                {
                    nearestIndices.Add(i);
                }
            }

            if (nearestIndices.Count > 0)
            {
                // Get position of the original point
                Vector3 originalPos = visualisationToInspect.theVisualizationObject.viewList[0].BigMesh.getBigMeshVertices()[nearestIndices[0]];

                // Normalise it based on visualisation properties
                float minNormX = visualisationToInspect.xDimension.minScale;
                float maxNormX = visualisationToInspect.xDimension.maxScale;
                float minNormY = visualisationToInspect.yDimension.minScale;
                float maxNormY = visualisationToInspect.yDimension.maxScale;
                float minNormZ = visualisationToInspect.zDimension.minScale;
                float maxNormZ = visualisationToInspect.zDimension.maxScale;
                float width = visualisationToInspect.width;
                float height = visualisationToInspect.height;
                float depth = visualisationToInspect.depth;

                float normX = NormaliseValue(originalPos.x, minNormX, maxNormX, 0, width);
                float normY = NormaliseValue(originalPos.y, minNormY, maxNormY, 0, height);
                float normZ = NormaliseValue(originalPos.z, minNormZ, maxNormZ, 0, depth);
                Vector3 normalisedPos = new Vector3(normX, normY, normZ);

                // Convert to world space
                Vector3 worldPos = visualisationToInspect.transform.TransformPoint(normalisedPos);

                // Draw leader line from laser hit position to point
                Vector3 hitPos = GetDestinationHit().point;
                leaderLineRenderer.enabled = true;
                leaderLineRenderer.SetPositions(new[] { hitPos, worldPos });

                // Draw label at the point
                if (detailsOnDemandLabel == null)
                {
                    detailsOnDemandLabel = ((GameObject)Instantiate(Resources.Load("DetailsOnDemandLabel"))).GetComponent<DetailsOnDemandLabel>();
                }

                detailsOnDemandLabel.gameObject.SetActive(true);
                detailsOnDemandLabel.SetText(nearestIndices, visualisationToInspect);
                detailsOnDemandLabel.transform.position = hitPos;
                detailsOnDemandLabel.transform.rotation = visualisationToInspect.transform.rotation;
            }
            else
            {
                leaderLineRenderer.enabled = false;
                if (detailsOnDemandLabel != null)
                    detailsOnDemandLabel.gameObject.SetActive(false);
            }
        }
    }

    private float NormaliseValue(float value, float min1, float max1, float min2, float max2)
    {
        float i = (min2 - max2) / (min1 - max1);
        return (min2 - (i * min1) + (i * value));
    }

    private bool IsFloatEqual(float value1, float value2, float tolerance)
    {
        return (Mathf.Abs(value1 - value2) < tolerance);
    }

    // Override the color of the laser such that it is still invalid when hitting the just the screen itself
    protected override void CheckRayHit(bool rayHit, RaycastHit pointerCollidedWith)
    {
        base.CheckRayHit(rayHit, pointerCollidedWith);

        if (rayHit && (pointerCollidedWith.collider.gameObject == screen))
        {
            ChangeColor(invalidCollisionColor);
        }
    }

    private GameObject GetCollidedObject()
    {
        RaycastHit tmp;
        return GetCollidedObject(out tmp);
    }

    private GameObject GetCollidedObject(out RaycastHit pointerCollidedWith)
    {
        Transform origin = GetOrigin();
        Ray pointerRaycast = new Ray(origin.position, origin.forward);
        bool rayHit = VRTK_CustomRaycast.Raycast(customRaycast, pointerRaycast, out pointerCollidedWith, defaultIgnoreLayer, maximumLength);

        if (pointerCollidedWith.collider != null)
            return pointerCollidedWith.collider.gameObject;
        else
            return null;
    }

    // Checks if a point is within a specified polygon that is defined by an array of points
    public bool ContainsPoint(Vector2[] polygon, Vector2 point)
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

    private void TriggerControllerVibration(float strength)
    {
        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(gameObject), strength);
    }
}
