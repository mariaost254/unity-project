using UnityEditor;
using UnityEngine;
using System.IO;

public class AssetBundleAutoAssigner
{
    [MenuItem("Tools/Assign AssetBundles By Folder")]
    public static void AssignBundles()
    {
        string rootPath = "Assets/RoomImages";
        var dirs = Directory.GetDirectories(rootPath);

        foreach (string dir in dirs)
        {
            string bundleName = Path.GetFileName(dir);
            string[] files = Directory.GetFiles(dir);

            foreach (string file in files)
            {
                if (file.EndsWith(".meta")) continue;

                string assetPath = file.Replace("\\", "/");
                var importer = AssetImporter.GetAtPath(assetPath);
                if (importer != null)
                {
                    importer.assetBundleName = bundleName;
                    Debug.Log($"Assigned {assetPath} to bundle: {bundleName}");
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
