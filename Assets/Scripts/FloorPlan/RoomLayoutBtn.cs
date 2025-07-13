using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class RoomLayoutBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerMoveHandler
{
    public string id;
    public GameObject tooltipObject;
    private CanvasGroup canvasGroup;
    public TextMeshProUGUI tooltipText;
    private Coroutine fadeCoroutine;

    public Texture2D customCursorTexture;
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    public CanvasGroup mainLayoutCanvasGroup;
    public UIDocument roomViewUI;
    private VisualElement root;
    private bool isPointerOver = false;

    private void Awake()
    {
        canvasGroup = tooltipObject.GetComponent<CanvasGroup>();
        if (!canvasGroup)
            canvasGroup = tooltipObject.AddComponent<CanvasGroup>();

        tooltipObject.SetActive(true);
        canvasGroup.alpha = 0f;
        roomViewUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void Start()
    {
        if (roomViewUI != null)
            root = roomViewUI.rootVisualElement;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        HideTooltip();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (isPointerOver)
            ShowTooltip();
    }

    private void ShowTooltip()
    {
        if (tooltipText == null) return;

        tooltipText.text = gameObject.name;
        StartFade(1f, 0.1f);

        if (customCursorTexture != null)
            UnityEngine.Cursor.SetCursor(customCursorTexture, hotspot, cursorMode);
    }

    private void HideTooltip()
    {
        StartFade(0f, 0.1f);
        UnityEngine.Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    private void StartFade(float targetAlpha, float duration)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeTo(targetAlpha, duration));
    }

    private IEnumerator FadeTo(float target, float duration)
    {
        float start = canvasGroup.alpha;
        float time = 0f;

        if (target > 0f && !tooltipObject.activeSelf)
            tooltipObject.SetActive(true);

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }

        canvasGroup.alpha = target;

        if (Mathf.Approximately(target, 0f))
            tooltipObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HideTooltip();
        StartCoroutine(SwitchToRoomView(id));
    }

    private IEnumerator SwitchToRoomView(string roomId)
    {
        yield return Room360Controller.Instance.InitRoom(roomId).AsIEnumerator();
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, 0.5f));
        mainLayoutCanvasGroup.gameObject.SetActive(false);
        roomViewUI.rootVisualElement.style.display = DisplayStyle.Flex;
        UnityEngine.Cursor.SetCursor(null, Vector2.zero, cursorMode);
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

public static class TaskExtensions
{
    public static IEnumerator AsIEnumerator(this Task task)
    {
        while (!task.IsCompleted)
            yield return null;

        if (task.IsFaulted)
            Debug.LogException(task.Exception);
    }
}


