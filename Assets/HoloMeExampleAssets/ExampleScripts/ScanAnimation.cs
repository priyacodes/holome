using System.Collections;
using UnityEngine;

public class ScanAnimation : MonoBehaviour
{
    private RectTransform animationTransform;
    private float widthPixelDistance = 50;
    private float moveAddition = 2;
    private float waitTime = 0.05f;

    private void OnEnable()
    {
        if (!animationTransform)
        {
            animationTransform = GetComponent<RectTransform>();
        }

        if (animationTransform != null)
        {
            StartCoroutine(ScanAnimationRoutine());
        }
        else
        {
            Debug.LogError("Scan Animation Failed" + gameObject.name);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator ScanAnimationRoutine()
    {
        while (isActiveAndEnabled)
        {
            while (animationTransform.anchoredPosition.x > -widthPixelDistance)
            {
                animationTransform.anchoredPosition = animationTransform.anchoredPosition -= new Vector2(moveAddition, 0);
                yield return new WaitForSeconds(waitTime);
            }

            while (animationTransform.anchoredPosition.x < widthPixelDistance)
            {
                animationTransform.anchoredPosition = animationTransform.anchoredPosition += new Vector2(moveAddition, 0);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
