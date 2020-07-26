using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public abstract class PlacementHandler : MonoBehaviour
{

    protected static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    public ARRaycastManager m_RaycastManager;
    public Action<Vector3> OnPlaceDetected { protected get; set; }

}
