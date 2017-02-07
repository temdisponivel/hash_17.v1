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
                InitializeIfNull();
            }

            public void InitializeIfNull()
            {
                if (CrackedDevices == null)
                    CrackedDevices = new Hash17HashSet<int>();
                if (CrackedFiles == null)
                    CrackedFiles = new Hash17HashSet<int>();
                if (UnlockedFiles == null)
                    UnlockedFiles = new Hash17HashSet<int>();
                if (UnlockedDevices == null)
                    UnlockedDevices = new Hash17HashSet<int>();
                if (CompletedCampaignItems == null)
                    CompletedCampaignItems = new Hash17HashSet<int>();
                if (UnlockPrograms == null)
                    UnlockPrograms = new Hash17HashSet<int>();
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
            Alias.SysVariables.OnSystemVariableChange += OnSystemVariableChanged;
            File.OnFileOpened += OnFileOpened;
            Alias.Devices.OnCurrentDeviceChange += OnDeviceConnected;

            if (IsFirstTimeInGame)
                ValidateCampaignItems(CampaignTriggerType.NewGameStarted, null);
        }

        public void LoadStuff()
        {
            LoadProgress();
            LoadCampaignItems(Alias.DataHolder.CampaignItemsSerializedData);
        }

        public void SaveProgress()
        {
            System.IO.File.WriteAllText(SavePath, JsonConvert.SerializeObject(Info));
        }

        public void LoadProgress()
        {
            CampaignInfo info;
            if (!System.IO.File.Exists(SavePath))
            {
                IsFirstTimeInGame = true;
                info = new CampaignInfo();
            }
            else
            {
                var content = System.IO.File.ReadAllText(SavePath);
                info = JsonConvert.DeserializeObject<CampaignInfo>(content);
                info.PlayerName = Alias.Config.DefaultUserName;
            }
            info.InitializeIfNull();
            Info = info;
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

            ValidateCampaignItems(CampaignTriggerType.SystemVariableChange, variable);
        }

        private void OnFileOpened(File openedFile)
        {
            ValidateCampaignItems(CampaignTriggerType.FileOpened, openedFile.UniqueId);
        }

        private void OnFileDecrypted(int fileId)
        {
            ValidateCampaignItems(CampaignTriggerType.FileDecrypted, fileId);
        }

        private void OnDeviceConnected()
        {
            ValidateCampaignItems(CampaignTriggerType.DeviceConnect, DeviceCollection.CurrentDevice.UniqueId);
        }

        private void OnDeviceCracked(int deviceCracked)
        {
            ValidateCampaignItems(CampaignTriggerType.DeviceCracked, deviceCracked);
        }

        private void OnFileUnlocked(int fileUnlocked)
        {
            ValidateCampaignItems(CampaignTriggerType.FileUnlocked, fileUnlocked);
        }

        private void OnProgramUnlocked(int programUnlocked)
        {
            ValidateCampaignItems(CampaignTriggerType.ProgramUnlocked, programUnlocked);
        }

        private void OnDeviceUnlocked(int programUnlocked)
        {
            ValidateCampaignItems(CampaignTriggerType.DeviceUnlocked, programUnlocked);
        }

        private void OnCampaignItemCompletedTrigget(int campaignItemCompleted)
        {
            ValidateCampaignItems(CampaignTriggerType.CampaignItemCompleted, campaignItemCompleted);
        }

        #endregion

        #region Campaign Items

        #region Load

        public void LoadCampaignItems(TextAsset serializedData)
        {
            UncompletedCampaignItems = new List<CampaignItem>();
            CompletedCampaignItems = new List<CampaignItem>();

            if (serializedData == null)
            {
                Debug.LogError("INVALIDA CAMPAIGN DATA!");
                return;
            }

            var content = serializedData.text;
            AllCampaignItems = JsonConvert.DeserializeObject<List<CampaignItem>>(content);

            UncompletedCampaignItems.AddRange(AllCampaignItems);
            UncompletedCampaignItems.RemoveAll(i => Info.CompletedCampaignItems.Contains(i.Id));

            CompletedCampaignItems.AddRange(AllCampaignItems);
            CompletedCampaignItems.RemoveAll(i => UncompletedCampaignItems.Contains(i));
        }

        #endregion

        #region Validations

        public void ValidateCampaignItems(CampaignTriggerType typeToValidate, object data)
        {
            var itemsToValidate = UncompletedCampaignItems.FindAll(i => i.Trigger == typeToValidate);
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
            var result = ValidateDependencies(item);
            switch (item.Trigger)
            {
                case CampaignTriggerType.FileOpened:
                case CampaignTriggerType.FileDecrypted:
                case CampaignTriggerType.DeviceCracked:
                case CampaignTriggerType.FileUnlocked:
                case CampaignTriggerType.ProgramUnlocked:
                case CampaignTriggerType.DeviceUnlocked:
                case CampaignTriggerType.CampaignItemCompleted:
                    result &= ValidateIntItem(item, data);
                    break;
                case CampaignTriggerType.SystemVariableChange:
                    result &= ValidateSystemVariableChange(item, data);
                    break;
                case CampaignTriggerType.NewGameStarted:
                    result &= IsFirstTimeInGame;
                    break;
                case CampaignTriggerType.DeviceConnect:
                    result &= ValidateStringItem(item, data);
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
            if (!int.TryParse(item.TriggerAditionalData, out fileId))
            {
                Debug.LogError("ERROR TRYING TO VALIDATE CAMPAIGN ITEM {0}. DATA {1}: ".InLineFormat(item.Id, item.ActionAditionalData));
            }

            return (int)openFileId == fileId;
        }

        public bool ValidateSystemVariableChange(CampaignItem item, object systemVariable)
        {
            if (!Enum.IsDefined(typeof(SystemVariableType), item.TriggerAditionalData))
            {
                Debug.LogError("ERROR TRYING TO VALIDATE CAMPAIGN ITEM {0}. SYSTEM VARIABLE NOT PARSED: {1}".InLineFormat(item.Id, item.ActionAditionalData));
            }

            return (SystemVariableType)Enum.Parse(typeof(SystemVariableType), item.TriggerAditionalData) == (SystemVariableType)systemVariable;
        }

        public bool ValidateStringItem(CampaignItem item, object systemVariable)
        {
            return item.TriggerAditionalData == systemVariable.ToString();
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
                    ExecuteUnlockFileCampaignItem(item);
                    break;
                case CampaignActionType.UnlockProgram:
                    ExecuteUnlockProgram(item);
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
                Info.UnlockedDevices.Add(devices[i].GetHashCode());
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

        public void ExecuteUnlockProgram(CampaignItem item)
        {
            var programIds = item.ActionAditionalData.Split(',');
            for (int i = 0; i < programIds.Length; i++)
            {
                int programId;
                if (!int.TryParse(programIds[i], out programId))
                {
                    Debug.LogError("ERROR PARSING ADITIONAL DATA {0} WHEN UNLOCK PROGRAM FROM CAMPAIGN ITEM ID {1}".InLineFormat(programIds[i], item.Id));
                }

                Info.UnlockPrograms.Add(programId);
            }
        }

        #endregion

        #endregion
    }
}