﻿using FH.DataRetrieving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Hash17.Devices;
using Hash17.Devices.Firewalls;
using Hash17.Files;
using Hash17.FilesSystem.Files;
using Hash17.Programs;
using Hash17.MockSystem;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;
using DeviceType = Hash17.Devices.DeviceType;
using Hash17.Campaign;

#if UNITY_EDITOR
using UnityEditor;

namespace Hash17.Utils
{
    [ExecuteInEditMode]
    public class Hash17DataRetrieverInstance : DataRetrieverInstanceBase
    {
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
            prog.KnownParametersAndOptions = !knownParameters.StartsWith("--") ? knownParameters.Split(';') : new string[0];
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

        private Device GetDeviceInstance(DeviceType type, JsonData current, Dictionary<string, List<File>> filesPerDevice)
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

        private void SetDeviceBaseProperties(Device prog, JsonData currentDevice, Dictionary<string, List<File>> filesPerDevice)
        {
            var uniqueId = currentDevice["UniqueId"].ToString();
            var name = currentDevice["Name"].ToString();
            var firewallType = (FirewallType)Enum.Parse(typeof(FirewallType), currentDevice["FirewallType"].ToString());
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

        public void FetchCampaignItems(string spreadSheetId)
        {
            StartCoroutine(RunFetchCampaignItems(spreadSheetId));
        }

        public IEnumerator RunFetchCampaignItems(string spreadSheetId)
        {
            yield return StartCoroutine(GetData(spreadSheetId, "CampaignItems"));

            Debug.Log("FINISH RETRIEVING CAMPAIGN ITEMS FROM GOOGLE");

            if (_spreadSheetResults == null)
            {
                Debug.Log("NULL RETURN - DESTROYING");
                Destroy(gameObject);
                yield break;
            }

            var items = new List<CampaignItem>();
            for (var i = 0; i < _spreadSheetResults.Length; i++)
            {
                var current = _spreadSheetResults[i];
                var campaignItem = new CampaignItem();

                campaignItem.Id = int.Parse(current["Id"].ToString());
                campaignItem.Trigger = (CampaignTriggerType) Enum.Parse(typeof (CampaignTriggerType), current["Trigger"].ToString());
                campaignItem.Action = (CampaignActionType)Enum.Parse(typeof(CampaignActionType), current["Action"].ToString());

                var dep = current["Dependencies"].ToString();
                campaignItem.Dependecies = new List<int>();
                if (dep != "--")
                {
                    var parts = dep.Split(',');
                    for (int j = 0; j < parts.Length; j++)
                    {
                        var currentPart = parts[j];
                        campaignItem.Dependecies.Add(int.Parse(currentPart));
                    }
                }

                campaignItem.TriggerAditionalData = current["TriggerAditionalData"].ToString();
                campaignItem.ActionAditionalData = current["ActionAditionalData"].ToString();

                items.Add(campaignItem);
            }

            var serializedData = JsonConvert.SerializeObject(items,
                        new JsonSerializerSettings()
                        {
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                            TypeNameHandling = TypeNameHandling.All,
                            TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                        });

            CreateFile(Alias.Config.CollectionsSavePath, "CampaignItemsData.txt", serializedData);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            Debug.Log("Finished creating and configuring campaign items data!");
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

        #endregion
    }
}
#endif