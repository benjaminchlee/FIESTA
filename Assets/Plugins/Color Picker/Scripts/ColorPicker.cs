using System;
using UnityEngine;
using UnityEngine.Events;

public class ColorPicker : MonoBehaviour {

    [Serializable]
    public class ColorChangedEvent : UnityEvent<Color> { }
    public ColorChangedEvent ColorChanged;

    [SerializeField]
    private HueSlider hueSlider;
    [SerializeField]
    private SaturationBrightnessPicker saturationBrightnessPicker;
    [SerializeField]
    private Color startColor = Color.white;
    [SerializeField]
    private int framesBetweenUpdates = 5;

    public float currentHue;
    public float currentSaturation;
    public float currentBrightness;

    private int previousEventFrameCount = 0;
    private bool isEventQueued = false;

    private void Start()
    {
        SetColor(startColor);
    }
    
    private void Update()
    {
        if (isEventQueued && (Time.frameCount - previousEventFrameCount > framesBetweenUpdates))
        {
            ColorChanged.Invoke(GetColor());
            previousEventFrameCount = Time.frameCount;
            isEventQueued = false;
        }
    }

    public void SetColor(Color color)
    {
        Color.RGBToHSV(color, out currentHue, out currentSaturation, out currentBrightness);

        hueSlider.SetHue(currentHue);
        saturationBrightnessPicker.SetSaturation(currentSaturation);
        saturationBrightnessPicker.SetBrightness(currentBrightness);

        EmitEvents();
    }

    public Color GetColor()
    {
        return Color.HSVToRGB(currentHue, currentSaturation, currentBrightness);
    }

    public void HueChanged(float hue)
    {
        currentHue = hue;

        EmitEvents();
    }

    public void SaturationChanged(float saturation)
    {
        currentSaturation = saturation;

        EmitEvents();
    }

    public void BrightnessChanged(float brightness)
    {
        currentBrightness = brightness;

        EmitEvents();
    }

    private void EmitEvents()
    {
        // Only immediately emit an event if there is no queued event and there has been enough frames since the last update
        if (!isEventQueued && Time.frameCount - previousEventFrameCount > framesBetweenUpdates)
        {
            ColorChanged.Invoke(GetColor());
            previousEventFrameCount = Time.frameCount;
        }
        else
        {
            isEventQueued = true;
        }
    }
}
