using System;
using UnityEngine;
using UnityEngine.Events;

public class VRColorPicker : MonoBehaviour {

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

    private float currentHue = -1;
    private float currentSaturation = -1;
    private float currentBrightness = -1;

    private int previousEventFrameCount = 0;
    private bool isEventQueued = false;

    private void Start()
    {
        if (currentHue < 0 || currentBrightness < 0 || currentBrightness < 0)
        {
            SetColor(startColor);
        }
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

    public void ChartTransferred(Chart chart)
    {
        if (chart.ColorDimension == "Undefined" && chart.ColorPaletteDimension == "Undefined")
        {
            Color color = chart.Color;
            Color.RGBToHSV(color, out currentHue, out currentSaturation, out currentBrightness);

            hueSlider.SetHue(currentHue);
            saturationBrightnessPicker.SetSaturation(currentSaturation);
            saturationBrightnessPicker.SetBrightness(currentBrightness);
        }
    }
}
