using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FocusSquare : PlacementHandler
{
    [SerializeField]
    Texture2D focusSquareTexture;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("This is the unit distance of how close the user can get to the camera before the square goes transparent")]
    private float transparencyRangeUser = 0.15f;

    [SerializeField]
    [Range(0, 10)]
    [Tooltip("This is the unit distance before the square becomes transparent as it gets closer to the hologram")]
    private float transparencyRangeHologram = 1f;

    [SerializeField]
    [Tooltip("Check this to have the focus square smoothly follow the camera")]
    private bool smoothFollow = true;

    [SerializeField]
    [Range(2, 10)]
    [Tooltip("This determines the speed of the smooth movement")]
    float smoothFollowSpeed = 5;

    [Tooltip("Use this to apply custom shaders, Unlit/Transparent recommended")]
    [SerializeField]
    Shader shader;

    [SerializeField]
    Camera arCamera;

    Material quadMat;
    GameObject quad;

    //[SerializeField]
    //bool stayOnAfterPlace;

    [Tooltip("Use this to force the logo to always display in the correct orientation")]
    [SerializeField]
    bool keepOrientation = true;

    Transform lookTarget;

    Vector3 hologramPlacedPosition;

    [SerializeField]
    UnityEvent OnSurfaceFound;

    [SerializeField]
    UnityEvent OnSurfaceLost;

    bool inAnimation;

    bool surfaceDetected;
    public bool SurfaceDetected
    {
        get => surfaceDetected;
        set
        {
            bool valueChanged = surfaceDetected != value;
            surfaceDetected = value;

            if (surfaceDetected)
            {
                if (valueChanged)
                {
                    //Debug.Log("VALUE CHANGED SURFACE FOUND");
                    StopAllCoroutines();
                    StartCoroutine(FadeHide(false));
                    OnSurfaceFound?.Invoke();
                }
            }
            else
            {
                if (valueChanged)
                {
                    //Debug.Log("VALUE CHANGED NO SURFACE");
                    StopAllCoroutines();
                    StartCoroutine(FadeHide(true));
                    OnSurfaceLost?.Invoke();
                }
            }
        }
    }

    public void Awake()
    {
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "FocusSquare";
        UpdateUnitSize(0.25f);
        quad.transform.Rotate(new Vector3(90, 0, 0));
        Destroy(quad.GetComponent<MeshCollider>());

        quadMat = quad.GetComponent<Renderer>().material;
        quad.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/FocusSquare");
        SetFocusSquareTexture(focusSquareTexture);

        StartCoroutine(FadeHide(true));
        OnSurfaceLost?.Invoke();
    }

    private void SetFocusSquareTexture(Texture2D texture)
    {
        if (texture != null)
        {
            quadMat.SetTexture("_MainTex", texture);
        }
        else if (Application.isEditor)
        {
            Debug.LogError("Passed in focus square texture was null");
        }
    }

    IEnumerator FadeHide(bool hide)
    {
        inAnimation = true;
        var quadMatCurrentColor = quadMat.GetColor("_Color");
        while (hide ? quadMatCurrentColor.a > 0 : quadMatCurrentColor.a < GetAlphaBasedOnDistance())
        {
            quadMat.SetColor("_Color", new Color(quadMatCurrentColor.r, quadMatCurrentColor.g, quadMatCurrentColor.b, quadMatCurrentColor.a -= hide ? 0.05f : -0.05f));
            yield return new WaitForSeconds(0.015f);
        }
        inAnimation = false;
    }

    public void UpdateUnitSize(float unitSize)
    {
        quad.transform.localScale = new Vector3(unitSize, unitSize, quad.transform.localScale.z);
    }

    private float GetAlphaBasedOnDistance()
    {
        var fadeValueUser = FadeBasedOnDistance(arCamera.transform.position, transparencyRangeUser);
        var fadeValueHologram = FadeBasedOnDistance(hologramPlacedPosition, transparencyRangeHologram);
        return Mathf.Min(fadeValueUser, fadeValueHologram);
    }

    public void Hide()
    {
        quad.SetActive(false);
    }

    private void HandleOrientation()
    {
        if (keepOrientation)
        {
            var cameraForwardBearing = new Vector3(arCamera.transform.forward.x, -90, arCamera.transform.forward.z).normalized;
            quad.transform.rotation = Quaternion.Slerp(quad.transform.rotation, Quaternion.LookRotation(cameraForwardBearing), Time.deltaTime * 15f);
        }
    }

    private void HandleDistanceFade()
    {
        if (SurfaceDetected && !inAnimation)
        {
            quadMat.color = new Color(1, 1, 1, GetAlphaBasedOnDistance());
        }
    }

    private float FadeBasedOnDistance(Vector3 fadeTargetPosition, float fullTransparencyRange)
    {
        return Mathf.Clamp(Vector3.Distance(GetZeroYVector(quad.transform.position), GetZeroYVector(fadeTargetPosition)) - fullTransparencyRange, 0, 1);
    }

    private static Vector3 GetZeroYVector(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    private void FollowCamera(Vector3 position)
    {
        if (smoothFollow)
        {
            quad.transform.position = Vector3.Lerp(quad.transform.position, new Vector3(position.x, position.y + 0.1f, position.z), Time.deltaTime * smoothFollowSpeed);
        }
        else
        {
            quad.transform.position = new Vector3(position.x, position.y + 0.1f, position.z);
        }
    }

    private void Update()
    {
        var hits = new List<ARRaycastHit>();
        m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon);

        SurfaceDetected = hits.Count > 0;

        if (hits.Count > 0)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                OnPlaceDetected?.Invoke(hits[0].pose.position);
                hologramPlacedPosition = quad.transform.position;
            }

            FollowCamera(hits[0].pose.position);
        }

        HandleOrientation();
        HandleDistanceFade();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
