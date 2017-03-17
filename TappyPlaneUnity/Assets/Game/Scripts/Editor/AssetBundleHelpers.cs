using UnityEditor;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", 
            BuildAssetBundleOptions.None, 
            BuildTarget.StandaloneWindows64);
    }
}