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
            LoadFirewalls();

            DeviceCollection.Load(DataHolder.DevicesSerializedData);
            ProgramCollection.Load(DataHolder.ProgramsSerializedData);

            Device owned;
            if (!DeviceCollection.GetDeviceById(Alias.Config.OwnedDeviceId.GetHashCode(), out owned))
            {
                Debug.LogError("OWNED DEVICE NOT UNLOCKED OR INVALID");
            }

            DeviceCollection.ChangeCurrentDevice(owned);

            CampaignManager.OnGameStarted();

            if (CampaignManager.IsFirstTimeInGame)
                SystemVariables[SystemVariableType.USERNAME] = Alias.Config.DefaultUserName;

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