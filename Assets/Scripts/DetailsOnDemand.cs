using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using VRTK;
using TMPro;
using System.Text;
using System.Linq;
using System;

public class DetailsOnDemand : Photon.MonoBehaviour {

    [SerializeField]
    private DataSource dataSource;
    [SerializeField]
    private GameObject detailsPanel;
    [SerializeField]
    private TextMeshPro detailsText;

    private BrushingAndLinking brushingAndLinking;
    private VRTK_ControllerEvents controllerEvents;

    private List<int> brushedIndices;
    private int numBrushed;
    private int currentIndex = -1;
    private bool holdLeft = false;
    private bool holdRight = false;
    private float holdTimer = 0;
    private bool isPastInitialHoldThreshold = false;

    [SerializeField] [Tooltip("The amount of time the touchpad needs to be held until the page turn speed increases.")]
    private float initialHoldThreshold = 0.7f;
    [SerializeField] [Tooltip("The amount of time between page turns after the initial page turn threshold had been met.")]
    private float postHoldThreshold = 0.1f;

    private void Start()
    {
        if (dataSource == null)
            dataSource = ChartManager.Instance.DataSource;

        if (photonView.isMine)
        {
            if (controllerEvents == null)
                controllerEvents = GetComponentInParent<VRTK_ControllerEvents>();

            controllerEvents.TouchpadPressed += TouchpadPressed;
            controllerEvents.TouchpadAxisChanged += TouchpadAxisChanged;
            controllerEvents.TouchpadReleased += TouchpadReleased;

            SpinMenu spinMenu = VRTK_DeviceFinder.GetControllerLeftHand().GetComponentInChildren<SpinMenu>();
            if (spinMenu == null)
                spinMenu = VRTK_DeviceFinder.GetControllerRightHand().GetComponentInChildren<SpinMenu>();

            spinMenu.SpinMenuToolChanged.AddListener(SpinMenuToggleClicked);

            brushedIndices = new List<int>(dataSource.DataCount);
        }

        TogglePanel(false);
    }

    private void SpinMenuToggleClicked(string toolName)
    {
        if (toolName == "detailsondemand")
        {
            TogglePanel(!IsVisible());
        }
    }

    public void SetBrushingAndLinking(BrushingAndLinking bal)
    {
        brushingAndLinking = bal;
    }

