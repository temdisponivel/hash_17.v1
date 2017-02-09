using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Hash17.Files;
using Hash17.MockSystem;
using Hash17.Utils;

namespace Hash17.Campaign
{
    public class CampaignMission
    {
        #region Properties

        public int Id;
        public int CampaignMissionReward;
        public bool Completed;
        public event Action<CampaignMission> OnComplete;

        #endregion

        #region To Complete

        public List<int> FilesToOpen;
        public List<int> FilesToDecrypt;
        public List<int> DevicesToConnect;
        public List<int> DevicesToDecrypt;
        public List<string> SystemVariablesToSet;
        public List<int> CampaignMissionToComplete;
        public List<int> ProgramsToUnlock;

        #endregion

        #region Progression

        public float GeneralProgression
        {
            get
            {
                var sum = FilesOpenedProgression + FilesDecryptedProgression + DevicesConnectProgression +
                          DevicesDecryptedProgression + SystemVariablesProgression + ProgramsProgression;
                return sum/6f;
            }
        }

        public float FilesOpenedProgression
        {
            get
            {
                var files = 0;
                for (int i = 0; i < FilesToOpen.Count; i++)
                {
                    var file = FilesToOpen[i];
                    if (Alias.Campaign.Info.OpenedFiles.Contains(file))
                        files++;
                }

                return files/Convert.ToSingle(FilesToOpen.Count);
            }
        }

        public float FilesDecryptedProgression
        {
            get
            {
                var files = 0;
                for (int i = 0; i < FilesToDecrypt.Count; i++)
                {
                    var file = FilesToDecrypt[i];
                    if (Alias.Campaign.Info.DecryptedFiles.Contains(file))
                        files++;
                }

                return files / Convert.ToSingle(FilesToDecrypt.Count);
            }
        }

        public float DevicesConnectProgression
        {
            get
            {
                var devies = 0;
                for (int i = 0; i < DevicesToConnect.Count; i++)
                {
                    var device = DevicesToConnect[i];
                    if (Alias.Campaign.Info.ConnectedDevices.Contains(device))
                        devies++;
                }

                return devies / Convert.ToSingle(DevicesToConnect.Count);
            }
        }

        public float DevicesDecryptedProgression
        {
            get
            {
                var files = 0;
                for (int i = 0; i < DevicesToDecrypt.Count; i++)
                {
                    var device = DevicesToDecrypt[i];
                    if (Alias.Campaign.Info.DecryptedDevices.Contains(device))
                        files++;
                }

                return files / Convert.ToSingle(DevicesToDecrypt.Count);
            }
        }

        public float SystemVariablesProgression
        {
            get
            {
                var variables = 0;
                for (int i = 0; i < SystemVariablesToSet.Count; i++)
                {
                    var variable = SystemVariablesToSet[i];
                    if (Alias.Campaign.Info.SystemVariablesSet.Contains(variable))
                        variables++;
                }

                return variables / Convert.ToSingle(SystemVariablesToSet.Count);
            }
        }

        public float CampaignMissionProgression
        {
            get
            {
                var missions = 0;
                for (int i = 0; i < CampaignMissionToComplete.Count; i++)
                {
                    var variable = CampaignMissionToComplete[i];
                    if (Alias.Campaign.Info.CompletedCampaignMissions.Contains(variable))
                        missions++;
                }

                return missions / Convert.ToSingle(CampaignMissionToComplete.Count);
            }
        }

        public float ProgramsProgression
        {
            get
            {
                var programs = 0;
                for (int i = 0; i < ProgramsToUnlock.Count; i++)
                {
                    var program = ProgramsToUnlock[i];
                    if (Alias.Campaign.Info.UnlockedPrograms.Contains(program))
                        programs++;
                }

                return programs / Convert.ToSingle(ProgramsToUnlock.Count);
            }
        }

        #endregion

        #region Callback

        public void Register()
        {
            Alias.Campaign.Info.OpenedFiles.OnItemAdded += OnFileOpened;
            Alias.Campaign.Info.DecryptedFiles.OnItemAdded += OnFileDecrypted;
            Alias.Campaign.Info.ConnectedDevices.OnItemAdded += OnDeviceConnected;
            Alias.Campaign.Info.DecryptedDevices.OnItemAdded += OnDeviceDecrypted;
            Alias.Campaign.Info.SystemVariablesSet.OnItemAdded += OnSystemVariableSet;
            Alias.Campaign.Info.CompletedCampaignMissions.OnItemAdded += OnCampaignMissionCompleted;
            Alias.Campaign.Info.UnlockedPrograms.OnItemAdded += OnProgramUnlocked;
        }

        public void Unregister()
        {
            Alias.Campaign.Info.OpenedFiles.OnItemAdded -= OnFileOpened;
            Alias.Campaign.Info.DecryptedFiles.OnItemAdded -= OnFileDecrypted;
            Alias.Campaign.Info.ConnectedDevices.OnItemAdded -= OnDeviceConnected;
            Alias.Campaign.Info.DecryptedDevices.OnItemAdded -= OnDeviceDecrypted;
            Alias.Campaign.Info.SystemVariablesSet.OnItemAdded -= OnSystemVariableSet;
            Alias.Campaign.Info.CompletedCampaignMissions.OnItemAdded -= OnCampaignMissionCompleted;
            Alias.Campaign.Info.UnlockedPrograms.OnItemAdded -= OnProgramUnlocked;
        }

        private void OnFileOpened(int file)
        {
            if (FilesToOpen.Contains(file))
                ValidateCompletion();
        }

        private void OnFileDecrypted(int file)
        {
            if (FilesToDecrypt.Contains(file))
                ValidateCompletion();
        }

        private void OnDeviceConnected(int device)
        {
            if (DevicesToConnect.Contains(device))
                ValidateCompletion();
        }

        private void OnDeviceDecrypted(int device)
        {
            if (DevicesToConnect.Contains(device))
                ValidateCompletion();
        }

        private void OnSystemVariableSet(string systemVariable)
        {
            if (SystemVariablesToSet.Contains(systemVariable))
                ValidateCompletion();
        }

        private void OnCampaignMissionCompleted(int campaignMission)
        {
            if (CampaignMissionToComplete.Contains(campaignMission))
                ValidateCompletion();
        }

        private void OnProgramUnlocked(int programUnlocked)
        {
            if (ProgramsToUnlock.Contains(programUnlocked))
                ValidateCompletion();
        }

        #endregion

        #region Validations

        public void ValidateCompletion()
        {
            if (GeneralProgression >= 1)
            {
                Completed = true;
                if (OnComplete != null)
                    OnComplete(this);
            }
        }

        #endregion
    }
}