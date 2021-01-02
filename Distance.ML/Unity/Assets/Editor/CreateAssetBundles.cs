using System.IO;
using UnityEditor;

namespace Editor {
    public static class BuildAssetBundles {
        [MenuItem("Assets/Build AssetBundles")]
        public static void Go() {
            const string ASSET_BUNDLE_DIRECTORY = "Assets/AssetBundles";

            if (!Directory.Exists(ASSET_BUNDLE_DIRECTORY))
                Directory.CreateDirectory(ASSET_BUNDLE_DIRECTORY);

            BuildPipeline.BuildAssetBundles(ASSET_BUNDLE_DIRECTORY,
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows);
        }
    }
}
