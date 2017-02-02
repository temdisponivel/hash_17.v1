using System.IO;
using Hash17.Programs;
using Hash17.Terminal_;
using UnityEditor;
using UnityEngine;

namespace Hash17.Utils
{
    public static class Hash17Menus
    {
        private static void CreateAsset<T>()
            where T : Object, new()
        {
            var asset = new T();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(path + "/MyAsset.asset");
            AssetDatabase.CreateAsset(asset, uniquePath);
            AssetDatabase.Refresh();
        }
    }
}