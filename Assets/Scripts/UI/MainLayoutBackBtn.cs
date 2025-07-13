using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using System.Collections;

public class MainLayoutController : MonoBehaviour, IPointerClickHandler
{
    public CanvasGroup mainLayoutCanvasGroup;
    public UIDocument mainViewUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(SwitchToMain());
    }

    private IEnumerator SwitchToMain()
    { 
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, 0.5f));
        mainLayoutCanvasGroup.gameObject.SetActive(false);
        var root = mainViewUI.rootVisualElement;
        root.style.display = DisplayStyle.Flex;
    }

    private IEnumerator FadeCanvasGroup(float from, float to, float duration)
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
