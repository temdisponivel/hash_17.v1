using System;
using System.Collections.Generic;
using System.Linq;
using Hash17.Files;
using Hash17.Game;
using Hash17.MockSystem;
using Hash17.Programs;
using MockSystem;
using Hash17.Utils;
using MockSystem.Term;
using Newtonsoft.Json;
using UnityEngine;
using File = System.IO.File;

namespace Hash17.Campaign
{
    public class CampaignManager : NonUnitySingleton<CampaignManager>
    {
        #region Inner Types

        public class CampaignInfo
        {
            [JsonProperty("PN")]
            public string PlayerName { get; set; }

            [JsonProperty("CD")]
            public HashSet<int> CrackedDevices { get; set; }

            [JsonProperty("CF")]
            public HashSet<int> CrackedFiles { get; set; }

            [JsonProperty("UD")]
            public HashSet<int> UnlockedDirectories { get; set; }

            [JsonProperty("UF")]
            public HashSet<int> UnlockedFiles { get; set; }

            [JsonProperty("UDV")]
            public HashSet<int> UnlockedDevices { get; set; }
            
            [JsonProperty("CCI")]
            public List<int> CompletedCampaignItems { get; set; }

            [JsonProperty("UP")]
            public List<int> UnlockPrograms { get; set; }

            public CampaignInfo()
            {
                CrackedDevices = new HashSet<int>();
                CrackedFiles = new HashSet<int>();
            }
        }

        #endregion

        #region Properties

        public bool IsFirstTimeInGame { get; set; }
        public CampaignInfo Info { get; protected set; }

        public List<CampaignItem> AllCampaignItems { get; set; }
        public List<CampaignItem> UncompletedCampaignItems { get; set; }
        public List<CampaignItem> CompletedCampaignItems { get; set; }

        public string SavePath
        {
            get { return "{0}{1}".InLineFormat(Application.persistentDataPath, Alias.Config.CampaignSavePath); }
        }

        public Action<CampaignItem> OnCampaignItemCompleted { get; set; }

        #endregion

        #region Setup

        public void OnGameStarted()
        {
            LoadProgress();
            Alias.SysVariables.OnSystemVariableChange += OnSystemVariableChanged;
        }

        public void SaveProgress()
        {
            File.WriteAllText(SavePath, JsonConvert.SerializeObject(Info));
        }

        public void LoadProgress()
        {
            if (!File.Exists(SavePath))
            {
                IsFirstTimeInGame = true;
                SaveProgress();
            }

            var content = File.ReadAllText(SavePath);
            Info = JsonConvert.DeserializeObject<CampaignInfo>(content);

            if (Info.CrackedFiles == null)
                Info.CrackedFiles = new HashSet<int>();

            if (Info.CrackedDevices == null)
                Info.CrackedDevices = new HashSet<int>();

            Alias.SysVariables.Add(SystemVariableType.USERNAME, Info.PlayerName);
        }

        #endregion
        
        #region Callbacks

        private void OnSystemVariableChanged(SystemVariableType variable)
        {
            if (variable == SystemVariableType.USERNAME)
            {
                IsFirstTimeInGame = true;
                Info.PlayerName = Alias.SysVariables[SystemVariableType.USERNAME];
            }
        }

        #endregion

        #region Campaign Items

        #region Validations

        public void ValidateCampaignItems(CampaignTriggetType typeToValidate)
        {
            var itemsToValidate = UncompletedCampaignItems.FindAll(i => i.Type == typeToValidate);
            for (int i = 0; i < itemsToValidate.Count; i++)
            {
                var item = itemsToValidate[i];
                if (!item.Dependecies.TrueForAll(id => CompletedCampaignItems.Exists(complete => complete.Id == id)))
                    continue;

                ExecuteCampignItem(item);

                CompletedCampaignItems.Add(item);
                UncompletedCampaignItems.Remove(item);

                if (OnCampaignItemCompleted != null)
                    OnCampaignItemCompleted(item);
            }
        }

        #endregion

        #region Execution

        public void ExecuteCampignItem(CampaignItem item)
        {
            switch (item.Action)
            {
                case CampaignActionType.ExecuteProgram:
                    ExecuteProgramCampaignItem(item);
                    break;
                case CampaignActionType.UnlockDevice:
                    ExecuteUnlockDeviceCampaignItem(item);
                    break;
                case CampaignActionType.UnlockFile:
                    ExecuteUnlockDirCampaignItem(item);
                    break;
                case CampaignActionType.UnlockDirectory:
                    ExecuteUnlockDirCampaignItem(item);
                    break;
            }
        }

        public void ExecuteProgramCampaignItem(CampaignItem item)
        {
            Program program;
            var parameters = string.Empty;
            var result = Alias.Programs.GetProgramAndParameters(item.AditionalData, out program, out parameters);
            if (result != ProgramCollection.ProgramRequestResult.Ok)
            {
                Debug.LogError("ERROR TRYING TO EXECUTE CAMPAIGN ITEM {0}. PROGRAM QUERY RETURNED {1}".InLineFormat(item.Id, result));
                return;
            }

            Alias.Term.RunProgram(program, parameters);
        }

        public void ExecuteUnlockDeviceCampaignItem(CampaignItem item)
        {
            var devices = item.AditionalData.Split(',');
            for (int i = 0; i < devices.Length; i++)
            {
                int deviceId;
                if (!int.TryParse(devices[i], out deviceId))
                {
                    Debug.LogError("ERROR PARSING ADITIONAL DATA {0} WHEN UNLOCK DEVICE FROM CAMPAIGN ITEM ID {1}".InLineFormat(devices[i], item.Id));
                }

                Info.UnlockedDevices.Add(deviceId);
            }
        }

        public void ExecuteUnlockFileCampaignItem(CampaignItem item)
        {
            var files = item.AditionalData.Split(',');
            for (int i = 0; i < files.Length; i++)
            {
                int fileId;
                if (!int.TryParse(files[i], out fileId))
                {
                    Debug.LogError("ERROR PARSING ADITIONAL DATA {0} WHEN UNLOCK FILE FROM CAMPAIGN ITEM ID {1}".InLineFormat(files[i], item.Id));
                }

                Info.UnlockedFiles.Add(fileId);
            }
        }

        public void ExecuteUnlockDirCampaignItem(CampaignItem item)
        {
            var dir = item.AditionalData.Split(',');
            for (int i = 0; i < dir.Length; i++)
            {
                int dirId;
                if (!int.TryParse(dir[i], out dirId))
                {
                    Debug.LogError("ERROR PARSING ADITIONAL DATA {0} WHEN UNLOCK DIRECTORY FROM CAMPAIGN ITEM ID {1}".InLineFormat(dir[i], item.Id));
                }

                Info.UnlockedDirectories.Add(dirId);
            }
        }

        #endregion

        #region Load

        public void LoadCampaignItems(TextAsset serializedData)
        {
            var content = serializedData.text;
            AllCampaignItems = JsonConvert.DeserializeObject<List<CampaignItem>>(content);

            UncompletedCampaignItems = new List<CampaignItem>(AllCampaignItems);
            UncompletedCampaignItems.RemoveAll(i => !Info.CompletedCampaignItems.Contains(i.Id));

            CompletedCampaignItems = new List<CampaignItem>(AllCampaignItems);
            CompletedCampaignItems.RemoveAll(i => Info.CompletedCampaignItems.Contains(i.Id));
        }

        #endregion

        #endregion
    }
}