using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;

public class Room360Controller : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    private VisualElement root;
    private DropdownField colorDrop;
    private DropdownField designerDrop;
    private DropdownField priceDrop;
    private DropdownField floorDrop;
    private DropdownField furnishDrop;
    private DropdownField vpDrop;
    private Label roomTitleLabel;
    public PanoramaDisplay panoramaSphere;

    public RoomEntity currentRoom;
    public static Room360Controller Instance { get; private set; }
    private Dictionary<string, Texture2D> imageCache = new();

    [SerializeField] GameObject loadingPanel;
    [SerializeField] UnityEngine.UI.Slider slider;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

        if (uiDocument != null)
        {
            root = uiDocument.rootVisualElement;
            roomTitleLabel = root.Q<Label>("title");
            colorDrop = root.Q<VisualElement>("color")?.Q<DropdownField>();
            designerDrop = root.Q<VisualElement>("designer")?.Q<DropdownField>();
            priceDrop = root.Q<VisualElement>("price")?.Q<DropdownField>();
            floorDrop = root.Q<VisualElement>("floor")?.Q<DropdownField>();
            furnishDrop = root.Q<VisualElement>("furnishing")?.Q<DropdownField>();
            vpDrop = root.Q<VisualElement>("viewpoint")?.Q<DropdownField>();
            loadingPanel.AddComponent<CanvasGroup>();
            loadingPanel.SetActive(false);
        }

    }

    private void PopulateDropdown(List<string> values, DropdownField dropdown, VisualElement container)
    {
        if (values == null || values.Count == 0)
        {
            container.style.display = DisplayStyle.None;
            return;
        }

        if (values.Count == 1)
        {
            container.style.display = DisplayStyle.None;
            return;
        }

        container.style.display = DisplayStyle.Flex;

        dropdown.choices = values;
        dropdown.value = values[0];
    }

    public void UpdateRoomImage()
    {
        if (currentRoom == null) return;

        var filteredOptions = currentRoom.Options.AsEnumerable();

        if (root.Q<VisualElement>("color").resolvedStyle.display != DisplayStyle.None)
            filteredOptions = filteredOptions.Where(o => o.Color == colorDrop.value);

        if (root.Q<VisualElement>("designer").resolvedStyle.display != DisplayStyle.None)
            filteredOptions = filteredOptions.Where(o => o.Designer == designerDrop.value);

        if (root.Q<VisualElement>("price").resolvedStyle.display != DisplayStyle.None)
            filteredOptions = filteredOptions.Where(o => o.Price == priceDrop.value);

        if (root.Q<VisualElement>("floor").resolvedStyle.display != DisplayStyle.None)
            filteredOptions = filteredOptions.Where(o => o.Floor == floorDrop.value);

        if (root.Q<VisualElement>("furnishing").resolvedStyle.display != DisplayStyle.None)
            filteredOptions = filteredOptions.Where(o => o.Furnishing == furnishDrop.value);

        if (root.Q<VisualElement>("viewpoint").resolvedStyle.display != DisplayStyle.None)
            filteredOptions = filteredOptions.Where(o => o.Viewpoint.ToString() == vpDrop.value);

        var option = filteredOptions.FirstOrDefault();

        if (option != null && !string.IsNullOrEmpty(option.ImagePath))
        {
            LoadImage(option.ImagePath);
        }
    }

    private void LoadImage(string path)
    {
        if (imageCache.TryGetValue(path, out var tex))
        {
            panoramaSphere.SetPanorama(tex);
        }
        else
        {
            Debug.LogWarning($"Image not found in cache: {path}");
        }
    }

    public async Task InitRoom(string id)
    {
        var room = RoomController.Instance.AllRooms.FirstOrDefault(r => r.id == id);
        currentRoom = room;
        if (room == null) return;
        loadingPanel.SetActive(true);
        slider.value = 0f;
        //StartCoroutine(PreloadRoomImages(room));
        await PreloadRoomImagesAsync(room);

        PopulateDropdown(room.Options.Select(o => o.Color).Distinct().ToList(), colorDrop, root.Q<VisualElement>("color"));
        PopulateDropdown(room.Options.Select(o => o.Designer).Distinct().ToList(), designerDrop, root.Q<VisualElement>("designer"));
        PopulateDropdown(room.Options.Select(o => o.Price).Distinct().ToList(), priceDrop, root.Q<VisualElement>("price"));
        PopulateDropdown(room.Options.Select(o => o.Floor).Distinct().ToList(), floorDrop, root.Q<VisualElement>("floor"));
        PopulateDropdown(room.Options.Select(o => o.Furnishing).Distinct().ToList(), furnishDrop, root.Q<VisualElement>("furnishing"));
        PopulateDropdown(room.Options.Select(o => o.Viewpoint.ToString()).Distinct().ToList(), vpDrop, root.Q<VisualElement>("viewpoint"));
        UpdateTitle();
        LoadImage(room.Options[0].ImagePath);
        loadingPanel.SetActive(false);
    }

    public void UpdateTitle()
    {
        if (roomTitleLabel != null && !string.IsNullOrEmpty(currentRoom?.nameRoom))
            roomTitleLabel.text = $"{currentRoom.nameRoom} View";
    }

    public async Task PreloadRoomImagesAsync(RoomEntity room)
    {
        imageCache.Clear();
        int total = room.Options.Count;
        int loaded = 0;

        string bundleName = room.id;
        string bundlePath = Path.Combine(Application.streamingAssetsPath, bundleName);
        using UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle("file://" + bundlePath);
        var op = request.SendWebRequest();

        while (!op.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
        {
            loadingPanel.SetActive(false);
            return;
        }

        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
        if (bundle == null)
        {
            loadingPanel.SetActive(false);
            return;
        }

        foreach (var option in room.Options)
        {
            if (string.IsNullOrEmpty(option.ImagePath)) continue;
            if (imageCache.ContainsKey(option.ImagePath)) continue;

            string assetName = Path.GetFileNameWithoutExtension(option.ImagePath);
            AssetBundleRequest requestAsset = bundle.LoadAssetAsync<Texture2D>(assetName);
            while (!requestAsset.isDone)
                await Task.Yield();

            if (requestAsset.asset is Texture2D fullTex)
            {
                Texture2D cropped = CropTexture(fullTex, useBottomHalf: true);
                imageCache[option.ImagePath] = cropped;
                loaded++;
                slider.value = (float)loaded / total;
            }
        }

        bundle.Unload(false);
    }

    private Texture2D CropTexture(Texture2D fullTex, bool useBottomHalf)
    {
        int width = fullTex.width;
        int halfHeight = fullTex.height / 2;
        int startY = useBottomHalf ? 0 : halfHeight;

        Texture2D cropped = new Texture2D(width, halfHeight, TextureFormat.RGBA32, false);
        Color[] pixels = fullTex.GetPixels(0, startY, width, halfHeight);
        cropped.SetPixels(pixels);
        cropped.Apply();

        return cropped;
    }
}
