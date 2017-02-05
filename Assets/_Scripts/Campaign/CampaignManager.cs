using System;
using System.Collections.Generic;
using Hash17.Files;
using Hash17.Game;
using Hash17.MockSystem;
using Hash17.Programs;
using MockSystem;
using Hash17.Utils;
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

        public string SavePath
        {
            get { return "{0}{1}".InLineFormat(Application.persistentDataPath, Alias.Config.CampaignSavePath); }
        }

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

        #region Programs

        public bool CanRunProgram(ProgramId program, string parameters, out string message)
        {
            message = string.Empty;
            if (program == ProgramId.Set && parameters.Contains(SystemVariableType.USERNAME.ToString()))
                return true;

            if (IsFirstTimeInGame)
                return true;

            message = "You must set your user name before using another program.\nUse 'set USERNAME <user_name>' to set your user name.";
            return IsFirstTimeInGame;
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
    }
}