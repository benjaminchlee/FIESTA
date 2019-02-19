using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using DG.Tweening;
using UnityEngine;
using IATK;
using UnityEngine.Events;
using UnityEngine.UI;

public class Menu : Photon.MonoBehaviour {

    [SerializeField]
    private DashboardDimension dimension;
    [SerializeField]
    private DashboardPage page;
    [SerializeField]
    private CSVDataSource dataSource;
    [SerializeField]
    private GameObject menuButtonPrefab;
    [SerializeField]
    private float spacing = 0.02f;
    [SerializeField]
    private bool includeNoneButton = false;
    [SerializeField]
    private List<GameObject> objectsToShowWhenDefined;
    [SerializeField]
    private List<GameObject> objectsToHideWhenDefined;
    [SerializeField]
    private List<GameObject> objectsToShowWhenOpen;
    [SerializeField]
    private List<GameObject> objectsToHideWhenOpen;
    [SerializeField]
    private bool animateHidingAndShowing;

    [Serializable]
    public class DimensionChangedEvent : UnityEvent<DashboardDimension, string> { }
    public DimensionChangedEvent DimensionChanged;
    
    private int selectedIndex;
    public string SelectedButton
    {
        get
        {
            return buttons[selectedIndex].Text;
        }
    }
    
    private List<MenuButton> buttons;
    private bool isOpen;

    private void Start()
    {
        if (dataSource == null)
            dataSource = ChartManager.Instance.DataSource;

        buttons = new List<MenuButton>();
        isOpen = false;

        CreateButtons();
        CloseButtons(0);
    }

    private void CreateButtons()
    {
        List<string> dimensions = GetAttributesList();

        if (includeNoneButton)
            dimensions.Insert(0, "None");

        foreach (string dimensionName in dimensions)
        {
            GameObject go = Instantiate(menuButtonPrefab);
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

    private List<string> GetAttributesList()
    {
        List<string> dimensions = new List<string>();
        for (int i = 0; i < dataSource.DimensionCount; ++i)
        {
            dimensions.Add(dataSource[i].Identifier);
        }
        return dimensions;
    }

    [PunRPC]
    private void OpenButtons()
    {
        float height = buttons[0].gameObject.transform.localScale.y;

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(true);

            Vector3 targetPos = buttons[i].transform.localPosition;
            targetPos.y -= (i * (height + spacing));
            buttons[i].AnimateTowards(targetPos, 0.5f, true);
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
    private void CloseButtons(int selectedIndex)
    {
        this.selectedIndex = selectedIndex;

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].AnimateTowards(Vector3.zero, 0.5f, true, (i != selectedIndex));
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

            photonView.RPC("CloseButtons", PhotonTargets.All, selectedIndex);
        }
        else
        {
            photonView.RPC("OpenButtons", PhotonTargets.All);
        }

        isOpen = !isOpen;
    }
}
