using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
    public delegate void OnSwiped(Vector3 delta);
    public delegate void OnPinched(float value);

    public OnSwiped WhenSwiped;
    public OnPinched WhenPinched;

    //SwipeVariables
    private Vector3 lastInputPos, currentInputPos;
    private Vector3 swipeDelta;
    private bool canDetectSwipe = false;

    //ISEditor
    public bool isEditor = false;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isEditor)
        {
            DetectEditorSwipe();
        }
        else
        {
            DetectDeviceSwipe();
            DetectDevicePinch();
        }
    }

    void DetectEditorSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastInputPos = Input.mousePosition;
            canDetectSwipe = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            canDetectSwipe = false;
            lastInputPos = Vector3.zero;
            currentInputPos = Vector3.zero;
        }


        if (canDetectSwipe)
        {
            currentInputPos = Input.mousePosition;
            swipeDelta = currentInputPos - lastInputPos;
            if (WhenSwiped != null)
            {
                WhenSwiped.Invoke(swipeDelta);
            }
            lastInputPos = currentInputPos;
        }

    }

    void DetectDeviceSwipe()
    {
        if (Input.touchCount == 1)
        {
            switch (Input.touches[0].phase)
            {
                case TouchPhase.Began:
                    lastInputPos = Input.touches[0].position;
                    canDetectSwipe = true;
                    break;
                case TouchPhase.Ended:
                    canDetectSwipe = false;
                    lastInputPos = Vector3.zero;
                    currentInputPos = Vector3.zero;
                    break;
                case TouchPhase.Moved:
                    if (canDetectSwipe)
                    {
                        currentInputPos = Input.touches[0].position;
                        swipeDelta = currentInputPos - lastInputPos;
                        if (WhenSwiped != null)
                        {
                            WhenSwiped.Invoke(swipeDelta);
                        }
                        lastInputPos = currentInputPos;
                    }
                    break;
                case TouchPhase.Canceled:
                    canDetectSwipe = false;
                    lastInputPos = Vector3.zero;
                    currentInputPos = Vector3.zero;
                    break;
            }
        }
        else
        {
            canDetectSwipe = false;
            lastInputPos = Vector3.zero;
            currentInputPos = Vector3.zero;
        }

    }
    void DetectDevicePinch()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            if (WhenPinched != null)
            {
                WhenPinched.Invoke(-deltaMagnitudeDiff);
            }

        }
    }
}



