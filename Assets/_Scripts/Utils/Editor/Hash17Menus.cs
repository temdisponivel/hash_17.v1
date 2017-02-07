using System.IO;
using Hash17.Programs;
using MockSystem;
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

        [MenuItem("Hash17/Remove save file", false)]
        public static void RemoveSaveFile()
        {
            System.IO.File.Delete("{0}{1}".InLineFormat(Application.persistentDataPath, Alias.Config.CampaignSavePath));
        }
    }
}