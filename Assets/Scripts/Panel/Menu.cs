using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using IATK;
using Photon.Pun;
using UnityEngine.Events;
using UnityEngine.UI;

public class Menu : MonoBehaviourPunCallbacks {

    [SerializeField]
    protected PanelDimension dimension;
    [SerializeField]
    protected PanelPage page;
    [SerializeField]
    protected CSVDataSource dataSource;
    [SerializeField]
    protected float spacing = 0.02f;
    [SerializeField]
    protected bool includeNoneButton = false;
    [SerializeField]
    protected List<GameObject> objectsToShowWhenDefined;
    [SerializeField]
    protected List<GameObject> objectsToHideWhenDefined;
    [SerializeField]
    protected List<GameObject> objectsToShowWhenOpen;
    [SerializeField]
    protected List<GameObject> objectsToHideWhenOpen;
    [SerializeField]
    protected bool animateHidingAndShowing;

    [Serializable]
    public class DimensionChangedEvent : UnityEvent<PanelDimension, string> { }
    public DimensionChangedEvent DimensionChanged;

    protected int selectedIndex;
    public string SelectedButton
    {
        get
        {
            return buttons[selectedIndex].Text;
        }
    }

    protected List<MenuButton> buttons;
    protected bool isOpen;
    protected bool isInitialised = false;

    private void Start()
    {
        if (!isInitialised)
        {
            Initialise();
        }
    }

    // This is needed to create the buttons if the script is disabled
    protected void Initialise()
    {
        if (dataSource == null)
            dataSource = ChartManager.Instance.DataSource;

        buttons = new List<MenuButton>();
        isOpen = false;

        CreateButtons();
        CloseButtons(0);

        isInitialised = true;
    }

    protected void CreateButtons()
    {
        List<string> dimensions = GetAttributesList();

        if (includeNoneButton)
            dimensions.Insert(0, "None");

        foreach (string dimensionName in dimensions)
        {
            GameObject go = (GameObject) Instantiate(Resources.Load("MenuButton"));
            go.transform.SetParent(transform);
            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;

            MenuButton button = go.GetComponent<MenuButton>();
            buttons.Add(button);
            button.ButtonClicked.AddListener(ButtonClicked);
            button.Text = dimensionName;

            go.SetActive(false);
        }

        buttons[0].gameObject.SetActive(true);
        selectedIndex = 0;
    }

    protected virtual List<string> GetAttributesList()
    {
        if (dataSource == null)
            dataSource = ChartManager.Instance.DataSource;

        List<string> dimensions = new List<string>();
        for (int i = 0; i < dataSource.DimensionCount; ++i)
        {
            dimensions.Add(dataSource[i].Identifier);
        }
        return dimensions;
    }

    [PunRPC]
    protected void OpenButtons()
    {
        float height = buttons[0].gameObject.transform.localScale.y;

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(true);

            Vector3 targetPos = buttons[i].transform.localPosition;
            targetPos.y -= (i * (height + spacing));
            buttons[i].AnimateTowards(targetPos, 0.35f, true);
        }

