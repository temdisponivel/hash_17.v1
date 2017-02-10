using FH.DataRetrieving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Hash17.Devices;
using Hash17.Devices.Firewalls;
using Hash17.FilesSystem.Files;
using Hash17.Programs;
using Hash17.MockSystem;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;
using DeviceType = Hash17.Devices.DeviceType;
using Hash17.Campaign;
using FH.Localization;
using System.IO;
using Directory = Hash17.Files.Directory;
using System.Text;
using System.Linq;
using File = Hash17.Files.File;

#if UNITY_EDITOR
using UnityEditor;

namespace Hash17.Utils
{
    [ExecuteInEditMode]
    public class Hash17DataRetrieverInstance : Singleton<Hash17DataRetrieverInstance>
    {
        #region BASE
        
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

        private IEnumerator RunFetchLocalizationInfos(string spreadSheetId, string[] sheetsName,
            SystemLanguage[] languages)
        {
            var results = new Dictionary<string, Dictionary<string, string>>(2);
            LocalizationManager.Instance.Languages = new List<SystemLanguage>();
            LocalizationManager.Instance.Languages.AddRange(languages);

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

                if (!System.IO.Directory.Exists(pathPrefix))
                {
                    System.IO.Directory.CreateDirectory(pathPrefix);
                }

                var cSharpPathPrefix = string.Format("{0}{1}{2}{3}{4}{5}",
                    Application.dataPath,
                    Path.DirectorySeparatorChar,
                    "FH-Framework",
                    Path.DirectorySeparatorChar,
                    "Localization",
                    Path.DirectorySeparatorChar);

                using (Stream fileStream = System.IO.File.Open(path, FileMode.Create))
                {
                    using (var zip = new StreamWriter(fileStream))
                    {
                        zip.Write(JsonConvert.SerializeObject(entry.Value));
                        CreateCSharpKeysFile(cSharpPathPrefix, "FH.Localization", "LocalizationKeys",
                            entry.Value.Keys.ToArray());
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

            System.IO.File.WriteAllText(path, sb.ToString());
        }

        #endregion

        #region GetData

        protected IEnumerator GetData(string spreadSheetId, string sheetName)
        {
            var connectionString = DataRetriever.WebServiceUrl + "?ssid=" + spreadSheetId + "&sheet=" + sheetName +
                                   "&pass=" + DataRetriever.Password + "&action=GetData";
            Debug.Log("Connecting to webservice on " + connectionString);

            var www = new WWW(connectionString);

            var elapsedTime = 0.0f;
            Debug.Log("Stablishing Connection... ");

            while (!www.isDone)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= DataRetriever.MaxWaitTime)
                {
                    Debug.Log("Max wait time reached, connection aborted.");
                    break;
                }
                Debug.Log("Loading...");
            }

            Debug.Log("Is www done? " + www.isDone);

            if (!www.isDone || !string.IsNullOrEmpty(www.error))
            {
                Debug.Log("Connection error after" + elapsedTime.ToString() + "seconds: " + www.error);
                DestroyImmediate(gameObject);
                yield break;
            }

            var response = www.text;
            Debug.Log(elapsedTime + " : " + response);
            Debug.Log("Connection stablished, parsing data...");

            if (response == "\"Incorrect Password.\"")
            {
                Debug.Log("Connection error: Incorrect Password.");
                DestroyImmediate(gameObject);
                yield break;
            }

            try
            {
                _spreadSheetResults = JsonMapper.ToObject<JsonData[]>(response);
            }
            catch
            {
                Debug.Log("Data error: could not parse retrieved data as json.");
                DestroyImmediate(gameObject);
                yield break;
            }

            Debug.Log("Data Successfully Retrieved!");
        }

        #endregion

        public void DestroyInstance()
        {
            DestroyImmediate(gameObject);
        }

        #endregion

        #region Fetch ProgramsByCommand

        public void FetchProgramsInfo(string spreadSheetId)
        {
            StartCoroutine(RunFetchProgramsInfos(spreadSheetId));
        }

        private IEnumerator RunFetchProgramsInfos(string spreadSheetId)
        {
            var results = new List<Program>();

            yield return StartCoroutine(GetData(spreadSheetId, "Programs"));

            Debug.Log("FINISH RETRIEVING FROM GOOGLE");

            if (_spreadSheetResults == null)
            {
                Debug.Log("NULL RETURN - DESTROYING");
                Destroy(gameObject);
                yield break;
            }

            for (var i = 0; i < _spreadSheetResults.Length; i++)
            {
                var current = _spreadSheetResults[i];
                var id = (ProgramType)Enum.Parse(typeof(ProgramType), current["Id"].ToString());
                var prog = GetProgramInstance(id, current);
                results.Add(prog);
            }

            var serializedData = JsonConvert.SerializeObject(results,
                new JsonSerializerSettings()
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                });

