using System;
using System.Collections;
using System.Collections.Generic;
using FH.DataRetrieving;
using Hash17.Devices;
using Hash17.Devices.Firewalls;
using Hash17.Files;
using Hash17.FilesSystem.Files;
using Hash17.Programs;
using Hash17.Programs.Implementation;
using LitJson;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Help = Hash17.Programs.Implementation.Help;
using DeviceType = Hash17.Devices.DeviceType;

namespace Hash17.Utils
{
    [ExecuteInEditMode]
    public class Hash17DataRetrieverInstance : DataRetrieverInstanceBase
    {
        protected static Hash17DataRetrieverInstance _instance;

        public static Hash17DataRetrieverInstance Instance
        {
            get
            {
                var go = new GameObject("Data-Retriever-Instance");
                _instance = go.AddComponent<Hash17DataRetrieverInstance>();
                return _instance;
            }
        }

        #region Fetch Programs

        public void FetchProgramsInfo(string spreadSheetId)
        {
            StartCoroutine(RunFetchProgramsInfos(spreadSheetId));
        }

        private IEnumerator RunFetchProgramsInfos(string spreadSheetId)
        {
            var results = new List<Program>();

            AssetDatabase.DeleteAsset("Assets/Resources/Programs/ProgramsCollection.asset");

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
                var id = (ProgramId) Enum.Parse(typeof(ProgramId), current["Id"].ToString());
                var prog = GetProgramInstance(id, current);
                results.Add(prog);
            }

            var collection = DataRetrieverBase.CreateAsset<ProgramCollection>("Assets/Resources/Programs/", "ProgramsCollection.asset");
            collection.Programs = results;
            collection.Save();

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            Debug.Log("Finished creating and configuring programs data!");
            DestroyImmediate(gameObject);
        }

        private Program GetProgramInstance(ProgramId id, JsonData current)
        {
            Program result = null;
            switch (id)
            {
                case ProgramId.Clear:
                    result = new Clear();
                    break;
                case ProgramId.Cd:
                    result = new Cd();
                    break;
                case ProgramId.Read:
                    result = new Read();
                    break;
                case ProgramId.Search:
                    result = new Search();
                    break;
                case ProgramId.Help:
                    result = new Help();
                    break;
                case ProgramId.Init:
                    result = new Init();
                    break;
                case ProgramId.Connect:
                    result = new Connect();
                    break;
                case ProgramId.Cypher:
                    result = new Cypher();
                    break;
            }

            SetProgramBaseProperties(id, result, current);

            if (id == ProgramId.Init)
                SetProgramInitProperties(result as Init, current);
            
            return result;
        }

        private void SetProgramBaseProperties(ProgramId programId, Program prog, JsonData current)
        {
            var uniqueId = int.Parse(current["UniqueId"].ToString());
            var com = current["Command"].ToString();
            var desc = current["Description"].ToString();
            var usage = current["Usage"].ToString();
            var knownParameters = current["KnownParametersAndOptions"].ToString();
            var availableGame = bool.Parse(current["Global"].ToString());

            prog.Id = programId;
            prog.Command = com;
            prog.UnitqueId = uniqueId;
            prog.Description = desc;
            prog.Usage = usage;
            prog.Global = availableGame;
            prog.KnownParametersAndOptions = !knownParameters.StartsWith("--") ? knownParameters.Split(';') : new string[0];
            for (int i = 0; i < prog.KnownParametersAndOptions.Length; i++)
            {
                prog.KnownParametersAndOptions[i] = prog.KnownParametersAndOptions[i].Trim();
            }
        }

        private void SetProgramInitProperties(Init init, JsonData current)
        {
            init.InitTextPath = current["AditionalData"].ToString();
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

            AssetDatabase.DeleteAsset("Assets/Resources/Devices/DevicesCollection.asset");

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
                var deviceType = (DeviceType) Enum.Parse(typeof (DeviceType), current["DeviceType"].ToString());
                var device = GetDeviceInstance(deviceType, current, files);
                results.Add(device);
            }

            var collection = DataRetrieverBase.CreateAsset<DeviceCollection>("Assets/Resources/Devices/", "DevicesCollection.asset");
            collection.Devices = results;
            collection.Save();

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
            };

            var fileType = (FileType) Enum.Parse(typeof (FileType), current["FileType"].ToString());
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
            var firewallType = (FirewallType) Enum.Parse(typeof (FirewallType), currentDevice["FirewallType"].ToString());
            var specialPrograms = currentDevice["SpecialPrograms"].ToString().Trim();
            var dicSpecialProgram = new Dictionary<ProgramId, int>();
            if (!string.IsNullOrEmpty(specialPrograms))
            {
                var specialProgramsDef = currentDevice["SpecialPrograms"].ToString().Split(',');
                for (int i = 0; i < specialProgramsDef.Length; i++)
                {
                    var currentProg = specialProgramsDef[i];
                    var parts = currentProg.Split(';');
                    var programId = (ProgramId)Enum.Parse(typeof(ProgramId), parts[0]);
                    var programUniqueId = parts[1];
                    dicSpecialProgram[programId] = int.Parse(programUniqueId);
                }
            }

            prog.UniqueId = uniqueId;
            prog.Name = name;
            prog.FirewallType = firewallType;
            prog.SpecialPrograms = dicSpecialProgram;
            
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
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                System.IO.Directory.CreateDirectory(path);
                System.IO.File.WriteAllText(string.Format("{0}{1}", path, name), content);
            }

            AssetDatabase.Refresh();

            Debug.Log("FINISH CREATING TEXT ASSETS");
            DestroyImmediate(gameObject);
        }

        #endregion
    }
}