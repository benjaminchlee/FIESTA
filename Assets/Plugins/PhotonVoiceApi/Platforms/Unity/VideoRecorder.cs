//#define USE_ImageBufferNativeAlloc 

#if USE_ImageBufferNativeAlloc  // Not good, copies plane on each frame. Provided only as a sample.
using ImageBufferClass = ExitGames.Client.Photon.Voice.ImageBufferNativeAlloc;
#else                           // Better solution because does not copy plane.
using ImageBufferClass = ExitGames.Client.Photon.Voice.ImageBufferNativeGCHandleSinglePlane;
#endif

using UnityEngine;
using Voice = ExitGames.Client.Photon.Voice;
using System.Collections;

public class VideoRecorder : MonoBehaviour
{
    const int MAX_IMAGE_QUEUE = 2;

    bool encoderReady = false;

    float nextEncodingRealtime;

    WebCamTexture webcamTexture;
    Texture2D webcamTexture2D;

    Voice.ImageBufferNativePool<ImageBufferClass> pushImageBufferPool;
    int encoderFPS;
    Voice.Flip verticalFlip;
    public WebCamTexture GetVideoTexture()
    {
        return webcamTexture;
    }

    [RuntimeInitializeOnLoadMethod]
    static public void InitCodec()
    {
        Debug.LogFormat("VEn: VPx Init");
    }

    IEnumerator Start()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            yield break;
        }
    }

    public void StartEncode(Voice.LocalVoiceVideo localVoice, string deviceName, int encoderFPS, bool flipVertically)
    {
        Debug.LogFormat("VEn: StatrEncode");

        this.localVoice = localVoice;
        this.encoderFPS = encoderFPS;
        this.verticalFlip = flipVertically ? Voice.Flip.Vertical : Voice.Flip.None;

        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (var d in devices)
        {
            Debug.LogFormat("VEn: " + (d.isFrontFacing ? "Cam (front): " : "Cam: " + d.name));
        }

        ReleaseWebcamTexture();

        webcamTexture = new WebCamTexture(deviceName);

        // apply texture to current object
        //Renderer renderer = GetComponent<Renderer>();
        //renderer.material.mainTexture = webcamTexture;
        webcamTexture.Play();
        webcamTexture2D = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGBA32, false);
        Debug.LogFormat("VEn: WebCamTexture: {0} x {1}", webcamTexture.width, webcamTexture.height);


        pushImageBufferPool = new Voice.ImageBufferNativePool<ImageBufferClass>(MAX_IMAGE_QUEUE + 1, // 1 more slot for image being processed (neither in queue nor in pool)
            (pool, info) => new ImageBufferClass(pool, info), 
            "VideoRecorder Image",
            new Voice.ImageBufferInfo(webcamTexture.width, webcamTexture.height, new int[] { webcamTexture.width * 4 }, Voice.ImageFormat.ABGR)
            ); 
      
        encoderReady = true;
    }

    public void StopEncode()
    {
        Debug.LogFormat("VEn: StopEncode");
        localVoice = null;
    }

    Voice.LocalVoiceVideo localVoice;
    void Update()
    {
        if (!encoderReady)
            return;

        if (Time.realtimeSinceStartup < nextEncodingRealtime)
        {
            return;
        }
        nextEncodingRealtime = Time.realtimeSinceStartup + 1 / encoderFPS;

        // convert WebCamTexture texture to Texture2D which has GetRawTextureData() method
        var tmpBytes = webcamTexture.GetPixels32();
        webcamTexture2D.SetPixels32(tmpBytes);
        
        if (localVoice != null && localVoice.PushImageQueueCount < MAX_IMAGE_QUEUE)
        {
            var b = pushImageBufferPool.AcquireOrCreate();

#if USE_ImageBufferNativeAlloc
            System.Runtime.InteropServices.Marshal.Copy(webcamTexture2D.GetRawTextureData(), 0, b.Planes[0], tmpBytes.Length * 4);
#else
            b.PinPlane(webcamTexture2D.GetRawTextureData());
#endif
            b.Info.Rotation = Voice.Rotation.Rotate0;
            b.Info.Flip = verticalFlip;

            localVoice.PushImageAsync(b);

            //localVoice.PushImage(b.Planes, b.Info.Width, b.Info.Height, b.Info.Stride, b.Info.Format, b.Info.Rotation, b.Info.Flip);
            //b.Release();
        }
        frame_count++;
    }

    int frame_count;

    public void ReleaseWebcamTexture()
    {
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
            webcamTexture = null;
            Debug.Log("VEn: WebcamTexture stopped and released");
        }
    }

    void OnDestroy()
    {
        ReleaseWebcamTexture();
        if (pushImageBufferPool != null)
        {
            pushImageBufferPool.Dispose();
            pushImageBufferPool = null;
        }
    }
}
