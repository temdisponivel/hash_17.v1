using System.Collections.Generic;
using Hash17.Campaign;
using Hash17.Data;
using Hash17.Devices;
using Hash17.Devices.Firewalls;
using Hash17.Devices.Firewalls.Implementation;
using Hash17.MockSystem;
using MockSystem;
using Hash17.Utils;
using MockSystem.Term;
using UnityEngine;

namespace Hash17.Game
{
    public class Hash17Game : PersistentSingleton<Hash17Game>
    {
        #region References

        public GameObject ToEnableAfterStart;

        public Terminal Terminal;
        public CampaignManager CampaignManager;
        public DataHolder DataHolder;
        public DeviceCollection DeviceCollection;
        public ProgramCollection ProgramCollection;
        public SystemVariables SystemVariables;
        public Dictionary<FirewallType, IFirewall> Firewalls;

        #endregion

        #region Unity Events

        [ContextMenu("AWAKE")]
        public override void Awake()
        {
            base.Awake();

            Terminal = GetComponentInChildren<Terminal>(true);
            DataHolder = GetComponentInChildren<DataHolder>(true);

            CampaignManager = new CampaignManager();
            DeviceCollection = new DeviceCollection();
            ProgramCollection = new ProgramCollection();
            SystemVariables = new SystemVariables();
            Firewalls = new Dictionary<FirewallType, IFirewall>();
        }

        [ContextMenu("START")]
        protected void Start()
        {
            // This order is important - keep this way

            LoadFirewalls();

            CampaignManager.LoadStuff();

            DeviceCollection.Load(DataHolder.DevicesSerializedData);
            ProgramCollection.Load(DataHolder.ProgramsSerializedData);

            CampaignManager.OnGameStarted();

            if (CampaignManager.IsFirstTimeInGame)
                SystemVariables[SystemVariables.USERNAME] = Alias.Config.DefaultUserName;

            // ----------------------------------------

            ToEnableAfterStart.gameObject.SetActive(true);

            transform.DetachChildren();
        }

        #endregion

        #region Firewalls

        private void LoadFirewalls()
        {
            Firewalls.Add(FirewallType.None, new NoFirewall());
            Firewalls.Add(FirewallType.Password, new PasswordFirewall());
        }

        #endregion
    }
}