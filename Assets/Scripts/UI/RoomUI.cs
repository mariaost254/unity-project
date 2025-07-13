using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.UI.VirtualMouseInput;

public class RoomUI : MonoBehaviour
{

    [SerializeField] private UIDocument uiDocument;

    private DropdownField colorDrop;
    private DropdownField designerDrop;
    private DropdownField priceDrop;
    private DropdownField floorDrop;
    private DropdownField furnishDrop;
    private DropdownField vpDrop;
    private VisualElement root;
    private VisualElement backBtn;

    private Label roomTitleLabel;

    public CanvasGroup mainLayoutCanvasGroup;
    void Start()
    {
         root = uiDocument.rootVisualElement;
        roomTitleLabel = root.Q<Label>("title");
        backBtn = root.Q<VisualElement>("back-btn");
        colorDrop = root.Q<VisualElement>("color")?.Q<DropdownField>();
        designerDrop = root.Q<VisualElement>("designer")?.Q<DropdownField>();
        priceDrop = root.Q<VisualElement>("price")?.Q<DropdownField>();
        floorDrop = root.Q<VisualElement>("floor")?.Q<DropdownField>();
        furnishDrop = root.Q<VisualElement>("furnishing")?.Q<DropdownField>();
        vpDrop = root.Q<VisualElement>("viewpoint")?.Q<DropdownField>();

        colorDrop?.RegisterValueChangedCallback(_ => Room360Controller.Instance.UpdateRoomImage());
        designerDrop?.RegisterValueChangedCallback(_ => Room360Controller.Instance.UpdateRoomImage());
        priceDrop?.RegisterValueChangedCallback(_ => Room360Controller.Instance.UpdateRoomImage());
        floorDrop?.RegisterValueChangedCallback(_ => Room360Controller.Instance.UpdateRoomImage());
        furnishDrop?.RegisterValueChangedCallback(_ => Room360Controller.Instance.UpdateRoomImage());
        vpDrop?.RegisterValueChangedCallback(_ => Room360Controller.Instance.UpdateRoomImage());

        backBtn?.RegisterCallback<ClickEvent>(OnClickEvent);
        if (Room360Controller.Instance.currentRoom != null)
        {
            roomTitleLabel.text = $"{Room360Controller.Instance.currentRoom.nameRoom} View";
        }
    }

    private void OnClickEvent(ClickEvent evt)
    {
        StartCoroutine(SwitchToRoomView());
    }

    private IEnumerator SwitchToRoomView()
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
