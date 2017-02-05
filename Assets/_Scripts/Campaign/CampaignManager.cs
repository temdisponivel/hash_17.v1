using System;
using System.Collections.Generic;
using Hash17.MockSystem;
using Hash17.Programs;
using Hash17.Terminal_;
using Hash17.Utils;

namespace Hash17.Campaign
{
    public class CampaignManager : NonUnitySingleton<CampaignManager>
    {
        #region Properties

        public bool HasSetUserName { get; set; }
        public HashSet<int> UnlockedFiles = new HashSet<int>();
        public HashSet<string> UnlockedDevices = new HashSet<string>();

        #endregion

        #region Setup

        public void OnGameStarted()
        {
            HasSetUserName = Alias.Board.SystemVariable.ContainsKey(SystemVariableType.USERNAME);
            Alias.Board.SystemVariable.OnSystemVariableChange += OnSystemVariableChanged;
        }

        #endregion

        #region Programs

        public bool CanRunProgram(ProgramId program, string parameters, out string message)
        {
            message = string.Empty;
            if (program == ProgramId.Set && parameters.Contains(SystemVariableType.USERNAME.ToString()))
                return true;

            if (HasSetUserName)
                return true;

            message = "You must set your user name before using another program.\nUse 'set USERNAME <user_name>' to set your user name.";
            return HasSetUserName;
        }

        #endregion

        #region Callbacks
        
        public void OnSystemVariableChanged(SystemVariableType variable)
        {
            if (variable == SystemVariableType.USERNAME)
                HasSetUserName = true;
        }

        #endregion
    }
}