            CreateFile(Alias.Config.CollectionsSavePath, "ProgramCollectionData.txt", serializedData);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            Debug.Log("Finished creating and configuring programs data!");
            DestroyImmediate(gameObject);
        }

        private Program GetProgramInstance(ProgramType type, JsonData current)
        {
            Program result = null;
            var typeName = "Hash17.Programs.Implementation.{0}".InLineFormat(type.ToString());
            Debug.Log(typeName);
            var programType = Type.GetType(typeName);
            result = Activator.CreateInstance(programType) as Program;

            SetProgramBaseProperties(type, result, current);

            return result;
        }

        private void SetProgramBaseProperties(ProgramType programType, Program prog, JsonData current)
        {
            var uniqueId = int.Parse(current["UniqueId"].ToString());
            var com = current["Command"].ToString();
            var desc = current["Description"].ToString();
            var usage = current["Usage"].ToString();
            var knownParameters = current["KnownParametersAndOptions"].ToString();
            var availableGame = bool.Parse(current["Global"].ToString());
            var aditionalData = current["AditionalData"].ToString().Trim();
            var startUnlocked = bool.Parse(current["StartUnlocked"].ToString());

            prog.Type = programType;
            prog.Command = com;
            prog.UniqueId = uniqueId;
            prog.Description = desc;
            prog.Usage = usage;
            prog.Global = availableGame;
            prog.AditionalData = aditionalData;
            prog.KnownParametersAndOptions = !knownParameters.StartsWith("--")
                ? knownParameters.Split(';')
                : new string[0];
            prog.StartUnlocked = startUnlocked;
            for (int i = 0; i < prog.KnownParametersAndOptions.Length; i++)
            {
                prog.KnownParametersAndOptions[i] = prog.KnownParametersAndOptions[i].Trim();
            }
        }

        #endregion

        #region Fetch Devices

        public void FetchDeviceInfo(string spreadSheetId)
        {
            StartCoroutine(RunFetchDeviceInfos(spreadSheetId));
        }

        private IEnumerator RunFetchDeviceInfos(string spreadSheetId)
        {
            var results = new List<Device>();

            yield return StartCoroutine(GetData(spreadSheetId, "Files"));

            Debug.Log("FINISH RETRIEVING FILEs FROM GOOGLE");

            if (_spreadSheetResults == null)
            {
                Debug.Log("NULL RETURN - DESTROYING");
                Destroy(gameObject);
                yield break;
            }

            Dictionary<string, List<File>> files = new Dictionary<string, List<File>>();
            for (var i = 0; i < _spreadSheetResults.Length; i++)
            {
                var current = _spreadSheetResults[i];
                File file;
                var deviceUniqueId = GetFile(current, out file);
                if (!files.ContainsKey(deviceUniqueId))
                    files[deviceUniqueId] = new List<File>();

                files[deviceUniqueId].Add(file);
            }

            yield return StartCoroutine(GetData(spreadSheetId, "Devices"));

            Debug.Log("FINISH RETRIEVING DEVICES FROM GOOGLE");

            if (_spreadSheetResults == null)
            {
                Debug.Log("NULL RETURN - DESTROYING");
                Destroy(gameObject);
                yield break;
            }

            for (var i = 0; i < _spreadSheetResults.Length; i++)
            {
                var current = _spreadSheetResults[i];
                var deviceType = (DeviceType)Enum.Parse(typeof(DeviceType), current["DeviceType"].ToString());
                var device = GetDeviceInstance(deviceType, current, files);
                results.Add(device);
            }

            var serializedData = JsonConvert.SerializeObject(results,
                new JsonSerializerSettings()
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                });

