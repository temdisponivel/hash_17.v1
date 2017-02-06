using System;
using System.Collections.Generic;
using Hash17.Devices;
using Hash17.Files;
using Hash17.MockSystem;
using Hash17.Programs;
using Hash17.Utils;
using Newtonsoft.Json;
using UnityEngine;

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
            public Hash17HashSet<int> CrackedDevices { get; set; }

            [JsonProperty("CF")]
            public Hash17HashSet<int> CrackedFiles { get; set; }

            [JsonProperty("UD")]
            public Hash17HashSet<int> UnlockedDirectories { get; set; }

            [JsonProperty("UF")]
            public Hash17HashSet<int> UnlockedFiles { get; set; }

            [JsonProperty("UDV")]
            public Hash17HashSet<int> UnlockedDevices { get; set; }

            [JsonProperty("CCI")]
            public Hash17HashSet<int> CompletedCampaignItems { get; set; }

            [JsonProperty("UP")]
            public Hash17HashSet<int> UnlockPrograms { get; set; }

            public CampaignInfo()
            {
                CrackedDevices = new Hash17HashSet<int>();
                CrackedFiles = new Hash17HashSet<int>();
            }
        }

        #endregion

        #region Properties

        public bool IsFirstTimeInGame { get; set; }

        private CampaignInfo _info;

        public CampaignInfo Info
        {
            get
            {
                return _info;
            }
            private set
            {
                var previous = _info;
                if (previous != null)
                {
                    previous.CrackedFiles.OnItemAdded -= OnFileDecrypted;
                    previous.CrackedDevices.OnItemAdded -= OnDeviceCracked;
                    previous.UnlockedDevices.OnItemAdded -= OnDeviceUnlocked;
                    previous.UnlockedFiles.OnItemAdded -= OnFileUnlocked;
                    previous.UnlockedDirectories.OnItemAdded -= OnDirUnlocked;
                    previous.UnlockPrograms.OnItemAdded -= OnProgramUnlocked;
                    previous.CompletedCampaignItems.OnItemAdded -= OnCampaignItemCompletedTrigget;
                }

                _info = value;
                if (_info != null)
                {
                    _info.CrackedFiles.OnItemAdded += OnFileDecrypted;
                    _info.CrackedDevices.OnItemAdded += OnDeviceCracked;
                    _info.UnlockedDevices.OnItemAdded += OnDeviceUnlocked;
                    _info.UnlockedFiles.OnItemAdded += OnFileUnlocked;
                    _info.UnlockedDirectories.OnItemAdded += OnDirUnlocked;
                    _info.UnlockPrograms.OnItemAdded += OnProgramUnlocked;
                    _info.CompletedCampaignItems.OnItemAdded += OnCampaignItemCompletedTrigget;
                }
            }
        }

        public List<CampaignItem> AllCampaignItems { get; set; }
        public List<CampaignItem> UncompletedCampaignItems { get; set; }
        public List<CampaignItem> CompletedCampaignItems { get; set; }

        public string SavePath
        {
            get { return "{0}{1}".InLineFormat(Application.persistentDataPath, Alias.Config.CampaignSavePath); }
        }

        public event Action<CampaignItem> OnCampaignItemCompleted;

        #endregion

        #region Setup

        public void OnGameStarted()
        {
            LoadProgress();
            Alias.SysVariables.OnSystemVariableChange += OnSystemVariableChanged;
            File.OnFileOpened += OnFileOpened;
            Alias.Devices.OnCurrentDeviceChange += OnDeviceConnected;
        }

        public void SaveProgress()
        {
            System.IO.File.WriteAllText(SavePath, JsonConvert.SerializeObject(Info));
        }

        public void LoadProgress()
        {
            if (!System.IO.File.Exists(SavePath))
            {
                IsFirstTimeInGame = true;
                SaveProgress();
            }

            var content = System.IO.File.ReadAllText(SavePath);
            Info = JsonConvert.DeserializeObject<CampaignInfo>(content);

            if (Info.CrackedFiles == null)
                Info.CrackedFiles = new Hash17HashSet<int>();

            if (Info.CrackedDevices == null)
                Info.CrackedDevices = new Hash17HashSet<int>();

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

            ValidateCampaignItems(CampaignTriggetType.SystemVariableChange, variable);
        }

        private void OnFileOpened(File openedFile)
        {
            ValidateCampaignItems(CampaignTriggetType.FileOpened, openedFile.UniqueId);
        }

        private void OnFileDecrypted(int fileId)
        {
            ValidateCampaignItems(CampaignTriggetType.FileDecrypted, fileId);
        }

        private void OnDeviceConnected()
        {
            ValidateCampaignItems(CampaignTriggetType.DeviceConnect, DeviceCollection.CurrentDevice.UniqueId);
        }

        private void OnDeviceCracked(int deviceCracked)
        {
            ValidateCampaignItems(CampaignTriggetType.DeviceCracked, deviceCracked);
        }

        private void OnDirUnlocked(int dirId)
        {
            ValidateCampaignItems(CampaignTriggetType.DirUnlocked, dirId);
        }

        private void OnFileUnlocked(int fileUnlocked)
        {
            ValidateCampaignItems(CampaignTriggetType.FileUnlocked, fileUnlocked);
        }

        private void OnProgramUnlocked(int programUnlocked)
        {
            ValidateCampaignItems(CampaignTriggetType.ProgramUnlocked, programUnlocked);
        }

        private void OnDeviceUnlocked(int programUnlocked)
        {
            ValidateCampaignItems(CampaignTriggetType.DeviceUnlocked, programUnlocked);
        }

        private void OnCampaignItemCompletedTrigget(int campaignItemCompleted)
        {
            ValidateCampaignItems(CampaignTriggetType.CampaignItemCompleted, campaignItemCompleted);
        }

        #endregion

        #region Campaign Items

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

        #region Validations

        public void ValidateCampaignItems(CampaignTriggetType typeToValidate, object data)
        {
            var itemsToValidate = UncompletedCampaignItems.FindAll(i => i.Type == typeToValidate);
            for (int i = 0; i < itemsToValidate.Count; i++)
            {
                var item = itemsToValidate[i];

                if (!ValidateCampaignItem(item, data))
                    continue;

                ExecuteCampignItem(item);

                CompletedCampaignItems.Add(item);
                UncompletedCampaignItems.Remove(item);

                if (OnCampaignItemCompleted != null)
                    OnCampaignItemCompleted(item);
            }
        }

        public bool ValidateCampaignItem(CampaignItem item, object data)
        {
            var result = !ValidateDependencies(item);
            switch (item.Type)
            {
                case CampaignTriggetType.FileOpened:
                case CampaignTriggetType.DeviceConnect:
                case CampaignTriggetType.FileDecrypted:
                case CampaignTriggetType.DeviceCracked:
                case CampaignTriggetType.FileUnlocked:
                case CampaignTriggetType.DirUnlocked:
                case CampaignTriggetType.ProgramUnlocked:
                case CampaignTriggetType.DeviceUnlocked:
                case CampaignTriggetType.CampaignItemCompleted:
                    result &= ValidateIntItem(item, data);
                    break;
                case CampaignTriggetType.SystemVariableChange:
                    result &= ValidateSystemVariableChange(item, data);
                    break;
            }
            
            return result;
        }

        public bool ValidateDependencies(CampaignItem item)
        {
            return item.Dependecies.TrueForAll(id => CompletedCampaignItems.Exists(complete => complete.Id == id));
        }

        public bool ValidateIntItem(CampaignItem item, object openFileId)
        {
            int fileId = 0;
            if (!int.TryParse(item.ActionAditionalData, out fileId))
            {
                Debug.LogError("ERROR TRYING TO VALIDATE CAMPAIGN ITEM {0}. DATA {1}: ".InLineFormat(item.Id, item.ActionAditionalData));
            }

            return (int) openFileId == fileId;
        }
        
        public bool ValidateSystemVariableChange(CampaignItem item, object systemVariable)
        {
            if (!Enum.IsDefined(typeof(SystemVariableType), item.ActionAditionalData))
            {
                Debug.LogError("ERROR TRYING TO VALIDATE CAMPAIGN ITEM {0}. SYSTEM VARIABLE NOT PARSED: {1}".InLineFormat(item.Id, item.ActionAditionalData));
            }

            return (SystemVariableType)Enum.Parse(typeof(SystemVariableType), item.ActionAditionalData) == (SystemVariableType)systemVariable;
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
            var result = Alias.Programs.GetProgramAndParameters(item.ActionAditionalData, out program, out parameters);
            if (result != ProgramCollection.ProgramRequestResult.Ok)
            {
                Debug.LogError("ERROR TRYING TO EXECUTE CAMPAIGN ITEM {0}. PROGRAM QUERY RETURNED {1}".InLineFormat(item.Id, result));
                return;
            }

            Alias.Term.RunProgram(program, parameters);
        }

        public void ExecuteUnlockDeviceCampaignItem(CampaignItem item)
        {
            var devices = item.ActionAditionalData.Split(',');
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
            var files = item.ActionAditionalData.Split(',');
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
            var dir = item.ActionAditionalData.Split(',');
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

        #endregion
    }
}