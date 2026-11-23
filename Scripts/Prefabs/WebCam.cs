using UnityEngine;
using UnityEngine.UI;

public class WebCam : MonoBehaviour
{
    private WebCamTexture webcamTexture;

    void Start()
    {
        RawImage rawImage = GetComponent<RawImage>();

        // Uses the default connected camera
        webcamTexture = new WebCamTexture();

        // Assign the texture and start the feed
        rawImage.texture = webcamTexture;
        webcamTexture.Play();
    }

    void OnDisable()
    {
        // Stop the camera when the object is disabled or scene closes
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}
