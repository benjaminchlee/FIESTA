using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using VRTK;
using TMPro;

public class SpinMenu : MonoBehaviourPunCallbacks {
    
    [SerializeField] [Tooltip("The buttons that are part of the spin menu. Note that the cancel button should be the last element of this list.")]
    private List<SpinMenuButton> buttons = new List<SpinMenuButton>();
    [SerializeField] [Tooltip("The button that acts as the cancel button. This both signifies that no tool is selected and to cancel the active tool.")]
    private SpinMenuButton cancelButton;
    [SerializeField] [Tooltip("The button that acts as the details on demand toggle.")]
    private SpinMenuButton detailsOnDemandButton;
    [SerializeField] [Tooltip("The local position of the details on demand toggle button.")]
    private Vector3 detailsOnDemandPosition = new Vector3(0, 0.1f, -0.0525f);

    [SerializeField] [Tooltip("The local position of the button that rests on top of the controller.")]
    private Vector3 topPosition = new Vector3(0, 0.06f, -0.0525f);
    [SerializeField] [Tooltip("The angle at which to position the first button when the menu is expanded, where 0 degrees is forwards from the controller and 180 degrees is backwards.")]
    private float startAngle = -80;
    [SerializeField] [Tooltip("The offset angle at which following buttons are positioned, where positive angles are in the clockwise direction.")]
    private float intervalAngle = 55;
    [SerializeField] [Tooltip("The distance from the center of the controller to position the buttons.")]
    private float buttonDistance = 0.15f;
    [SerializeField] [Tooltip("The duration of the animation when the menu is opened and closed.")]
    private float animationDuration = 0.1f;
    [SerializeField] [Tooltip("The color of a button when it is unselected.")]
    private Color unselectedColor = new Color(0, 0, 0);
    [SerializeField] [Tooltip("The color of a button when it is selected.")]
    private Color selectedColor = new Color(190, 0, 0);

    private VRTK_ControllerEvents controllerEvents;
    private bool isExpanded = false;
    private bool isEnabled = false;
    private SpinMenuButton activeButton = null;

    [Serializable]
    public class SpinMenuToolChangedEvent : UnityEvent<string> { }
    public SpinMenuToolChangedEvent SpinMenuToolChanged;