            CreateFile(Alias.Config.CollectionsSavePath, "DeviceCollectionData.txt", serializedData);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            Debug.Log("Finished creating and configuring devices data!");
            DestroyImmediate(gameObject);
        }

        private string GetFile(JsonData current, out File file)
        {
            file = new File()
            {
                UniqueId = int.Parse(current["UniqueId"].ToString()),
                Name = current["Name"].ToString(),
                Content = current["Content"].ToString(),
                PathString = current["Path"].ToString(),
                IsProtected = bool.Parse(current["IsProtected"].ToString()),
                Password = current["Password"].ToString(),
                StartUnlocked = bool.Parse(current["StartUnlocked"].ToString()),
            };

            var fileType = (FileType)Enum.Parse(typeof(FileType), current["FileType"].ToString());
            file.FileType = fileType;

            return current["DeviceUniqueId"].ToString();
        }

        private Device GetDeviceInstance(DeviceType type, JsonData current,
            Dictionary<string, List<File>> filesPerDevice)
        {
            Device result = null;
            switch (type)
            {
                case DeviceType.Normal:
                    result = new Device();
                    break;
                case DeviceType.Passworded:
                    result = new PasswordedDevice();
                    break;
            }

            SetDeviceBaseProperties(result, current, filesPerDevice);

            if (type == DeviceType.Passworded)
                SetPasswordedDeviceProperties(result as PasswordedDevice, current);

            return result;
        }

        private void SetDeviceBaseProperties(Device prog, JsonData currentDevice,
            Dictionary<string, List<File>> filesPerDevice)
        {
            var uniqueId = currentDevice["UniqueId"].ToString();
            var name = currentDevice["Name"].ToString();
            var firewallType =
                (FirewallType)Enum.Parse(typeof(FirewallType), currentDevice["FirewallType"].ToString());
            var specialPrograms = currentDevice["SpecialPrograms"].ToString().Trim();
            var dicSpecialProgram = new Dictionary<ProgramType, int>();
            var startUnlocked = bool.Parse(currentDevice["StartUnlocked"].ToString());
            if (!string.IsNullOrEmpty(specialPrograms))
            {
                var specialProgramsDef = currentDevice["SpecialPrograms"].ToString().Split(',');
                for (int i = 0; i < specialProgramsDef.Length; i++)
                {
                    var currentProg = specialProgramsDef[i];
                    var parts = currentProg.Split(';');
                    var programId = (ProgramType)Enum.Parse(typeof(ProgramType), parts[0]);
                    var programUniqueId = parts[1];
                    dicSpecialProgram[programId] = int.Parse(programUniqueId);
                }
            }

            prog.Id = uniqueId;
            prog.UniqueId = prog.Id.GetHashCode();
            prog.Name = name;
            prog.FirewallType = firewallType;
            prog.SpecialPrograms = dicSpecialProgram;
            prog.StartUnlocked = startUnlocked;

            FileSystem fileSystem = new FileSystem();
            prog.FileSystem = fileSystem;

            var files = filesPerDevice[uniqueId];

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];

                Directory dir;
                fileSystem.CreateDiretory(file.PathString, out dir);
                fileSystem.AddFileWithoutValidation(dir, file);
            }
        }

        private void SetPasswordedDeviceProperties(PasswordedDevice device, JsonData current)
        {
            device.Password = current["AditionalData"].ToString();
        }

        #endregion

        #region Fetch text assets

        public void FetchTextAssets(string spreadSheetId)
        {
            StartCoroutine(RunFetchTextAsset(spreadSheetId));
        }

        public IEnumerator RunFetchTextAsset(string spreadSheetId)
        {
            yield return StartCoroutine(GetData(spreadSheetId, "TextAssets"));

            Debug.Log("FINISH RETRIEVING TEXT ASSETS FROM GOOGLE");

            if (_spreadSheetResults == null)
            {
                Debug.Log("NULL RETURN - DESTROYING");
                Destroy(gameObject);
                yield break;
            }

            for (var i = 0; i < _spreadSheetResults.Length; i++)
            {
                var current = _spreadSheetResults[i];
                var content = current["Content"].ToString();
                var path = current["Path"].ToString();
                var name = current["Name"].ToString();
                CreateFile(path, name, content);
            }

            AssetDatabase.Refresh();

            Debug.Log("FINISH CREATING TEXT ASSETS");
            DestroyImmediate(gameObject);
        }

        #endregion

        #region Fetch Campaign Items

        public void FetchCampaignMission(string spreadSheetId)
        {
            StartCoroutine(RunFetchCampaignMission(spreadSheetId));
        }

        public IEnumerator RunFetchCampaignMission(string spreadSheetId)
        {
            {
                yield return StartCoroutine(GetData(spreadSheetId, "CampaignMissions"));

                Debug.Log("FINISH RETRIEVING CAMPAIGN ITEMS FROM GOOGLE");

                if (_spreadSheetResults == null)
                {
                    Debug.Log("NULL RETURN - DESTROYING");
                    Destroy(gameObject);
                    yield break;
                }

                var items = new List<CampaignMission>();
                for (var i = 0; i < _spreadSheetResults.Length; i++)
                {
                    var current = _spreadSheetResults[i];
                    var campaignItem = new CampaignMission();

                    campaignItem.Id = int.Parse(current["Id"].ToString());
                    campaignItem.FilesToOpen = GetIntList(current, "FilesToOpen");
                    campaignItem.FilesToDecrypt = GetIntList(current, "FilesToDecrypt");
                    campaignItem.DevicesToConnect = GetStringHashedList(GetStringList(current, "DevicesToConnect"));
                    campaignItem.DevicesToDecrypt = GetStringHashedList(GetStringList(current, "DevicesToDecrypt"));
                    campaignItem.SystemVariablesToSet = GetStringList(current, "SystemVariablesToSet");
                    campaignItem.ProgramsToUnlock = GetIntList(current, "ProgramsToUnlock");
                    campaignItem.CampaignMissionReward = GetIntList(current, "CampaignMissionRewards");
                    campaignItem.CampaignMissionReward = GetIntList(current, "CampaignMissionToComplete");

                    items.Add(campaignItem);
                }

                var serializedData = JsonConvert.SerializeObject(items,
                    new JsonSerializerSettings()
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        TypeNameHandling = TypeNameHandling.All,
                        TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                    });

                CreateFile(Alias.Config.CollectionsSavePath, "CampaignMissionsData.txt", serializedData);
                
                Debug.Log("Finished creating and configuring campaign missions data!");
            }

            {
                yield return StartCoroutine(GetData(spreadSheetId, "CampaignMissionsRewards"));


                Debug.Log("FINISH RETRIEVING CAMPAIGN ITEMS FROM GOOGLE");

                if (_spreadSheetResults == null)
                {
                    Debug.Log("NULL RETURN - DESTROYING");
                    Destroy(gameObject);
                    yield break;
                }

                var itemsRewards = new List<CampaignMissionReward>();
                for (var i = 0; i < _spreadSheetResults.Length; i++)
                {
                    var current = _spreadSheetResults[i];
                    var campaignItem = new CampaignMissionReward();

                    campaignItem.Id = int.Parse(current["Id"].ToString());
                    campaignItem.FilesToUnlock = GetIntList(current, "FilesToUnlock");
                    campaignItem.DevicesToUnlock = GetStringHashedList(GetStringList(current, "DevicesToUnlock"));
                    campaignItem.ProgramsToUnlock = GetStringHashedList(GetStringList(current, "ProgramsToUnlock"));
                    campaignItem.CommandsToRun = GetStringList(current, "CommandsToRun");

                    itemsRewards.Add(campaignItem);
                }

                var serializedData = JsonConvert.SerializeObject(itemsRewards,
                    new JsonSerializerSettings()
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        TypeNameHandling = TypeNameHandling.All,
                        TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                    });

                CreateFile(Alias.Config.CollectionsSavePath, "CampaignMissionsRewardsData.txt", serializedData);

            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            Debug.Log("Finished creating and configuring campaign mission rewards data!");
            DestroyImmediate(gameObject);
        }

        #endregion

        #region Helpers

        public void CreateFile(string path, string name, string content)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            System.IO.Directory.CreateDirectory(path);
            System.IO.File.WriteAllText(string.Format("{0}{1}", path, name), content);
        }

        private List<int> GetIntList(JsonData currentEntry, string collumn)
        {
            Debug.Log("LINE: {0}".InLineFormat(currentEntry));
            Debug.Log("FINDING COLLUMN: {0}".InLineFormat(collumn));
            var results = new List<int>();
            var values = currentEntry[collumn].ToString().Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                var cur = values[i];
                if (cur == "--")
                    continue;
                int curInt = int.Parse(cur);
                results.Add(curInt);
            }

            return results;
        }

        private List<string> GetStringList(JsonData currentEntry, string collumn)
        {
            var results = new List<string>();
            var values = currentEntry[collumn].ToString().Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                var cur = values[i];
                if (cur == "--")
                    continue;
                results.Add(cur.ToString());
            }

            return results;
        }

        private List<int> GetStringHashedList(List<string> valuesToHash)
        {
            var result = new List<int>();
            for (int i = 0; i < valuesToHash.Count; i++)
            {
                result.Add(valuesToHash[i].GetHashCode());
            }

            return result;
        }

        #endregion
    }
}
#endif