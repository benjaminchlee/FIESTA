using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using UnityEngine;
using IATK;
using UnityEngine.Events;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

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
    private bool hideSpecialButtonsWhenUndefined = false;

    [Serializable]
    public class DimensionChangedEvent : UnityEvent<DashboardDimension, string> { }
    public DimensionChangedEvent DimensionChanged;

    [Serializable]
    public class SpecialButtonsChangedEvent : UnityEvent<DashboardPage, bool> { }
    public SpecialButtonsChangedEvent SpecialButtonsChanged;


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
    }

    private void CloseButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].AnimateTowards(Vector3.zero, 0.5f, true, (i != selectedIndex));
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
                SpecialButtonsChanged.Invoke(page, !hideSpecialButtonsWhenUndefined);
            }
            else
            {
                DimensionChanged.Invoke(dimension, button.Text);
                SpecialButtonsChanged.Invoke(page, hideSpecialButtonsWhenUndefined);
            }
            
            // Store the index of the selected option
            selectedIndex = buttons.IndexOf(button);

            CloseButtons();
        }
        else
        {
            OpenButtons();
        }

        isOpen = !isOpen;
    }
}
