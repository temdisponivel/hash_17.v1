using System;
using System.Collections;
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
            public Hash17HashSet<int> DecryptedDevices { get; set; }

            [JsonProperty("CF")]
            public Hash17HashSet<int> DecryptedFiles { get; set; }

            [JsonProperty("UF")]
            public Hash17HashSet<int> UnlockedFiles { get; set; }

            [JsonProperty("UDV")]
            public Hash17HashSet<int> UnlockedDevices { get; set; }

            [JsonProperty("CCI")]
            public Hash17HashSet<int> CompletedCampaignMissions { get; set; }

            [JsonProperty("UP")]
            public Hash17HashSet<int> UnlockedPrograms { get; set; }

            [JsonProperty("OF")]
            public Hash17HashSet<int> OpenedFiles { get; set; }

            [JsonProperty("OF")]
            public Hash17HashSet<int> ConnectedDevices { get; set; }

            [JsonProperty("OF")]
            public Hash17HashSet<string> SystemVariablesSet { get; set; }

            public CampaignInfo()
            {
                InitializeIfNull();
            }

            public void InitializeIfNull()
            {
                if (DecryptedDevices == null)
                    DecryptedDevices = new Hash17HashSet<int>();
                if (DecryptedFiles == null)
                    DecryptedFiles = new Hash17HashSet<int>();
                if (UnlockedFiles == null)
                    UnlockedFiles = new Hash17HashSet<int>();
                if (UnlockedDevices == null)
                    UnlockedDevices = new Hash17HashSet<int>();
                if (CompletedCampaignMissions == null)
                    CompletedCampaignMissions = new Hash17HashSet<int>();
                if (UnlockedPrograms == null)
                    UnlockedPrograms = new Hash17HashSet<int>();
                if (OpenedFiles == null)
                    OpenedFiles = new Hash17HashSet<int>();
                if (ConnectedDevices == null)
                    ConnectedDevices = new Hash17HashSet<int>();
                if (SystemVariablesSet == null)
                    SystemVariablesSet = new Hash17HashSet<string>();
            }
        }

        #endregion

        #region Properties

        public bool IsFirstTimeInGame { get; set; }
        public CampaignInfo Info { get; private set; }
        public List<CampaignMission> AllCampaignItems { get; set; }
        public List<CampaignMission> UncompletedCampaignItems { get; set; }
        public List<CampaignMission> CompletedCampaignItems { get; set; }
        public Dictionary<int, CampaignMissionReward> CampaignMissionRewards { get; set; }

        public string SavePath
        {
            get { return "{0}{1}".InLineFormat(Application.persistentDataPath, Alias.Config.CampaignSavePath); }
        }

        public event Action<CampaignMission> OnCampaignItemCompleted;

        #endregion

        #region Setup

        public void OnGameStarted()
        {
            Alias.SysVariables.OnSystemVariableChange += OnSystemVariableChanged;
            File.OnFileOpened += OnFileOpened;
            File.OnFileDecrypted += OnFileDecrypted;
            Device.OnDecrypted += OnDeviceDecrypted;
            Alias.Devices.OnCurrentDeviceChange += OnDeviceConnected;

            if (IsFirstTimeInGame)
            {
                CampaignMissionRewards[Alias.Config.GameStartCampaignItemReward].Execute();
            }
        }

        public void LoadStuff()
        {
            LoadProgress();
            LoadCampaignMissions(Alias.DataHolder.CampaignMissionsSerializedData);
            LoadCampaignItemsRewards(Alias.DataHolder.CampaignMissionsRewardSerializedData);
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
            }
            info.InitializeIfNull();
            Info = info;
            Alias.SysVariables.Add(SystemVariables.USERNAME, Info.PlayerName);
        }

        #endregion

        #region Campaign Items

        #region Load

        public void LoadCampaignMissions(TextAsset serializedData)
        {
            UncompletedCampaignItems = new List<CampaignMission>();
            CompletedCampaignItems = new List<CampaignMission>();

            if (serializedData == null)
            {
                Debug.LogError("INVALIDA CAMPAIGN DATA!");
                return;
            }

            var content = serializedData.text;
            AllCampaignItems = JsonConvert.DeserializeObject<List<CampaignMission>>(content);

            UncompletedCampaignItems.AddRange(AllCampaignItems);
            UncompletedCampaignItems.RemoveAll(i => Info.CompletedCampaignMissions.Contains(i.Id));

            CompletedCampaignItems.AddRange(AllCampaignItems);
            CompletedCampaignItems.RemoveAll(i => UncompletedCampaignItems.Contains(i));

            for (int i = 0; i < AllCampaignItems.Count; i++)
            {
                var current = AllCampaignItems[i];
                current.ValidateCompletion();
                current.OnComplete += OnCampaignMissionCompleted;

                if (!current.Completed)
                    current.Register();
            }
        }

        public void LoadCampaignItemsRewards(TextAsset serializedData)
        {
            if (serializedData == null)
            {
                Debug.LogError("INVALIDA CAMPAIGN REWARDS DATA!");
                return;
            }

            CampaignMissionRewards = new Dictionary<int, CampaignMissionReward>();

            var content = serializedData.text;
            var campaignMissionRewards = JsonConvert.DeserializeObject<List<CampaignMissionReward>>(content);

            for (int i = 0; i < campaignMissionRewards.Count; i++)
            {
                CampaignMissionRewards.Add(campaignMissionRewards[i].Id, campaignMissionRewards[i]);
            }
        }

        #endregion

        #region Complete

        private void OnCampaignMissionCompleted(CampaignMission mission)
        {
            RunMissionRewards(mission);
        }

        private void RunMissionRewards(CampaignMission mission)
        {
            for (int i = 0; i < mission.CampaignMissionReward.Count; i++)
            {
                var reward = CampaignMissionRewards[mission.CampaignMissionReward[i]];
                reward.Execute();
            }

            Info.CompletedCampaignMissions.Add(mission.Id);

            if (OnCampaignItemCompleted != null)
                OnCampaignItemCompleted(mission);
        }

        #endregion

        #endregion

        #region Callbacks

        private void OnSystemVariableChanged(string variable)
        {
            if (variable == SystemVariables.USERNAME)
                Info.PlayerName = Alias.SysVariables[SystemVariables.USERNAME];

            Info.SystemVariablesSet.Add(variable);
        }

        private void OnFileOpened(File openedFile)
        {
            Info.OpenedFiles.Add(openedFile.UniqueId);
        }

        private void OnFileDecrypted(File file)
        {
            Info.DecryptedFiles.Add(file.UniqueId);
        }

        private void OnDeviceConnected()
        {
            Info.ConnectedDevices.Add(DeviceCollection.CurrentDevice.UniqueId);
        }

        private void OnDeviceDecrypted(Device device)
        {
            Info.DecryptedDevices.Add(device.UniqueId);
        }

        #endregion
        
    }
}