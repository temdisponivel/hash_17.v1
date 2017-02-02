using Hash17.Utils;
using UnityEngine;
using System.Collections;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using FH.Localization;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;

namespace FH.DataRetrieving
{
    [ExecuteInEditMode]
    public class DataRetrieverInstanceBase : Singleton<Hash17DataRetrieverInstance>
    {
		#if UNITY_EDITOR

		#region Data Retriever
		
		protected JsonData[] _spreadSheetResults;
		protected DataRetrieverBase _dataRetriever;
		public DataRetrieverBase DataRetriever
		{
			get { return _dataRetriever ?? (_dataRetriever = Resources.LoadAll<DataRetrieverBase>("")[0]); }
		}
        
        #endregion

        #region Fetch localization

        public void FetchLocalizationInfo(string spreadSheetId, string[] sheetName, SystemLanguage[] languages)
		{
			StartCoroutine(RunFetchLocalizationInfos(spreadSheetId, sheetName, languages));
		}

		private IEnumerator RunFetchLocalizationInfos(string spreadSheetId, string[] sheetsName, SystemLanguage[] languages)
		{
			var results = new Dictionary<string, Dictionary<string, string>>(2);
			LocalizationManager.Instance.Languages = new List<SystemLanguage> ();
			LocalizationManager.Instance.Languages.AddRange (languages);

			for (var i = 0; i < languages.Length; i++)
			{
				results[languages[i].ToString()] = new Dictionary<string, string>();
			}

			for (var k = 0; k < sheetsName.Length; k++)
			{
				yield return StartCoroutine(GetData(spreadSheetId, sheetsName[k]));
				yield return null;

				if (_spreadSheetResults == null)
					yield break;

				for (var i = 0; i < _spreadSheetResults.Length; i++)
				{
					var key = _spreadSheetResults[i]["Key"].ToString();
					for (var j = 0; j < languages.Length; j++)
					{
						var currentValue = _spreadSheetResults[i][languages[j].ToString()].ToString();
						if (results[languages[j].ToString()].ContainsKey(key))
						{
							Debug.LogError(string.Format("Duplicated key: {0} in sheet {1}", key, sheetsName[k]));
							continue;
						}
						results[languages[j].ToString()].Add(key, currentValue);
					}
				}
			}

			foreach (var entry in results)
			{
				var path = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}",
					Application.dataPath,
					Path.DirectorySeparatorChar,
					"Resources",
					Path.DirectorySeparatorChar,
					LocalizationManager.PathPrefix,
					Path.DirectorySeparatorChar,
					entry.Key,
					Path.DirectorySeparatorChar,
					"Data.txt");

				var pathPrefix = string.Format("{0}{1}{2}{3}{4}{5}{6}",
					Application.dataPath,
					Path.DirectorySeparatorChar,
					"Resources",
					Path.DirectorySeparatorChar,
					LocalizationManager.PathPrefix,
					Path.DirectorySeparatorChar,
					entry.Key);

				if (!Directory.Exists(pathPrefix))
				{
					Directory.CreateDirectory(pathPrefix);
				}

				var cSharpPathPrefix = string.Format("{0}{1}{2}{3}{4}{5}",
					Application.dataPath,
					Path.DirectorySeparatorChar,
					"FH-Framework",
					Path.DirectorySeparatorChar,
					"Localization",
					Path.DirectorySeparatorChar);

				using (Stream fileStream = File.Open(path, FileMode.Create))
				{
					using (var zip = new StreamWriter(fileStream))
					{
						zip.Write(JsonConvert.SerializeObject(entry.Value));
						CreateCSharpKeysFile(cSharpPathPrefix, "FH.Localization", "LocalizationKeys", entry.Value.Keys.ToArray());
						Debug.Log("File saved: " + path);
					}
				}

				//using (Stream fileStream = File.Open(path, FileMode.Create))
				//{
				//    using (StreamWriter zip = new StreamWriter(fileStream))
				//    {
				//        zip.Write(JsonConvert.SerializeObject(entry.Value));
				//        CreateCSharpKeysFile(cSharpPathPrefix, "Firehorse.Localization", "LocalizationKeys", entry.Value.Keys.ToArray());
				//        Debug.Log("File saved: " + path);
				//    }
				//}
			}

			AssetDatabase.Refresh();

			Debug.Log("Finished creating and configuring localization data!");
			DestroyImmediate(gameObject);
		}

		#endregion

		#region File util

		public void CreateCSharpKeysFile(string path, string nameSpace, string fileName, string[] keys)
		{
			path += fileName + ".cs";

			var sb = new StringBuilder();

			sb.AppendLine("using System;");
			sb.AppendLine();
			sb.AppendLine(string.Format("namespace {0}", nameSpace));
			sb.AppendLine("{");
			sb.AppendLine(string.Format("  public class {0}", fileName));
			sb.AppendLine("  {");

			for (var i = 0; i < keys.Length; i++)
			{
				sb.AppendLine(string.Format("    public const string {0} = \"{1}\";", keys[i], keys[i]));
			}

			sb.AppendLine("  }");
			sb.AppendLine("}");

			File.WriteAllText(path, sb.ToString());
		}

		#endregion

		#region GetData
		
		protected IEnumerator GetData(string spreadSheetId, string sheetName)
		{
			var connectionString = DataRetriever.WebServiceUrl + "?ssid=" + spreadSheetId + "&sheet=" + sheetName + "&pass=" + DataRetriever.Password + "&action=GetData";
			Debug.Log("Connecting to webservice on " + connectionString);
			
			var www = new WWW(connectionString);
			
			var elapsedTime = 0.0f;
			Debug.Log ("Stablishing Connection... ");
			
			while (!www.isDone)
			{
				elapsedTime += Time.deltaTime;			
				if (elapsedTime >= DataRetriever.MaxWaitTime)
				{
					Debug.Log ("Max wait time reached, connection aborted.");
					break;
				}
                Debug.Log("Loading...");
			}
			
			Debug.Log ("Is www done? " + www.isDone);
			
			if (!www.isDone || !string.IsNullOrEmpty(www.error))
			{
				Debug.Log ("Connection error after" + elapsedTime.ToString() + "seconds: " + www.error);
				DestroyImmediate (gameObject);
				yield break;
			}
			
			var response = www.text;
			Debug.Log (elapsedTime + " : " + response);
			Debug.Log ("Connection stablished, parsing data...");
			
			if (response == "\"Incorrect Password.\"")
			{
				Debug.Log ("Connection error: Incorrect Password.");
				DestroyImmediate (gameObject);
				yield break;
			}
			
			try 
			{
				_spreadSheetResults = JsonMapper.ToObject<JsonData[]>(response);
			}
			catch
			{
				Debug.Log ("Data error: could not parse retrieved data as json.");
				DestroyImmediate (gameObject);
				yield break;
			}
			
			Debug.Log ("Data Successfully Retrieved!");
		}
		
		#endregion

		public void DestroyInstance()
		{
			DestroyImmediate(gameObject);
		}

		#endif
	}
}
#endif