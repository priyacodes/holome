using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class PnlTutorial : MonoBehaviour
{
    Text txtMessage;
    Button btnOK;
    CanvasGroup canvasGroup;

    [SerializeField]
    UnityEvent OnTutorialComplete;

    [SerializeField]
    string[] messages;

    [SerializeField]
    Transform imageParent;

    [SerializeField]
    ARPlaneManager planeManager;

    int tutorialStepIndex = -1;

    bool endReached;

    void Awake()
    {
        txtMessage = GetComponentInChildren<Text>();
        btnOK = GetComponentInChildren<Button>();
        btnOK.onClick.AddListener(() => StartCoroutine(FadeClose()));
        canvasGroup = GetComponent<CanvasGroup>();

        planeManager.enabled = false;
    }

    private void OnEnable()
    {
        ShowNextMessage();
    }

    void ShowNextMessage()
    {
        if (tutorialStepIndex >= messages.Length - 1)
        {
            gameObject.SetActive(false);
            return;
        }
        canvasGroup.alpha = 1;

        tutorialStepIndex += 1;
        txtMessage.text = messages[tutorialStepIndex];

        int childCount = imageParent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            imageParent.GetChild(i).gameObject.SetActive(i == tutorialStepIndex ? true : false);
        }
    }

    IEnumerator FadeClose()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= 0.05f;
            yield return new WaitForSeconds(0.025f);
        }

        if (tutorialStepIndex == 0)
        {
            planeManager.enabled = true;
        }

        if (tutorialStepIndex + 1 == messages.Length)
        {
            OnTutorialComplete?.Invoke();
        }

        gameObject.SetActive(false);
    }

}
