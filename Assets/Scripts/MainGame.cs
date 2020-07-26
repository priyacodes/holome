using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;

public class MainGame : MonoBehaviour
{
       
    
    private bool isGameStarted;

    [SerializeField]
    private GameObject normalCanvas;
    [SerializeField]
    private GameObject calibrationCanvas;
    [SerializeField]
    private GameObject PlaceButtonBG;
    [SerializeField]
    private GameObject MoveImage;
    [SerializeField]
    private GameObject UIIconsPanel;
    [SerializeField]
    private VideoClip holoVid;
    [SerializeField]
    private VideoPlayer vidPlayer;

    [Header("Buttons")]
    [SerializeField]
    Button initButton;
    //[SerializeField]
    //Button btnToggleScaleFactor;
    [SerializeField]
    Button pauseButton;
    [SerializeField]
    Button resumeButton;
    [SerializeField]
    Button PlayLocalButton;
    [SerializeField]
    Button PlayStreamButton;

    string streamURL = "http://www.youtube.com/watch?v=yYjqJvd8lQI";
    bool SessionStarted = false;
    Material quadmat;

    [Header("AR")]
    [SerializeField]
    private ARFoundationCalibration aRFoundationCalibration;
    [SerializeField]
    private TouchControl touchControl;
    [SerializeField]
    private Transform arRoot;
    [SerializeField]
    Camera arCamera;
    [SerializeField]
    private Transform dummy;
    [SerializeField]
    private Transform actual;
    [SerializeField]
    private LookAt lookAt;
    [SerializeField]
    private GameObject quad;


    private void Awake()
    {
        quadmat = quad.GetComponent<MeshRenderer>().material;
        initButton.onClick.AddListener(SessionInit);
        pauseButton.onClick.AddListener(onClickedPause);
        resumeButton.onClick.AddListener(onClickedResume);
        PlayLocalButton.onClick.AddListener(OnClickedPlayLocalButton);
        PlayStreamButton.onClick.AddListener(OnClickedPlayStreamButton);
    }
    void Start()
    {
      EnableCalibrationCanvas(true);
      StartCalibration();
    }


    // Session Initialisation
    private void SessionInit()
    {
        if( SessionStarted == false)
        {
            SessionStarted = true;
            initButton.image.color = Color.blue;
            initButton.GetComponentInChildren<Text>().text = "Deinitialize Session";
            vidPlayer.Play();
        }

        else
        {
            SessionStarted = false;
            initButton.image.color = Color.red;
            initButton.GetComponentInChildren<Text>().text = "Initialize Session";
            vidPlayer.Stop();
        }
        
    }

    
    public void EnableCalibrationCanvas(bool state)
    {
        normalCanvas.SetActive(!state);
        calibrationCanvas.SetActive(state);
    }

    public void StartCalibration()
    {
        aRFoundationCalibration.OnPlaneFound += OnPlaneFound;
        aRFoundationCalibration.OnPlaneNotFound += OnPlaneNotFound;
        touchControl.WhenSwiped += RotateARRoot;
        touchControl.WhenPinched += ScaleARRoot;

        dummy.gameObject.SetActive(false);
        actual.gameObject.SetActive(false);

        aRFoundationCalibration.StartCalibration();
        touchControl.enabled = true;
        lookAt.enabled = true;
    }

    private void StopCalibration()
    {
        aRFoundationCalibration.OnPlaneFound -= OnPlaneFound;
        aRFoundationCalibration.OnPlaneNotFound -= OnPlaneNotFound;
        touchControl.WhenSwiped -= RotateARRoot;
        touchControl.WhenPinched -= ScaleARRoot;

        aRFoundationCalibration.StopCalibration();
        touchControl.enabled = false;
        lookAt.enabled = false;
    }

    private void OnPlaneFound(object sender, ARFoundationCalibration.OnPlaneFoundEventArgs args)
    {
        arRoot.position = args.hitPosition;

        //arRoot.rotation = args.hitRotation;
        dummy.gameObject.SetActive(true);

        //set interactable of placebtn set to true
       
        PlaceButtonBG.SetActive(true);
        MoveImage.SetActive(false);

    }


    private void OnPlaneNotFound(object sender, EventArgs args)
    {
        dummy.gameObject.SetActive(false);
        //set interactable of placebtn set to false
       
        PlaceButtonBG.SetActive(false);
        MoveImage.SetActive(true);
    }

    public void PlaceObject()
    {
        dummy.gameObject.SetActive(false);
        actual.gameObject.SetActive(true);
        StopCalibration();
        UIIconsPanel.SetActive(true);
        EnableCalibrationCanvas(false);
            
        OnClickedPlayLocalButton();
        SessionInit();
    }

    public void OnClickedPlayLocalButton()
    {
       // vidPlayer.Stop();
        vidPlayer.source  = VideoSource.VideoClip;
        vidPlayer.clip = holoVid;
        vidPlayer.Play();

    }

    public void OnClickedPlayStreamButton()
    {
       // vidPlayer.Stop();
        vidPlayer.source  = VideoSource.Url;
        vidPlayer.url = streamURL;
        vidPlayer.Play();
    }
    
    public void onClickedPause()
    {
        vidPlayer.Pause();
    }

    public void onClickedResume()
    {
        vidPlayer.Play();
    }

    
    public void OnClickedReScan()
    {
        EnableCalibrationCanvas(true);
        StartCalibration();
               
    }

    public void ScaleQuad(Slider scaleSlider)
    {
        quad.transform.localScale = new Vector3(scaleSlider.value, scaleSlider.value + 5, scaleSlider.value);
    }

    public void Exit()
    {
        Application.Quit();
    }

    
    public void RotateARRoot(Vector3 delta)
    {
        //if (Input.touchCount != 1)
        //    return;

        lookAt.offset += delta.x * -.1f;
        //arRoot.transform.Rotate(new Vector3(0, delta.x * -.1f, 0));
    }

    public void ScaleARRoot(float delta)
    {
        float min = .1f;
        float max = .25f;
        float currentScale = arRoot.transform.localScale.x + (delta * .0005f);

        currentScale = Mathf.Clamp(currentScale, min, max);
        arRoot.transform.localScale = Vector3.one * currentScale;
    }

    private void Update()
    {
        HandleOrientation();

        if (vidPlayer.isPlaying || vidPlayer.isPaused)
            quadmat.SetFloat("_t", 1);
        else
            quadmat.SetFloat("_t", 0);
    }
    private void HandleOrientation()
    {
            var cameraForwardBearing = new Vector3(arCamera.transform.forward.x, -90, arCamera.transform.forward.z).normalized;
            quad.transform.rotation = Quaternion.Slerp(quad.transform.rotation, Quaternion.LookRotation(cameraForwardBearing), Time.deltaTime * 15f);
       
    }

}