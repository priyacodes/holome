using UnityEngine.XR.ARFoundation;
using UnityEngine.Rendering;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class ARAmbientLight : MonoBehaviour
{
    //new Light light;

    Light light;

    [SerializeField]
    [Tooltip("The ARCameraManager which will produce frame events containing light estimation information.")]
    ARCameraManager m_CameraManager;

    public void Start()
    {
        light = GetComponent<Light>();
        m_CameraManager.frameReceived += UpdateLightEstimation;
    }

    void UpdateLightEstimation(ARCameraFrameEventArgs eventArgs)
    {
        // Unity ambient intensity ranges 0-8 (for over-bright lights)
        if (eventArgs.lightEstimation.averageBrightness.HasValue)
        {
            float newLightValue = eventArgs.lightEstimation.averageBrightness.Value;
            light.intensity = Mathf.Clamp(newLightValue * 2, 0, 1.25f);
            print($"Brightness changed {eventArgs.lightEstimation.averageBrightness.Value} new value = {light.intensity}");
        }

        if (eventArgs.lightEstimation.averageColorTemperature.HasValue)
        {
            light.colorTemperature = eventArgs.lightEstimation.averageColorTemperature.Value;
        }
    }

    void OnDestroy()
    {
        m_CameraManager.frameReceived -= UpdateLightEstimation;
    }
}
