using UnityEngine.UI;
using UnityEngine;
using System;

public class OffScreenIndicator : MonoBehaviour
{
    [SerializeField]
    private Vector2 IndicatorSize;
    private Transform target;
    [SerializeField]
    private Canvas targetCanvas;
    [SerializeField]
    private Camera mainCamera;

    private Vector3 indicatorWorldPos;
    private Vector3 indicatorRotation;
    private bool isOffScreen;


    public class OnOffScreenIndicatorModifiedArgs : EventArgs
    {
        public Transform target;
        public bool isOffScreen;
        public Vector3 indicatorWorldPosition;
        public Vector3 rotation;
        public Camera camera;
    }

    public EventHandler OnOffScreenIndicatorActive;
    public EventHandler OnOffScreenIndicatorInActive;
    public EventHandler<OnOffScreenIndicatorModifiedArgs> OnIndicatorModified;



    void Update()
    {
        if (target != null)
        {
            UpdateState();
        }
    }

    private void UpdateState()
    {
        isOffScreen = false;
        CalculateIndicatorPos(out isOffScreen, out indicatorWorldPos, out indicatorRotation);

        OnIndicatorModified?.Invoke(this, new OnOffScreenIndicatorModifiedArgs
        {
            target = this.target,
            isOffScreen = isOffScreen,
            indicatorWorldPosition = indicatorWorldPos,
            rotation = this.indicatorRotation,
            camera = mainCamera
        });

    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        OnOffScreenIndicatorActive?.Invoke(this, new EventArgs());
    }

    public void RemoveTarget()
    {
        target = null;
        OnOffScreenIndicatorInActive?.Invoke(this, new EventArgs());
    }

    private void CalculateIndicatorPos(out bool isOffScreen, out Vector3 indicatorWorldPos, out Vector3 indicatorRotation)
    {
        Vector3 targetScreenPos = mainCamera.WorldToScreenPoint(target.position);
        if (targetScreenPos.z < 0)
        {
            targetScreenPos *= -1f;
        }

        Vector2 currentIndicatorSize = Vector2.zero;
        currentIndicatorSize.x = IndicatorSize.x * targetCanvas.transform.lossyScale.x;
        currentIndicatorSize.y = IndicatorSize.y * targetCanvas.transform.lossyScale.y;

        if ((targetScreenPos.x < currentIndicatorSize.x || targetScreenPos.x > Screen.width + currentIndicatorSize.x) ||
            ((targetScreenPos.y < currentIndicatorSize.y || targetScreenPos.y > Screen.height + currentIndicatorSize.y)))
        {
            isOffScreen = true;
        }
        else
        {
            isOffScreen = false;
        }

        targetScreenPos.x = Mathf.Clamp(targetScreenPos.x, currentIndicatorSize.x, Screen.width - currentIndicatorSize.x);
        targetScreenPos.y = Mathf.Clamp(targetScreenPos.y, currentIndicatorSize.y, Screen.height - currentIndicatorSize.y);
        targetScreenPos.z = 0;

        indicatorWorldPos = targetScreenPos;
        indicatorRotation = GetIndicatorRotation(targetScreenPos);
    }
    private Vector3 GetIndicatorRotation(Vector3 targetScreenPos)
    {
        Vector3 toPosition = targetScreenPos;
        Vector3 fromPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = GetAngleFromVectorFloat(dir);
        return new Vector3(0, 0, 180 + angle);
    }

    public float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
