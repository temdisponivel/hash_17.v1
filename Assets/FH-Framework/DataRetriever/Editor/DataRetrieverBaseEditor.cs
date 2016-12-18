using UnityEngine;
using System.Collections;
using UnityEditor;

namespace FH.DataRetrieving
{
	[CustomEditor(typeof(DataRetrieverBase))]
	public class DataRetrieverBaseEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var config = (DataRetrieverBase) target;

			config.WebServiceUrl = EditorGUILayout.TextField ("Web Service URL", config.WebServiceUrl);
			config.Password = EditorGUILayout.TextField ("Pass Code", config.Password);
			config.MaxWaitTime = EditorGUILayout.FloatField ("Max Wait Time", config.MaxWaitTime);

			if (NGUIEditorTools.DrawHeader ("Localization"))
			{
				config.LocalizationSpreadSheetId = EditorGUILayout.TextField ("Localization Sheet Id", config.LocalizationSpreadSheetId);
				EditorGUILayout.PropertyField (serializedObject.FindProperty ("Sheets"), true);
				EditorGUILayout.PropertyField (serializedObject.FindProperty ("Languages"), true);
				if (GUILayout.Button ("Fetch all localization data"))
					DataRetrieverInstanceBase.Instance.FetchLocalizationInfo (config.LocalizationSpreadSheetId, config.Sheets, config.Languages);
			}
		}
	}
}