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
    private WorkScreenDimension dimension;
    [SerializeField]
    private CSVDataSource dataSource;
    [SerializeField]
    private GameObject menuButtonPrefab;
    [SerializeField]
    private float spacing = 0.02f;

    [Serializable]
    public class DimensionChangedEvent : UnityEvent<WorkScreenDimension, string> { }
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
    private string prefix;
    private bool isOpen;

    private void Start()
    {
        buttons = new List<MenuButton>();
        isOpen = false;

        CreateButtons();
    }

    private void CreateButtons()
    {
        List<string> dimensions = GetAttributesList();

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

        // Add a prefix to the default button
        switch (dimension)
        {
            case WorkScreenDimension.X:
                prefix = "X: ";
                break;
            case WorkScreenDimension.Y:
                prefix = "Y: ";
                break;
            case WorkScreenDimension.Z:
                prefix = "Z: ";
                break;
            case WorkScreenDimension.FACETBY:
                prefix = "Facet: ";
                break;
        }
        buttons[0].Text = prefix + buttons[0].Text;
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

            Vector3 targetPos = buttons[i].transform.position;
            targetPos.y -= (i * (height + spacing));
            buttons[i].AnimateTowards(targetPos, 0.5f);
        }

        // Remove prefix from selected button
        buttons[selectedIndex].Text = buttons[selectedIndex].Text.Replace(prefix, "");
    }

    private void CloseButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].AnimateTowards(transform.position, 0.5f, (i != selectedIndex));
        }
    }

    public void ButtonClicked(MenuButton button)
    {
        // If the menu was open, then close it
        if (isOpen)
        {
            // Invoke the event telling listeners that the chosen dimension has changed
            DimensionChanged.Invoke(dimension, button.Text);

            // Add the prefix to the clicked button
            button.Text = prefix + button.Text;

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
