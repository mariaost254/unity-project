using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    private VisualElement root;
    private VisualElement startTourBtn;
    public CanvasGroup mainLayoutCanvasGroup;
    private void Start()
    {
        if (uiDocument != null)
        {
            root = uiDocument.rootVisualElement;
            startTourBtn = root.Q<VisualElement>("start-button");

            startTourBtn?.RegisterCallback<ClickEvent>(OnClickEvent);
            mainLayoutCanvasGroup.alpha = 0f;
            mainLayoutCanvasGroup.gameObject.SetActive(false);
        }
    }

    private void OnClickEvent(ClickEvent evt)
    {
        StartCoroutine(SwitchToFloorPlan());
    }

    private IEnumerator SwitchToFloorPlan()
    {
        mainLayoutCanvasGroup.gameObject.SetActive(true);
        yield return StartCoroutine(FadeinCanvasGroup(0f, 1f, 0.5f));
        root.style.display = DisplayStyle.None;
    }

    private IEnumerator FadeinCanvasGroup(float from, float to, float duration)
    {
        float time = 0f;
        mainLayoutCanvasGroup.alpha = from;

        while (time < duration)
        {
            time += Time.deltaTime;
            mainLayoutCanvasGroup.alpha = Mathf.Lerp(from, to, time / duration);
            yield return null;
        }

        mainLayoutCanvasGroup.alpha = to;
    }

}
