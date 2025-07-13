using UnityEditor;
using UnityEngine;

public class BuildAssetBundles
{
    [MenuItem("Tools/Build All Room AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string outputPath = Application.streamingAssetsPath;
        if (!System.IO.Directory.Exists(outputPath))
            System.IO.Directory.CreateDirectory(outputPath);

        Debug.Log("Starting AssetBundle build...");

        BuildPipeline.BuildAssetBundles(outputPath,
            BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.StandaloneWindows64);

        Debug.Log("Finished AssetBundle build.");
    }
}
