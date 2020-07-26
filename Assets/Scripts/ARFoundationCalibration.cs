using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System;


public class ARFoundationCalibration : MonoBehaviour
{
    [SerializeField]
    private ARRaycastManager arRaycastManager;
    [SerializeField]
    private ARPlaneManager arPlaneManager;

    [SerializeField]
    private bool canCalibrate;

    private bool foundPlane;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public class OnPlaneFoundEventArgs : EventArgs
    {
        public Vector3 hitPosition;
        public Quaternion hitRotation;
    }

    public EventHandler<OnPlaneFoundEventArgs> OnPlaneFound;
    public EventHandler OnPlaneNotFound;


    void Start()
    {

    }

    void Update()
    {
        if (!canCalibrate)
            return;

        //raycast from center
        if (arRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds))
        {
            foundPlane = true;
            Vector3 position = hits[0].pose.position;
            Quaternion rotation = hits[0].pose.rotation;
            OnPlaneFound?.Invoke(this,
            new OnPlaneFoundEventArgs
            {
                hitPosition = position,
                hitRotation = rotation
            });
        }
        else
        {
            foundPlane = false;
            OnPlaneNotFound?.Invoke(this, new EventArgs());
        }
    }

    public void StartCalibration()
    {
        canCalibrate = true;
        this.enabled = true;

        arPlaneManager.enabled = true;
        SetAllPlanesActive(true);
    }

    public void StopCalibration()
    {
        canCalibrate = false;
        this.enabled = false;

        arPlaneManager.enabled = false;
        SetAllPlanesActive(false);
    }

    public void ReCalibration()
    {
        StartCalibration();
    }

    public void SetAllPlanesActive(bool active)
    {
        foreach (var plane in arPlaneManager.trackables)
            plane.gameObject.SetActive(active);
    }
}
