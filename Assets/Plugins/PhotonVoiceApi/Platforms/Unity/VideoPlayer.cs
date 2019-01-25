using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class VideoPlayer : MonoBehaviour
{
    Texture2D tex2d;
    public Texture2D GetVideoTexture()
    {
        return tex2d;
    }

    void Start()
    {
        Debug.LogFormat("VDe: Start...");

        // placeholder
        tex2d = new Texture2D(1, 1, TextureFormat.RGBA32, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (bufReady)
        {
            if (tex2d.width != lastDrawWidth || tex2d.height != lastDrawHeight)
            {
                tex2d = new Texture2D((int)lastDrawWidth, (int)lastDrawHeight, TextureFormat.RGBA32, false);
            }
            tex2d.LoadRawTextureData(buf);
            tex2d.Apply();
            bufReady = false;
        }
    }

    volatile bool bufReady;
    volatile int lastDrawWidth;
    volatile int lastDrawHeight;
    byte[] buf = new byte[4]; // as tex2d inited
    public void Draw(IntPtr bufPtr, int w, int h, int stride)
    {        
        if (tex2d == null) // no inited yet
            return;
        if (bufPtr == IntPtr.Zero)
            return;
        if (!bufReady)
        {
            if (lastDrawWidth != w || lastDrawHeight != h)
            {
                lastDrawWidth = w;
                lastDrawHeight = h;
                buf = new byte[w * h * 4];
            }
            Marshal.Copy(bufPtr, buf, 0, buf.Length);
            bufReady = true;
        }
    }

    void OnDestroy()
    {
    }
}
