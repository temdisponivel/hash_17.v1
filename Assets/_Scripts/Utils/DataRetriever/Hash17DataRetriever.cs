#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Hash17.Utils
{
    [CreateAssetMenu(fileName = "Hash17DataRetriever", menuName = "Hash17/Data retriever")]
    public class Hash17DataRetriever : ScriptableObject
    {
        #region Config Variables

        public string WebServiceUrl;
        public string Password;
        public float MaxWaitTime = 10f;

        #endregion

        #region Lozalization

        public string LocalizationSpreadSheetId;
        public string[] Sheets;
        public SystemLanguage[] Languages;

        #endregion

        #region ProgramsByCommand

        public string ProgramsSpreadSheetId;

        #endregion

        #region Devices

        public string DevicesSpreadSheetId;

        #endregion

        #region Text assets

        public string TextAssetsSpreadSheetId;

        #endregion

        #region Campaign

        public string CampaignMissionsSpreadSheetId;

        #endregion

#if UNITY_EDITOR

        public static T CreateAsset<T>(string destinationPath, string name = "") where T : ScriptableObject
        {
            var folders = destinationPath.Split('/');
            var folderFullPath = "Assets/";
            for (var i = 1; i < folders.Length - 1; i++)
            {
                if (!Directory.Exists(folderFullPath + folders[i]))
                    Directory.CreateDirectory(folderFullPath + folders[i]);

                folderFullPath += folders[i] + "/";
            }

            var scriptableObject = ScriptableObject.CreateInstance<T>();
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(destinationPath + ((name != "") ? name : ("New " + typeof(T).ToString() + ".asset")));
            AssetDatabase.CreateAsset(scriptableObject, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            return (T)AssetDatabase.LoadAssetAtPath(destinationPath + name, typeof(T));

        }

#endif
    }
}
#endif