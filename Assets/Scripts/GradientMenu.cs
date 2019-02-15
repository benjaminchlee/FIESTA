using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GradientMenu : MonoBehaviour {

    [Serializable]
    public struct GradientTexturePair
    {
        public string name;
        public Texture texture;
        public Gradient gradient;
    }

    [SerializeField]
    private GradientTexturePair[] gradientTexturePairs;
    [SerializeField]
    private float spacing = 0.005f;
    [SerializeField]
    private MenuButton reverseButton;

    private List<GradientButton> gradientButtons;
    private int selectedIndex;
    private bool isOpen = false;
    private bool isReversed = false;

    [Serializable]
    public class GradientChangedEvent : UnityEvent<Gradient> { }
    public GradientChangedEvent GradientChanged;

    private void Start()
    {
        gradientButtons = new List<GradientButton>();

        foreach (GradientTexturePair gradientTexturePair in gradientTexturePairs)
        {
            GameObject go = Instantiate(Resources.Load("GradientButton"), transform) as GameObject;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            GradientButton gradientButton = go.GetComponent<GradientButton>();

            gradientButton.Text = gradientTexturePair.name;
            gradientButton.Texture = gradientTexturePair.texture;
            gradientButton.Gradient = gradientTexturePair.gradient;
            gradientButton.ButtonClicked.AddListener(GradientButtonClicked);

            gradientButtons.Add(gradientButton);

            go.SetActive(false);
        }

        gradientButtons[0].gameObject.SetActive(true);
        selectedIndex = 0;

        reverseButton.ButtonClicked.AddListener(ReverseButtonClicked);
    }

    private void OpenButtons()
    {
        float height = gradientButtons[0].gameObject.transform.localScale.y;

        for (int i = 0; i < gradientButtons.Count; i++)
        {
            gradientButtons[i].gameObject.SetActive(true);

            Vector3 targetPos = gradientButtons[i].transform.localPosition;
            targetPos.y -= (i * (height + spacing));
            gradientButtons[i].AnimateTowards(targetPos, 0.5f, true);
        }
    }

    private void CloseButtons()
    {
        for (int i = 0; i < gradientButtons.Count; i++)
        {
            gradientButtons[i].AnimateTowards(Vector3.zero, 0.5f, true, (i != selectedIndex));
        }
    }

    private void GradientButtonClicked(GradientButton gradientButton)
    {
        // If the menu was open, then close it
        if (isOpen)
        {
            // Invoke the event telling listeners that the chosen dimension has changed
            if (!isReversed)
                GradientChanged.Invoke(gradientButton.Gradient);
            else
                GradientChanged.Invoke(ReverseGradient(gradientButton.Gradient));

            // Store the index of the selected option
            selectedIndex = gradientButtons.IndexOf(gradientButton);

            CloseButtons();
        }
        else
        {
            OpenButtons();
        }

        isOpen = !isOpen;
    }

    private void ReverseButtonClicked(MenuButton button)
    {
        isReversed = !isReversed;
        
        foreach (GradientButton gradientButton in gradientButtons)
        {
            gradientButton.SetGradientReversed(isReversed);
        }

        // Emit the event again with the gradient
        if (!isReversed)
            GradientChanged.Invoke(gradientButtons[selectedIndex].Gradient);
        else
            GradientChanged.Invoke(ReverseGradient(gradientButtons[selectedIndex].Gradient));
    }

    private Gradient ReverseGradient(Gradient gradient)
    {
        List<GradientColorKey> reversedColorKeys = new List<GradientColorKey>();
        foreach (GradientColorKey colorKey in gradient.colorKeys)
        {
            GradientColorKey reversedColorKey = new GradientColorKey(colorKey.color, 1 - colorKey.time);
            reversedColorKeys.Add(reversedColorKey);
        }
        
        Gradient reversedGradient = new Gradient();
        reversedGradient.colorKeys = reversedColorKeys.ToArray();
        return reversedGradient;
    }
}