    private void Start()
    {
        if (photonView.IsMine)
        {
            // If the cancel button is not the last element in the last, move it to the end
            if (buttons.IndexOf(cancelButton) != buttons.Count - 1)
            {
                buttons.Remove(cancelButton);
                buttons.Add(cancelButton);
            }

            cancelButton.transform.position = topPosition;
            cancelButton.Color = selectedColor;
            activeButton = cancelButton;

            // Make all other buttons size 0
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] != cancelButton)
                    buttons[i].transform.localScale = Vector3.zero;
            }

            controllerEvents = transform.parent.GetComponentInChildren<VRTK_ControllerEvents>();
            controllerEvents.TouchpadPressed += OnTouchpadPressed;
            controllerEvents.TouchpadReleased += OnTouchpadReleased;
        }
        else
        {
            isEnabled = false;
        }
    }

    /// <summary>
    /// This method makes the spin menu available to be interacted with.
    /// </summary>
    public void Enable()
    {
        isEnabled = true;
        
        RetractButtons();
    }

    /// <summary>
    /// This method hides the spin menu such that it can no longer be interacted with.
    /// </summary>
    public void Disable()
    {
        isEnabled = false;
        isExpanded = false;

        // Reset the active tool
        ActiveButtonChanged(cancelButton);

        HideButtons();
    }

    public void Show()
    {
        isEnabled = true;
        isExpanded = false;

        ShowButtons();
    }

    public void Hide()
    {
        isEnabled = false;
        isExpanded = false;

        HideButtons();
    }


    private void OnTouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (isEnabled && !isExpanded)
        {
            isExpanded = true;
            ExpandButtons();
        }
    }

    private void OnTouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        if (isExpanded)
        {
            isExpanded = false;
            RetractButtons();

            DataLogger.Instance.LogActionData(this, PhotonNetwork.LocalPlayer, PhotonNetwork.LocalPlayer, "Spin Menu end");
        }
    }

    public void SpinMenuButtonClicked(SpinMenuButton spinMenuButton)
    {
        if (isEnabled)
        {
            // If the menu is closed, open it
            if (!isExpanded)
            {
                isExpanded = true;
                ExpandButtons();
            }
            // If the menu is open, choose the clicked button
            else
            {
                // But if it is the details on demand, toggle it instead
                if (spinMenuButton == detailsOnDemandButton)
                {
                   // SpinMenuToolChanged.Invoke(spinMenuButton.RangedInteractionsToolName);
                }
                else
                {
                    isExpanded = false;

                    ActiveButtonChanged(spinMenuButton);

                    RetractButtons();

                    DataLogger.Instance.LogActionData(this, PhotonNetwork.LocalPlayer, PhotonNetwork.LocalPlayer, "Spin Menu end");
                }
            }
        }
    }

    private void ActiveButtonChanged(SpinMenuButton spinMenuButton)
    {
        activeButton = spinMenuButton;

        foreach (SpinMenuButton button in buttons)
        {
            button.Color = (button == activeButton) ? selectedColor : unselectedColor;
        }
        
        SpinMenuToolChanged.Invoke(spinMenuButton.RangedInteractionsToolName);
    }

    /// <summary>
    /// This method expands the buttons from the default resting state to the expanded state.
    /// </summary>
    private void ExpandButtons()
    {
        // Set the expanded positions of the buttons
        for (int i = 0; i < buttons.Count; i++)
        {
            // Set the position of the cancel button to be above the controller as well as its text
            if (buttons[i] == cancelButton)
            {
                cancelButton.AnimateToPosition(topPosition, Vector3.one, animationDuration);
                cancelButton.Text = "Cancel";
            }
            else if (buttons[i] == detailsOnDemandButton)
            {
                //detailsOnDemandButton.AnimateToPosition(detailsOnDemandPosition, Vector3.one, animationDuration);
            }
            else
            {
                float angle = (startAngle + intervalAngle * i) % 360;
                Vector2 direction = new Vector2((float)Math.Sin(angle * Mathf.Deg2Rad), (float)Math.Cos(angle * Mathf.Deg2Rad));
                Vector2 p = direction * buttonDistance;
                Vector3 position = new Vector3(p.x, 0, p.y);

                buttons[i].AnimateToPosition(position, Vector3.one, animationDuration);
            }
        }

        DataLogger.Instance.LogActionData(this, PhotonNetwork.LocalPlayer, PhotonNetwork.LocalPlayer, "Spin Menu start");
    }

    /// <summary>
    /// This method retracts the buttons from their expanded state to the default resting state. The button whose tool is currently active will be positioned
    /// on top of the controller, otherwise the cancel button will be positioned there instead.
    /// </summary>
    private void RetractButtons()
    {        
        for (int i = 0; i < buttons.Count; i++)
        {
            // If the button is not the active one, send it back to the controller
            if (buttons[i] != activeButton)
            {
                buttons[i].AnimateToPosition(Vector3.zero, Vector3.zero, animationDuration);
            }
            // Otherwise move it on top of the controller's touchpad
            else
            {
                buttons[i].AnimateToPosition(topPosition, Vector3.one, animationDuration);
            }
        }

        // Change the text of the cancel button
        cancelButton.Text = "No Tool Selected";
    }

    private void ShowButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            // If the button is not the active one, send it back to the controller
            if (buttons[i] != activeButton)
            {
                buttons[i].AnimateToPosition(Vector3.zero, Vector3.zero, animationDuration);
            }
            // Otherwise move it on top of the controller's touchpad
            else
            {
                buttons[i].AnimateToPosition(topPosition, Vector3.one, animationDuration);
            }
        }

        // Change the text of the cancel button
        cancelButton.Text = "No Tool Selected";
    }

    /// <summary>
    /// This method hides all buttons such that they cannot be interacted with.
    /// </summary>
    private void HideButtons()
    {
        // Hide the buttons
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].AnimateToPosition(Vector3.zero, Vector3.zero, animationDuration);
        }
        
        // Change the text of the cancel button
        cancelButton.Text = "No Tool Selected";
    }

}
