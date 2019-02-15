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

    public Color currentColor;

    private void Start()
    {
        SetColor(startColor);
    }

    public void SetColor(Color color)
    {
        currentColor = color;

        float h, s, v;
        Color.RGBToHSV(currentColor, out h, out s, out v);
        
        hueSlider.SetHue(h);
        saturationBrightnessPicker.SetSaturation(s);
        saturationBrightnessPicker.SetBrightness(v);
        
        ColorChanged.Invoke(currentColor);
    }

    public void HueChanged(float hue)
    {
        float h, s, v;
        Color.RGBToHSV(currentColor, out h, out s, out v);
        currentColor = Color.HSVToRGB(hue, s, v);

        ColorChanged.Invoke(currentColor);
    }

    public void SaturationChanged(float saturation)
    {
        float h, s, v;
        Color.RGBToHSV(currentColor, out h, out s, out v);
        currentColor = Color.HSVToRGB(h, saturation, v);

        ColorChanged.Invoke(currentColor);
    }

    public void BrightnessChanged(float brightness)
    {
        float h, s, v;
        Color.RGBToHSV(currentColor, out h, out s, out v);
        currentColor = Color.HSVToRGB(h, s, brightness);

        ColorChanged.Invoke(currentColor);
    }
}
