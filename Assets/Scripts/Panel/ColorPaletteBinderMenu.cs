using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ColorPaletteBinderMenu : MonoBehaviourPunCallbacks {

    [SerializeField]
    private List<ColorPaletteBinderButton> buttons;
    [SerializeField]
    private List<ColorPaletteBinderButton> sortedButtons;
    [SerializeField]
    private MenuButton confirmButton;
    [SerializeField]
    private VRColorPicker vrColorPicker;
    [SerializeField]
    private TextMeshPro label;

    [SerializeField]
    private float spacing = 0.02f;

    private string dimension;
    private int selectedIndex;

    [Serializable]
    public class ColorPaletteChangedEvent : UnityEvent<Color[]> { }
    public ColorPaletteChangedEvent ColorPaletteChanged;

    public void CreateColorButtons(PanelDimension dimension, string dimensionName)
    {
        if (dimension == PanelDimension.COLORPALETTE)
        {
            if (dimensionName != "Undefined")
            {
                photonView.RPC("CreateColorButtons", RpcTarget.All, dimensionName);
            }
            else
            {
                photonView.RPC("DestroyColorButtons", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void CreateColorButtons(string dimension)
    {
        if (this.dimension != dimension)
        {
            this.dimension = dimension;

            DestroyColorButtons();

            List<float> paletteValues = ChartManager.Instance.DataSource[dimension].MetaData.categories.ToList();

            for (int i = 0; i < paletteValues.Count; i++)
            {
                GameObject button = (GameObject) Instantiate(Resources.Load("ColorPaletteBinderButton"));
                button.transform.SetParent(transform);
                ColorPaletteBinderButton paletteButton = button.GetComponent<ColorPaletteBinderButton>();

                paletteButton.Text = ChartManager.Instance.DataSource.getOriginalValue(paletteValues[i], dimension).ToString();
                paletteButton.NormalisedValue = paletteValues[i];
                paletteButton.ButtonClicked.AddListener(ColorButtonClicked);


                buttons.Add(paletteButton);
            }

            // Sort the buttons
            sortedButtons = buttons.OrderBy(x => x.NormalisedValue).ToList();

            // Position the buttons
            for (int i = 0; i < sortedButtons.Count; i++)
            {
                sortedButtons[i].Color = Color.HSVToRGB((i * 0.09f) % 1, 1, 1);

                float height = sortedButtons[i].transform.localScale.y;
                Vector3 targetPos = Vector3.zero;
                targetPos.y -= (i * (height + spacing));
                sortedButtons[i].transform.localPosition = targetPos;
                sortedButtons[i].transform.localRotation = Quaternion.identity; ;
            }


            ColorPaletteChanged.Invoke(GetColorPalette());
            // Reset the page
            ButtonClicked(null);
        }
    }
    
    public void CreateColorButtonsFromPalette(string dimension, Color[] palette)
    {
        if (this.dimension != dimension)
        {
            this.dimension = dimension;

            DestroyColorButtons();

            float[] paletteValues = ChartManager.Instance.DataSource[dimension].MetaData.categories;

            for (int i = 0; i < paletteValues.Length; i++)
            {
                GameObject button = (GameObject)Instantiate(Resources.Load("ColorPaletteBinderButton"));
                button.transform.SetParent(transform);
                ColorPaletteBinderButton paletteButton = button.GetComponent<ColorPaletteBinderButton>();

                paletteButton.Color = palette[i];
                paletteButton.Text = ChartManager.Instance.DataSource.getOriginalValue(paletteValues[i], dimension).ToString();
                paletteButton.ButtonClicked.AddListener(ColorButtonClicked);

                float height = paletteButton.transform.localScale.y;
                Vector3 targetPos = Vector3.zero;
                targetPos.y -= (i * (height + spacing));
                paletteButton.transform.localPosition = targetPos;
                paletteButton.transform.localRotation = Quaternion.identity; ;

                buttons.Add(paletteButton);
            }

            // Reset the page
            ButtonClicked(null);
        }
    }

    [PunRPC]
    public void DestroyColorButtons()
    {
        foreach (ColorPaletteBinderButton button in buttons)
        {
            Destroy(button.gameObject);
        }

        dimension = "";
        buttons.Clear();
    }

    public void ToggleColorButtons(bool visibility)
    {
        foreach (ColorPaletteBinderButton button in buttons)
        {
            button.gameObject.SetActive(visibility);
        }
    }

    public void ColorButtonClicked(ColorPaletteBinderButton button)
    {
        photonView.RPC("ShowColorPalettePicker", RpcTarget.All, buttons.IndexOf(button));
    }

    [PunRPC]
    private void ShowColorPalettePicker(int selectedIndex)
    {
        this.selectedIndex = selectedIndex;
        ColorPaletteBinderButton button = buttons[selectedIndex];

        ToggleColorButtons(false);
        confirmButton.gameObject.SetActive(true);
        label.gameObject.SetActive(true);
        label.text = "Picking Colour for Value: \n<b>" + button.Text + "</b>";

        Color col = button.Color;
        vrColorPicker.gameObject.SetActive(true);
        vrColorPicker.SetColor(col);
    }

    public void ButtonClicked(MenuButton button)
    {
        photonView.RPC("HideColorPalettePicker", RpcTarget.All);
    }

    [PunRPC]
    private void HideColorPalettePicker()
    {
        ToggleColorButtons(true);

        confirmButton.gameObject.SetActive(false);
        vrColorPicker.gameObject.SetActive(false);
        label.gameObject.SetActive(false);
    }

    public void ColorPickerValueChanged(Color color)
    {
        buttons[selectedIndex].Color = color;

        ColorPaletteChanged.Invoke(GetColorPalette());
    }

    private Color[] GetColorPalette()
    {
        List<Color> colors = new List<Color>();

        foreach (ColorPaletteBinderButton button in buttons)
        {
            colors.Add(button.Color);
        }

        return colors.ToArray();
    }

    public void LoadColorPalette(Color[] palette)
    {
        for (int i = 0; i < palette.Length; i++)
        {
            Color col = palette[i];
            ColorPaletteBinderButton button = buttons[i];

            button.Color = col;
        }
    }
}