        // Switch object visibility associated with this menu
        if (animateHidingAndShowing)
        {
            foreach (GameObject go in objectsToShowWhenOpen)
            {
                go.SetActive(true);
                go.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
            }
            foreach (GameObject go in objectsToHideWhenOpen)
            {
                go.transform.DOScale(0f, 0.2f).SetEase(Ease.OutBack).OnComplete(() => go.SetActive(false));
            }
        }
        else
        {
            foreach (GameObject go in objectsToShowWhenOpen)
                go.SetActive(true);
            foreach (GameObject go in objectsToHideWhenOpen)
                go.SetActive(false);
        }
    }

    [PunRPC]
    protected void CloseButtons(int selectedIndex)
    {
        this.selectedIndex = selectedIndex;

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].AnimateTowards(Vector3.zero, 0.35f, true, (i != selectedIndex));
        }

        // Switch object visibility associated with this menu
        if (animateHidingAndShowing)
        {
            foreach (GameObject go in objectsToShowWhenOpen)
            {
                go.transform.DOScale(0f, 0.2f).SetEase(Ease.OutBack).OnComplete(() => go.SetActive(false));
            }
            foreach (GameObject go in objectsToHideWhenOpen)
            {
                go.SetActive(true);
                go.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
            }

            // Show/hide special buttons depending on selected value
            bool isDefined = buttons[selectedIndex].Text != "None";
            foreach (GameObject go in objectsToShowWhenDefined)
            {
                if (isDefined)
                {
                    go.SetActive(true);
                    go.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
                }
                else
                {
                    go.transform.DOScale(0f, 0.2f).SetEase(Ease.OutBack).OnComplete(() => go.SetActive(false));
                }
            }
            foreach (GameObject go in objectsToHideWhenDefined)
            {
                if (isDefined)
                {
                    go.transform.DOScale(0f, 0.2f).SetEase(Ease.OutBack).OnComplete(() => go.SetActive(false));
                }
                else
                {
                    go.SetActive(true);
                    go.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
                }
            }
        }
        else
        {
            foreach (GameObject go in objectsToShowWhenOpen)
                go.SetActive(false);
            foreach (GameObject go in objectsToHideWhenOpen)
                go.SetActive(true);

            // Show/hide special buttons depending on selected value
            bool isDefined = buttons[selectedIndex].Text != "None";
            foreach (GameObject go in objectsToShowWhenDefined)
                go.SetActive(isDefined);
            foreach (GameObject go in objectsToHideWhenDefined)
                go.SetActive(!isDefined);
        }
    }

    public void ButtonClicked(MenuButton button)
    {
        // If the menu was open, then close it
        if (isOpen)
        {
            // Invoke the event telling listeners that the chosen dimension has changed
            if (button.Text == "None")
            {
                DimensionChanged.Invoke(dimension, "Undefined");
            }
            else
            {
                DimensionChanged.Invoke(dimension, button.Text);
            }
            
            // Store the index of the selected option
            selectedIndex = buttons.IndexOf(button);

            photonView.RPC("CloseButtons", RpcTarget.All, selectedIndex);
        }
        else
        {
            photonView.RPC("OpenButtons", RpcTarget.All);
        }

        isOpen = !isOpen;
    }

    public virtual void ChartTransferred(Chart chart)
    {
        if (!isInitialised)
            Initialise();

        string valueToCompare = "";

        switch (dimension)
        {
            case PanelDimension.X:
                valueToCompare = chart.XDimension;
                break;

            case PanelDimension.Y:
                valueToCompare = chart.YDimension;
                break;

            case PanelDimension.Z:
                valueToCompare = chart.ZDimension;
                break;

            case PanelDimension.SIZEBY:
                valueToCompare = chart.SizeDimension;
                break;

            case PanelDimension.FACETBY:
                valueToCompare = chart.FacetDimension;
                break;

            case PanelDimension.COLORBY:
                valueToCompare = chart.ColorDimension;
                break;

            case PanelDimension.COLORPALETTE:
                valueToCompare = chart.ColorPaletteDimension;
                break;

            case PanelDimension.LINKING:
                valueToCompare = chart.LinkingDimension;
                break;
        }

        // If the dimension is undefined, the "None" button is the first index
        if (includeNoneButton && valueToCompare == "Undefined")
        {
            selectedIndex = 0;
            CloseButtons(selectedIndex);
        }
        // If the dimension is undefined and yet there is no "None" button, log an error
        else if (!includeNoneButton && valueToCompare == "Undefined")
        {
            Debug.LogError("The dimension " + Enum.GetName(typeof(PanelDimension), dimension) + " was set to Undefined but does not have a None button.");
        }
        else
        {
            List<string> dimensions = GetAttributesList();
            selectedIndex = dimensions.IndexOf(valueToCompare);

            if (includeNoneButton)
                selectedIndex++;

            CloseButtons(selectedIndex);
        }
    }
}
