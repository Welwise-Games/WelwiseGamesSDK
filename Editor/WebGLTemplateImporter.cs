using UnityEditor;
using UnityEngine;

namespace WelwiseGames.Editor
{
    public static class WebGLTemplateImporter
    {
        [MenuItem("Tools/WelwiseGamesSDK/Add WebGL Template", priority = 1)]
        public static void AddWebGLTemplate()
        {
            var guids = AssetDatabase.FindAssets("welwisesdk-template");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (System.IO.Path.GetFileName(path) != "welwisesdk-template.unitypackage") continue;
                AssetDatabase.ImportPackage(path, true);
                return;
            }
            Debug.LogError("welwisesdk-template.unitypackage not found in project resources.");
        }
    }
}