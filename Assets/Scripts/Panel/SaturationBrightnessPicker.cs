using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class SaturationBrightnessPicker : MonoBehaviour {

    [Serializable]
    public class SaturationChangedEvent : UnityEvent<float> { }
    public SaturationChangedEvent SaturationChanged;

    [Serializable]
    public class BrightnessChangedEvent : UnityEvent<float> { }
    public BrightnessChangedEvent BrightnessChanged;

    [SerializeField]
    private ComputeShader computeShader;
    private int kernelID;
    private RenderTexture renderTexture;
    private int textureWidth = 128;
    private int textureHeight = 128;

    [SerializeField]
    private Renderer backgroundRenderer;
    [SerializeField]
    private float minClamp = -0.5f;
    [SerializeField]
    private float maxClamp = 0.5f;

    private float previousX;
    private float previousY;

    [SerializeField]
    private VRTK_InteractableObject interactableObject;

    private void Awake()
    {
        InitialiseComputeShader();

        interactableObject.InteractableObjectGrabbed += PickerGrabbed;
        interactableObject.InteractableObjectUngrabbed += PickerUngrabbed;
    }

    private void PickerGrabbed(object sender, InteractableObjectEventArgs e)
    {
        DataLogger.Instance.LogActionData(this, GetComponentInParent<Panel>().OriginalOwner, GetComponentInParent<Panel>().OriginalOwner, gameObject.name + " Drag start");
    }

    private void PickerUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        DataLogger.Instance.LogActionData(this, GetComponentInParent<Panel>().OriginalOwner, GetComponentInParent<Panel>().OriginalOwner, gameObject.name + " Drag end");
    }

    private void InitialiseComputeShader()
    {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(textureWidth, textureHeight, 0, RenderTextureFormat.RGB111110Float);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }

        kernelID = computeShader.FindKernel("CSMain");

        backgroundRenderer.material.mainTexture = renderTexture;
    }

    private void GenerateSVTexture(float hue)
    {
        computeShader.SetTexture(kernelID, "Texture", renderTexture);
        computeShader.SetFloats("TextureSize", textureWidth, textureHeight);
        computeShader.SetFloat("Hue", hue);

        var threadGroupsX = Mathf.CeilToInt(textureWidth / 32f);
        var threadGroupsY = Mathf.CeilToInt(textureHeight / 32f);
        computeShader.Dispatch(kernelID, threadGroupsX, threadGroupsY, 1);
    }

    public void SetHue(float value)
    {
        GenerateSVTexture(value);
    }

    public void SetSaturation(float value)
    {
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Lerp(minClamp, maxClamp, value);
        transform.localPosition = pos;

        previousX = pos.x;

        SaturationChanged.Invoke(GetSaturation());
    }

    public void SetBrightness(float value)
    {
        Vector3 pos = transform.localPosition;
        pos.y = Mathf.Lerp(minClamp, maxClamp, value);
        transform.localPosition = pos;

        previousY = pos.y;

        BrightnessChanged.Invoke(GetBrightness());
    }

    public float GetSaturation()
    {
        return (transform.localPosition.x - minClamp) / (maxClamp - minClamp);
    }

    public float GetBrightness()
    {
        return (transform.localPosition.y - minClamp) / (maxClamp - minClamp);
    }

    private void FixedUpdate()
    {
        Vector3 pos = transform.localPosition;

        pos.x = Mathf.Clamp(transform.localPosition.x, minClamp, maxClamp);
        pos.y = Mathf.Clamp(transform.localPosition.y, minClamp, maxClamp);
        pos.z = 0;

        transform.localPosition = pos;

        if (pos.x != previousX)
        {
            previousX = pos.x;
            SaturationChanged.Invoke(GetSaturation());
        }

        if (pos.y != previousY)
        {
            previousY = pos.y;
            BrightnessChanged.Invoke(GetBrightness());
        }
    }
}