    private void TouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (IsVisible() && numBrushed > 0)
        {
            if (e.touchpadAngle < 180f)
            {
                holdRight = true;
                NextPage();
            }
            else
            {
                holdLeft = true;
                PreviousPage();
            }
        }
    }

    private void TouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        if (IsVisible())
        {
            // If the touchpad was being held to the right previously, but the touchpad angle has now changed to the left side of the touchpad
            if (holdRight && e.touchpadAngle >= 180f)
            {
                holdLeft = true;
                holdRight = false;
                isPastInitialHoldThreshold = false;
                holdTimer = 0;
            }
            // If the touchpad was being held to the left previously, but the touchpad angle has now changed to the right side of the touchpad
            else if (holdLeft && e.touchpadAngle < 180f)
            {
                holdLeft = false;
                holdRight = true;
                isPastInitialHoldThreshold = false;
                holdTimer = 0;
            }
        }
    }

    private void TouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        holdLeft = false;
        holdRight = false;
        isPastInitialHoldThreshold = false;
        holdTimer = 0;
    }

    private void Update()
    {
        if (IsVisible())
        {
            if (holdRight)
            {
                holdTimer += Time.deltaTime;

                // If the touchpad is held for a duration longer than the initial hold threshold
                if (!isPastInitialHoldThreshold && initialHoldThreshold <= holdTimer)
                {
                    isPastInitialHoldThreshold = true;
                    holdTimer = 0;
                    NextPage();
                }
                // If the touchpad has already been held past the initial hold threshold earlier, increase the frequency of page turns
                else if (isPastInitialHoldThreshold && postHoldThreshold <= holdTimer)
                {
                    holdTimer = 0;
                    NextPage();
                }
            }
            else if (holdLeft)
            {
                holdTimer += Time.deltaTime;

                // If the touchpad is held for a duration longer than the initial hold threshold
                if (!isPastInitialHoldThreshold && initialHoldThreshold <= holdTimer)
                {
                    isPastInitialHoldThreshold = true;
                    holdTimer = 0;
                    PreviousPage();
                }
                // If the touchpad has already been held past the initial hold threshold earlier, increase the frequency of page turns
                else if (isPastInitialHoldThreshold && postHoldThreshold <= holdTimer)
                {
                    holdTimer = 0;
                    PreviousPage();
                }
            }
        }
    }


    private bool IsVisible()
    {
        return detailsPanel.activeInHierarchy;
    }

    public void TogglePanel(bool value)
    {
        photonView.RPC("PropagateTogglePanel", PhotonTargets.All, value);

        if (value)
        {
            if (currentIndex == -1)
            {
                SetDetailsPage(GetFirstBrushedIndex());
            }
            else
            {
                brushingAndLinking.HighlightIndex(currentIndex);
            }
        }
        else
        {
            brushingAndLinking.HighlightIndex(-1);
        }
    }

    [PunRPC]
    private void PropagateTogglePanel(bool value)
    {
        detailsPanel.SetActive(value);
    }

    private void SetDetailsPage(int index)
    {
        brushingAndLinking.HighlightIndex(index);

        photonView.RPC("PropagateSetDetailsPage", PhotonTargets.All, index, numBrushed);
    }

    [PunRPC]
    private void PropagateSetDetailsPage(int index, int totalBrushed)
    {
        currentIndex = index;

        StringBuilder stringBuilder = new StringBuilder();

        // If there is a current index
        if (currentIndex >= 0)
        {
            stringBuilder.AppendFormat("Record {0} of {1} ({2} selected)\n", currentIndex + 1, dataSource.DataCount, totalBrushed);
            stringBuilder.Append("\n");

            for (int i = 0; i < dataSource.DimensionCount; i++)
            {
                stringBuilder.AppendFormat("<b>{0}:</b> {1}\n", dataSource[i].Identifier, dataSource.getOriginalValue(dataSource[i].Data[currentIndex], dataSource[i].Identifier));
            }
        }
        // If there is no current index (i.e. it is -1)
        else
        {
            stringBuilder.Append("No brushed points.");
        }

        detailsText.text = stringBuilder.ToString();
    }

    /// <summary>
    /// Loops through the brushed indices to find the next index that is selected from the currently brushed one
    /// </summary>
    public void NextPage()
    {
        bool pageFound = false;
        for (int i = 1; i < dataSource.DataCount; i++)
        {
            int idx = (currentIndex + i) % dataSource.DataCount;
            if (brushedIndices[idx] == 1)
            {
                currentIndex = idx;
                pageFound = true;
                VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(transform.parent.gameObject), 0.2f);
                break;
            }
        }

        if (!pageFound)
            currentIndex = -1;

        SetDetailsPage(currentIndex);
    }

    public void PreviousPage()
    {
        bool pageFound = false;
        for (int i = 1; i < dataSource.DataCount; i++)
        {
            int idx;

            if (currentIndex - i >= 0)
                idx = currentIndex - i;
            else
                idx = dataSource.DataCount - i + currentIndex;

            if (brushedIndices[idx] == 1)
            {
                currentIndex = idx;
                pageFound = true;
                VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(transform.parent.gameObject), 0.2f);
                break;
            }
        }

        if (!pageFound)
            currentIndex = -1;

        SetDetailsPage(currentIndex);
    }

    public void BrushedIndicesChanged(List<int> brushedIndices)
    {
        if (!this.brushedIndices.SequenceEqual(brushedIndices))
        {
            this.brushedIndices = brushedIndices;
            numBrushed = brushedIndices.Count(x => x == 1);

            if (IsVisible())
            {
                if (currentIndex == -1 || brushedIndices[currentIndex] == -1)
                {
                    NextPage();
                }
                else
                {
                    SetDetailsPage(currentIndex);
                }
            }
        }
    }

    private int GetFirstBrushedIndex()
    {
        try
        {
            return brushedIndices.IndexOf(1);
        }
        catch
        {
            return -1;
        }
    }
}
