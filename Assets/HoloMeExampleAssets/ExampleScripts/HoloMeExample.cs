using UnityEngine;
using HoloMeSDK;
using UnityEngine.Video;
using UnityEngine.UI;

public class HoloMeExample : MonoBehaviour
{
    [SerializeField]
    Transform targetCameraTransform;

    [SerializeField]
    PlacementHandler placementHandler;

    [SerializeField]
    VideoClip videoClip;

    [SerializeField]
    Button btnInitSession;
    [SerializeField]
    Button btnToggleScaleFactor;
    [SerializeField]
    Button btnPause;
    [SerializeField]
    Button btnResume;
    [SerializeField]
    Button btnPlayLocalVideo;
    [SerializeField]
    Button btnStreamVideo;

    [SerializeField]
    Text txtErrorMessage;

    HoloMe holoMe;

    bool scaledUp;

    const string StreamTestURL = "http://188.166.44.23/NapStreamExample.mp4";

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        holoMe = new HoloMe();
        ToggleInitialization();

        holoMe.EnableAmbientLighting();

        holoMe.SetOnVisibleFunction(holoMe.ResumeVideo);
        holoMe.SetOnInvisibleFunction(holoMe.PauseVideo);

        holoMe.OnPlaybackError += DisplayError;
        holoMe.PositionOffset = -0.2f;

        placementHandler.OnPlaceDetected = PlayOnPlace;
        btnInitSession.onClick.AddListener(ToggleInitialization);
        btnToggleScaleFactor.onClick.AddListener(ToggleScale);
        btnPause.onClick.AddListener(holoMe.PauseVideo);
        btnResume.onClick.AddListener(holoMe.ResumeVideo);
        btnPlayLocalVideo.onClick.AddListener(PlayLocalVideo);
        btnStreamVideo.onClick.AddListener(PlayStreamVideo);
    }

    private void PlayOnPlace(Vector3 position)
    {
        holoMe.PlaceVideo(position);
        if (!holoMe.IsPlaying)
        {
            PlayLocalVideo();
        }
    }

    private void DisplayError(string error)
    {
        txtErrorMessage.text = error;
        Invoke("ClearErrorMessage", 3);
    }

    private void ClearErrorMessage()
    {
        txtErrorMessage.text = string.Empty;
    }

    private void PlayLocalVideoIfNotPlaying()
    {
        holoMe.PlayVideo(videoClip);
    }

    private void PlayLocalVideo()
    {
        holoMe.PlayVideo(videoClip);
    }

    private void PlayStreamVideo()
    {
        holoMe.PlayVideo(StreamTestURL);
    }

    private void ToggleInitialization()
    {
        if (holoMe.Initialized)
        {
            btnInitSession.image.color = Color.red;
            btnInitSession.GetComponentInChildren<Text>().text = "Initialize Session";
            holoMe.DeInit();
        }
        else
        {
            btnInitSession.image.color = Color.blue;
            btnInitSession.GetComponentInChildren<Text>().text = "Deinitialize Session";
            holoMe.Init(targetCameraTransform);
        }
    }

    void ToggleScale()
    {
        if (scaledUp)
        {
            holoMe.SetScale(1);
        }
        else
        {
            holoMe.SetScale(2);
        }

        scaledUp = !scaledUp;
    }
}
