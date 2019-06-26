using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class InteractionsController : MonoBehaviour {

    [SerializeField]
    private VRTK_InteractGrab leftInteractGrab;
    [SerializeField]
    private VRTK_InteractGrab rightInteractGrab;
    [SerializeField]
    private RangedInteractions rangedInteractions;

    private SpinMenu spinMenu;

    private InteractionState activeState = InteractionState.None;

    private enum InteractionState
    {
        None,
        Grabbing,
        CloseInteracting,
        RangedInteracting,
        RangedMenuOpen
    }

    private bool isMainSceneLoaded = false;

    private void Awake()
    {
        if (leftInteractGrab == null) leftInteractGrab = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_InteractGrab>();
        if (rightInteractGrab == null) rightInteractGrab = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_InteractGrab>();
        if (rangedInteractions == null) rangedInteractions = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<RangedInteractions>();
    }

    private void Start()
    {
        leftInteractGrab.ControllerStartGrabInteractableObject += StartGrabObject;
        rightInteractGrab.ControllerStartGrabInteractableObject += StartGrabObject;

        leftInteractGrab.ControllerUngrabInteractableObject += StopGrabObject;
        rightInteractGrab.ControllerUngrabInteractableObject += StopGrabObject;

        rangedInteractions.RangedToolActivated.AddListener(StartRangedInteraction);
        rangedInteractions.RangedToolDeactivated.AddListener(StopRangedInteraction);

        DontDestroyOnLoad(transform.root.gameObject);
        SceneManager.sceneLoaded += SceneChanged;
    }

    private void SceneChanged(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "MainScene")
        {
            isMainSceneLoaded = true;
            Invoke("GetScriptReferences", 0.1f);
        }
    }

    /// <summary>
    /// This gets the script references that are needed, which are instantiated by different scripts upon entering the main scene.
    ///
    /// There's probably a better way to do this but it works for now. 
    /// </summary>
    private void GetScriptReferences()
    {
        if (spinMenu == null)
        {
            foreach (var sm in FindObjectsOfType<SpinMenu>())
            {
                if (sm.photonView.IsMine)
                    spinMenu = sm;
            }
        }

        SetInteractionState(InteractionState.None);
    }

    private void SetInteractionState(InteractionState state)
    {
        switch (state)
        {
            case InteractionState.None:
                spinMenu.Show();
                rangedInteractions.Enable();
                break;

            case InteractionState.Grabbing:
                //closeInteractionsScript.DisableAndInterrupt();
                spinMenu.Disable();
                rangedInteractions.Disable();
                break;

            case InteractionState.CloseInteracting:
                spinMenu.Disable();
                rangedInteractions.Disable();
                break;

            case InteractionState.RangedInteracting:
                spinMenu.Hide();
                break;

            case InteractionState.RangedMenuOpen:
                break;
        }

        activeState = state;
        Debug.Log("Controller interaction changed to " + state.ToString());
    }

    private void StartGrabObject(object sender, ObjectInteractEventArgs e)
    {
        if (isMainSceneLoaded)
            SetInteractionState(InteractionState.Grabbing);
    }

    private void StopGrabObject(object sender, ObjectInteractEventArgs e)
    {
        if (isMainSceneLoaded)
        {
            // Double check that the other controller is not grabbing anything
            if (e.controllerReference.hand == SDK_BaseController.ControllerHand.Left)
            {
                if (rightInteractGrab.GetGrabbedObject() == null)
                    SetInteractionState(InteractionState.None);
            }
            else if (e.controllerReference.hand == SDK_BaseController.ControllerHand.Right)
            {
                if (leftInteractGrab.GetGrabbedObject() == null)
                    SetInteractionState(InteractionState.None);
            }
        }
    }

    private void StartRangedInteraction()
    {
        if (isMainSceneLoaded)
        {
            if (activeState == InteractionState.None)
                SetInteractionState(InteractionState.RangedInteracting);
        }
    }

    private void StopRangedInteraction()
    {
        if (isMainSceneLoaded)
        {
            if (activeState == InteractionState.RangedInteracting)
                SetInteractionState(InteractionState.None);
        }
    }

    //private void StartRangedMenu()
    //{
    //    if (activeState == InteractionState.None)
    //        SetInteractionState(InteractionState.RangedMenuOpen);
    //}

    //public void StopRangedMenu()
    //{
    //    if (activeState == InteractionState.RangedMenuOpen)
    //        SetInteractionState(InteractionState.None);
    //}
}
