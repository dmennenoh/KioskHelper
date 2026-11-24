
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

/*
* WebCamPhoto
* This is attached to the WebCamPhoto prefab
*
* Displays a webcam feed on a RawImage and captures photos as PNG.
* Supports optional flash, and an animated countdown timer
*
* Suports PNG photo retrieval, Base64 encoding, and saving the photos to disk.
*/

public class WebCamPhoto : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private CanvasGroup flashImage;
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private CanvasGroup counter;
    [SerializeField] private int startCountAt = 3;

    [Header("Camera")]
    [Tooltip("Leave empty to use the first available device.")]
    [SerializeField] private string deviceName = "";
    [SerializeField] private int requestedWidth = 1920;
    [SerializeField] private int requestedHeight = 1080;
    [SerializeField] private int requestedFPS = 30;
    [SerializeField] private bool showCameraOnStart = true;
    [SerializeField] private bool mirrorHorizontally = true;

    [Header("Counter Animation")]
    [SerializeField] private bool fadeCounter= true;
    [SerializeField] private bool scaleCounter = true;
    [SerializeField] private float scaleEndSize = .2f;

    public WebCamTexture WebCam { get; private set; }
    public bool IsReady => WebCam != null && WebCam.isPlaying && WebCam.width > 16;
    public bool HasPhoto => _photoData != null;

    public UnityEvent CameraStarted;
    public UnityEvent<string> CameraFailed;
    public UnityEvent PhotoCaptured;

    private Color32[] _pixelBuffer;
    private byte[] _photoData;   
    private bool isCapturing = false;
    private int count;//3-2-1
    private bool useFlash;//used when showing the countdown
    private Vector3 counterStartScale;


    private void Start()
    {
        //Display available camera names in the console.
        foreach (var d in WebCamTexture.devices)
            Debug.Log($"Webcam: \"{d.name}\"  frontFacing={d.isFrontFacing}");

        if (mirrorHorizontally && rawImage != null)
        {
            Vector3 scale = rawImage.rectTransform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            rawImage.rectTransform.localScale = scale;
        }

        counterStartScale = counter.transform.localScale;

        if (showCameraOnStart)
            StartCamera();
    }


    private void OnDisable()
    {
        if (flashImage != null)
            flashImage.DOKill();

        StopCamera();
    }


    //Starts the webcam.
    public void StartCamera()
    {
        StartCoroutine(StartCameraRoutine());
    }


    //Stops the webcam.
    public void StopCamera()
    {
        if (WebCam != null)
        {
            WebCam.Stop();
            Destroy(WebCam);
            WebCam = null;
        }

        if (rawImage != null)
            rawImage.texture = null;
    }

    //Captures a photo without flash.
    public void TakePhoto(bool flash)
    {
        if(isCapturing) return;
        isCapturing = true;
        CapturePhoto(flash);
    }

    public void TakeTimedPhoto(bool flash)
    {
        if(isCapturing) return;
        isCapturing = true;
        count = startCountAt;
        useFlash = flash;
        nextCount();
    }

    //Returns the last captured photo as a PNG byte array
    //Returns null if no photo was captured
    public byte[] GetPhotoPNG()
    {
        return _photoData;
    }

    //Returns the last captured photo as a B64 encoded string
    //Returns null if no photo was captured
    public string GetPhotoBase64()
    {
        return _photoData == null ? null : Convert.ToBase64String(_photoData);
    }

    //Saves the last captured photo to disk
    //Pass null for a timestamped file in persistentDataPath
    public string SavePhoto(string path = null)
    {
        if (_photoData == null) return null;

        if (string.IsNullOrEmpty(path))
        {
            path = Path.Combine(Application.persistentDataPath, $"webcam_{DateTime.Now:yyyyMMdd_HHmmss}.png");
        }

        File.WriteAllBytes(path, _photoData);
        return path;
    }

    private void nextCount()
    {  
        if(count == 0)
        {
            counter.alpha = 0;
            CapturePhoto(useFlash);            
        }
        else
        {
            counterText.text = count.ToString();
            counter.alpha = 1;
            counter.transform.localScale = counterStartScale;

            Sequence seq = DOTween.Sequence();
            if (fadeCounter) seq.Join(counter.DOFade(0, 1f));
            if (scaleCounter) seq.Join(counter.transform.DOScale(scaleEndSize, 1f));

            //still wait 1 sec if no animation
            if (!fadeCounter && !scaleCounter) seq.AppendInterval(1f);

            seq.OnComplete(() =>
            {
                count--;
                nextCount();
            });
        }             
    }


    private void CapturePhoto(bool useFlash)
    {
        if (!IsReady) return;
        
        _photoData = null;

        if (useFlash && flashImage != null)
        {
            flashImage.DOKill();
            flashImage.alpha = 1;
            flashImage.DOFade(0, 1f).SetDelay(.1f);
        }

        DOVirtual.DelayedCall(useFlash ? .05f : 0f, () =>
        {
            if (!IsReady) return;
            _photoData = CaptureCurrentFrame();
            
            if (_photoData == null) CameraFailed?.Invoke("Capture was null");

            PhotoCaptured?.Invoke();
            isCapturing = false;
        });
    }

    private IEnumerator StartCameraRoutine()
    {
        var devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            CameraFailed?.Invoke("No webcam found.");
            yield break;
        }

        string device = devices[0].name;

        if (!string.IsNullOrEmpty(deviceName))
        {
            var match = Array.Find(devices, d =>
                d.name.IndexOf(deviceName, StringComparison.OrdinalIgnoreCase) >= 0);

            if (!string.IsNullOrEmpty(match.name))
                device = match.name;
        }

        WebCam = new WebCamTexture(device, requestedWidth, requestedHeight, requestedFPS);

        if (rawImage != null)
            rawImage.texture = WebCam;

        WebCam.Play();

        //Wait for a real frame...
        float timeout = 10f;
        while (WebCam != null && WebCam.width <= 16 && timeout > 0f)
        {
            timeout -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (!IsReady)
        {
            CameraFailed?.Invoke("Webcam failed to start.");
            StopCamera();
            yield break;
        }

        CameraStarted?.Invoke();
    }


    //Captures the current webcam frame internally as a PNG byte array
    private byte[] CaptureCurrentFrame()
    {
        if (!IsReady) return null;

        int w = WebCam.width;
        int h = WebCam.height;

        if (_pixelBuffer == null || _pixelBuffer.Length != w * h)
            _pixelBuffer = new Color32[w * h];

        WebCam.GetPixels32(_pixelBuffer);

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

        try
        {
            tex.SetPixels32(_pixelBuffer);
            tex.Apply();

            return tex.EncodeToPNG();
        }
        finally
        {
            Destroy(tex);
        }
    }
}
