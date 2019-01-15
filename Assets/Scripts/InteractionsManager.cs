using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class InteractionsManager : MonoBehaviour {

    public static InteractionsManager Instance { get; private set; }

    [SerializeField] [Tooltip("The game object which represents the screen.")]
    private GameObject screen;
    [SerializeField] [Tooltip("The maximum distance from the screen that is considered to the close, also known as the minimum distance from the screen that is considered to be far.")]
    public float CloseZoneThreshold = 0.75f;
    [SerializeField] [Tooltip("The maximum distance from the scene that is considered to be immediate, also known as the minimum distance from the screen that is considered to be close.")]
    public float ImmediateZoneThreshold = 0.01f;

    [SerializeField] [Tooltip("The script that manages the spin menu on a controller.")]
    private SpinMenu spinMenuScript;
    [SerializeField] [Tooltip("The script that manages the ranged interactions chosen by the spin menu.")]
    private RangedInteractions rangedInteractionsScript;
    [SerializeField] [Tooltip("The script that manages the interactions close to the wall.")]
    private CloseInteractionsManager closeInteractionsScript;

    public GameObject leftController;
    public GameObject rightController;

    private Rigidbody leftRigidbody;
    private Rigidbody rightRigidbody;

    private Zone headZone = Zone.None;
    private Zone controllerZone = Zone.None;

    private InteractionState activeState = InteractionState.None;
    private bool isGrabbingObject = false;

    private enum Zone
    {
        None,
        Far,
        Close,
        Immediate
    }

    private enum InteractionState
    {
        None,
        Grabbing,
        CloseInteracting,
        RangedInteracting,
        RangedMenuOpen
    }

    public string HeadZone
    {
        get { return headZone.ToString(); }
    }

    public string ControllerZone
    {
        get { return controllerZone.ToString(); }
    }

    public string ActiveState
    {
        get { return activeState.ToString(); }
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

    private void Start()
    {
        //leftController = VRTK_DeviceFinder.GetControllerLeftHand();
        //rightController = VRTK_DeviceFinder.GetControllerRightHand();

        leftRigidbody = leftController.GetComponent<Rigidbody>();
        rightRigidbody = rightController.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Check the current zone for the head
        float headDistance = CalculateHeadDistance();

        if (headZone != Zone.Immediate && headDistance < ImmediateZoneThreshold)
        {
            SetHeadZone(Zone.Immediate);
        }
        else if (headZone != Zone.Close && ImmediateZoneThreshold < headDistance && headDistance < CloseZoneThreshold)
        {
            SetHeadZone(Zone.Close);
        }
        else if (headZone != Zone.Far && CloseZoneThreshold < headDistance)
        {
            SetHeadZone(Zone.Far);
        }

        // If the user is not currently grabbing
        if (activeState != InteractionState.Grabbing)
        {
            // Check the current zone for the closest controller
            float controllerDistance = CalculateControllerDistance();

            if (controllerZone != Zone.Immediate && controllerDistance < ImmediateZoneThreshold)
            {
                SetControllerZone(Zone.Immediate);
            }
            else if (controllerZone != Zone.Close && ImmediateZoneThreshold < controllerDistance && controllerDistance < CloseZoneThreshold)
            {
                SetControllerZone(Zone.Close);
            }
            else if (controllerZone != Zone.Far && CloseZoneThreshold < controllerDistance)
            {
                SetControllerZone(Zone.Far);
            }
        }
    }

    private void SetHeadZone(Zone zone)
    {
        switch (zone)
        {
            case Zone.None:
                break;                

            case Zone.Immediate:
                break;

            case Zone.Close:
                break;

            case Zone.Far:
                break;
        }

        headZone = zone;
        Debug.Log("Head zone changed to " + zone.ToString());
    }

    private void SetControllerZone(Zone zone)
    {
        switch (zone)
        {
            case Zone.None:
                break;

            case Zone.Immediate:
                closeInteractionsScript.Disable();
                spinMenuScript.Disable();
                break;

            case Zone.Close:
                closeInteractionsScript.Enable();
                spinMenuScript.Disable();
                rangedInteractionsScript.Disable();
                break;

            case Zone.Far:
                closeInteractionsScript.Disable();
                spinMenuScript.Enable();
                rangedInteractionsScript.Enable();
                break;
        }

        controllerZone = zone;
        Debug.Log("Controller zone changed to " + zone.ToString());
    }

    private void SetInteractionState(InteractionState state)
    {
        switch (state)
        {
            // If there are no active interactions, let the update loop check for the ones which are enabled
            case InteractionState.None:
                controllerZone = Zone.None;
                break;
            
            case InteractionState.Grabbing:
                closeInteractionsScript.DisableAndInterrupt();
                spinMenuScript.Disable();
                rangedInteractionsScript.Disable();
                break;
            
            case InteractionState.CloseInteracting:
                spinMenuScript.Disable();
                rangedInteractionsScript.Disable();
                break;
            
            case InteractionState.RangedInteracting:
                spinMenuScript.Hide();
                break;

            case InteractionState.RangedMenuOpen:
                break;
        }

        activeState = state;
        Debug.Log("Controller interaction changed to " + state.ToString());
    }

    /// <summary>
    /// Disables all other interactions while an object is grabbed. Note that this function needs to be called by the object being grabbed or the script doing the grabbing.
    /// </summary>
    public void GrabbingStarted()
    {
        SetInteractionState(InteractionState.Grabbing);
    }

    /// <summary>
    /// Allows interactions to be enabled again after an object is ungrabbed/released. Note that this function needs to be called by the object being ungrabbed or the script doing the ungrabbing.
    /// </summary>
    public void GrabbingFinished()
    {
        SetInteractionState(InteractionState.None);
    }

    public void CloseInteractionStarted()
    {
        if (activeState == InteractionState.None)
            SetInteractionState(InteractionState.CloseInteracting);
    }

    public void CloseInteractionFinished()
    {
        if (activeState == InteractionState.CloseInteracting)
            SetInteractionState(InteractionState.None);
    }

    public void RangedInteractionStarted()
    {
        if (activeState == InteractionState.None)
            SetInteractionState(InteractionState.RangedInteracting);
    }

    public void RangedInteractionFinished()
    {
        if (activeState == InteractionState.RangedInteracting)
            SetInteractionState(InteractionState.None);
    }

    public void RangedMenuStarted()
    {
        if (activeState == InteractionState.None)
        {
            rangedInteractionsScript.Hide();
            SetInteractionState(InteractionState.RangedMenuOpen);
        }
    }

    public void RangedMenuFinished()
    {
        if (activeState == InteractionState.RangedMenuOpen)
        {
            rangedInteractionsScript.Show();
            SetInteractionState(InteractionState.None);
        }
    }

    /// <summary>
    /// Calculates the distance between the Screen and the HMD
    /// </summary>
    /// <returns>The distance between the Screen and the HMD, or infinity if the headset does not exist</returns>
    private float CalculateHeadDistance()
    {
        if (Camera.main != null)
            return Vector3.Distance(screen.GetComponent<Collider>().ClosestPointOnBounds(Camera.main.transform.position), Camera.main.transform.position);
        else
            return Mathf.Infinity;
    }

    /// <summary>
    /// Calculates the distance between the Screen and the left controller
    /// </summary>
    /// <returns>The distance between the Screen and the left controller</returns>
    private float CalculateLeftControllerDistance()
    {
        Vector3 closestPointOnScreen = screen.GetComponent<Collider>().ClosestPointOnBounds(leftController.transform.position);
        Vector3 closestPointOnController = leftRigidbody.ClosestPointOnBounds(closestPointOnScreen);

        return Vector3.Distance(closestPointOnScreen, closestPointOnController);
    }

    /// <summary>
    /// Calculates the distance between the Screen and the right controller
    /// </summary>
    /// <returns>The distance between the Screen and the right controller</returns>
    private float CalculateRightControllerDistance()
    {
        Vector3 closestPointOnScreen = screen.GetComponent<Collider>().ClosestPointOnBounds(rightController.transform.position);
        Vector3 closestPointOnController = rightRigidbody.ClosestPointOnBounds(closestPointOnScreen);

        return Vector3.Distance(closestPointOnScreen, closestPointOnController);
    }

    /// <summary>
    /// Calculates the distance between the Screen and the controllers
    /// </summary>
    /// <returns>The shortest distance between the Screen and either controller</returns>
    private float CalculateControllerDistance()
    {
        float leftDistance = CalculateLeftControllerDistance();
        float rightDistance = CalculateRightControllerDistance();

        return Mathf.Min(leftDistance, rightDistance);
    }

}
