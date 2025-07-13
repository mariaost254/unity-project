
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public List<RoomEntity> AllRooms = new();
    public static RoomController Instance { get; private set; }

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

    void Start()
    {
        LoadRoomsFromAssetBundleFilenames();
    }

    void LoadRoomsFromAssetBundleFilenames()
    {
        string basePath = Path.Combine(Application.dataPath, "RoomImages");

        if (!Directory.Exists(basePath))
        {
            Debug.LogError("RoomImages folder not found at: " + basePath);
            return;
        }

        var roomDirs = Directory.GetDirectories(basePath);

        foreach (var dir in roomDirs)
        {
            string roomId = Path.GetFileName(dir);
            var room = new RoomEntity { id = roomId };

            string[] files = Directory.GetFiles(dir);
            foreach (var file in files)
            {
                string extension = Path.GetExtension(file).ToLower();
                if (extension != ".jpg" && extension != ".png") continue;

                string fileName = Path.GetFileNameWithoutExtension(file);
                var match = Regex.Match(fileName, @"(\w+)_def_(\w+)_(\w+)_(\w+)_(\w+)_l1_(\w+)_(\d+)");

                if (!match.Success) continue;

                var config = new RoomOptions
                {
                    Color = match.Groups[1].Value,
                    Designer = match.Groups[2].Value,
                    Price = match.Groups[3].Value,
                    Function = match.Groups[4].Value,
                    Floor = match.Groups[5].Value,
                    Furnishing = match.Groups[6].Value,
                    Viewpoint = int.Parse(match.Groups[7].Value),
                    ImagePath = fileName
                };
                room.nameRoom = match.Groups[4].Value;
                room.Options.Add(config);
            }

            if (room.Options.Count > 0)
            {
                AllRooms.Add(room);
            }
        }

    }
}
