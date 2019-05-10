using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class GradientMenu : MonoBehaviourPunCallbacks {

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
    private bool isInitialised = false;

    [SerializeField]
    private List<GameObject> objectsToShowWhenOpen;
    [SerializeField]
    private List<GameObject> objectsToHideWhenOpen;

    [Serializable]
    public class GradientChangedEvent : UnityEvent<Gradient> { }
    public GradientChangedEvent GradientChanged;

    private void Awake()
    {
        if (!isInitialised)
            Initialise();
    }

    private void Initialise()
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

        isInitialised = true;
    }

    private void OnEnable()
    {
        if (!isReversed)
            GradientChanged.Invoke(gradientButtons[selectedIndex].Gradient);
        else
            GradientChanged.Invoke(ReverseGradient(gradientButtons[selectedIndex].Gradient));
    }

    [PunRPC]
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

        // Switch object visibility associated with this menu
        foreach (GameObject go in objectsToShowWhenOpen)
            go.SetActive(true);
        foreach (GameObject go in objectsToHideWhenOpen)
            go.SetActive(false);
    }

    [PunRPC]
    private void CloseButtons(int selectedIndex)
    {
        this.selectedIndex = selectedIndex;

        for (int i = 0; i < gradientButtons.Count; i++)
        {
            gradientButtons[i].AnimateTowards(Vector3.zero, 0.5f, true, (i != selectedIndex));
        }

        // Switch object visibility associated with this menu
        foreach (GameObject go in objectsToShowWhenOpen)
            go.SetActive(false);
        foreach (GameObject go in objectsToHideWhenOpen)
            go.SetActive(true);
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

            photonView.RPC("CloseButtons", RpcTarget.All, selectedIndex);
        }
        else
        {
            photonView.RPC("OpenButtons", RpcTarget.All);
        }

        isOpen = !isOpen;
    }

    private void ReverseButtonClicked(MenuButton button)
    {
        photonView.RPC("ReverseGradientButtons", RpcTarget.All);
    }

    [PunRPC]
    private void ReverseGradientButtons()
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

    public void ChartTransferred(Chart chart)
    {
        if (!isInitialised)
            Initialise();

        if (chart.ColorDimension != "Undefined")
        {
            GradientButton selectedGradientButton = FindGradientButton(chart.Gradient);
            selectedIndex = gradientButtons.IndexOf(selectedGradientButton);

            CloseButtons(selectedIndex);
        }
    }

    private GradientButton FindGradientButton(Gradient gradient)
    {
        foreach (GradientButton button in gradientButtons)
        {
            if (EquateGradients(gradient, button.Gradient))
            {
                return button;
            }

            // Check reverse
            if (EquateGradients(gradient, ReverseGradient(button.Gradient)))
            {
                return button;
            }
        }

        Debug.LogError("Unable to find corresponding gradient button with supplied gradient.");
        return null;
    }

    private bool EquateGradients(Gradient grad1, Gradient grad2)
    {
        // First check if number of keys are the same
        if (grad1.colorKeys.Length != grad2.colorKeys.Length)
            return false;

        // Then check if every color key has the same time and color
        for (int i = 0; i < grad1.colorKeys.Length; i++)
        {
            GradientColorKey key1 = grad1.colorKeys[i];
            GradientColorKey key2 = grad2.colorKeys[i];

            if (key1.time != key2.time || key1.color != key2.color)
                return false;
        }

        return true;
    }
}
