using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Hash17.Devices;
using Hash17.Devices.Firewalls;
using Hash17.Devices.Firewalls.Implementation;
using Hash17.Files;
using Hash17.Programs;
using Hash17.Terminal_;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Blackboard_
{
    public class Blackboard : PersistentSingleton<Blackboard>
    {
        #region Properties

        #region References

        public GameConfiguration GameConfiguration;

        #endregion

        #region Programs

        public Dictionary<string, Program> Programs = new Dictionary<string, Program>();
        public Dictionary<ProgramId, Program> SpecialPrograms = new Dictionary<ProgramId, Program>();
        public Dictionary<int, Program> ProgramDefinitionByUniqueId = new Dictionary<int, Program>();
        public Dictionary<ProgramId, Program> ProgramDefinitionById = new Dictionary<ProgramId, Program>();

        #endregion

        #region Devices

        public Dictionary<string, Device> DevicesById = new Dictionary<string, Device>();
        public DeviceCollection DeviceCollection;
        public List<Device> Devices
        {
            get { return DeviceCollection.Devices; }
        }
        private Device _currentDevice;
        public Device CurrentDevice
        {
            get
            {
                if (_currentDevice == null)
                    _currentDevice = Devices.Find(d => d.UniqueId == GameConfiguration.OwnedDeviceId);

                return _currentDevice;
            }
            set
            {
                _currentDevice = value;
                Alias.Term.UpdateUserNameLocation();
            }
        }

        #endregion

        #region Unlocks

        public HashSet<int> UnlockedFiles = new HashSet<int>();
        public HashSet<string> UnlockedDevices = new HashSet<string>();

        #endregion

        #region Files

        public FileSystem FileSystem
        {
            get
            {
                return CurrentDevice.FileSystem;
            }
        }
        public List<File> AllFiles { get; protected set; }
        public List<Directory> AllDirectories { get; protected set; }

        #endregion

        #region Firewall

        public Dictionary<FirewallType, IFirewall> Firewalls;

        #endregion

        #endregion

        #region Unity events

        protected override void Awake()
        {
            AllFiles = new List<File>();
            AllDirectories = new List<Directory>();
            LoadAll();
        }

        #endregion
        
        #region Load

        public void LoadAll()
        {
            LoadGameConfiguration();
            LoadPrograms();
            LoadDeviceCollection();
            LoadFirewalls();
        }

        private void LoadPrograms()
        {
            var progCollection = Resources.LoadAll<ProgramCollection>("")[0];
            progCollection.Load();
            var allPrograms = progCollection.Programs;

            if (allPrograms == null)
                return;

            for (int i = 0; i < allPrograms.Count; i++)
            {
                var program = allPrograms[i];

                ProgramDefinitionById[program.Id] = program;
                Programs[program.Command] = program;
                ProgramDefinitionByUniqueId[program.UnitqueId] = program;

                if (!program.Global)
                    SpecialPrograms[program.Id] = program.Clone();
            }
        }

        public void LoadDeviceCollection()
        {
            DeviceCollection = Resources.LoadAll<DeviceCollection>("")[0];
            DeviceCollection.Load();
            for (int i = 0; i < DeviceCollection.Devices.Count; i++)
            {
                AddFilesAndDirToList(DeviceCollection.Devices[i].FileSystem);
                DevicesById[DeviceCollection.Devices[i].UniqueId] = DeviceCollection.Devices[i];
            }
        }

        public void LoadFirewalls()
        {
            Firewalls = new Dictionary<FirewallType, IFirewall>();
            Firewalls[FirewallType.None] = new NoFirewall();
            Firewalls[FirewallType.Password] = new PasswordFirewall();
        }

        public void LoadGameConfiguration()
        {
            GameConfiguration = Resources.LoadAll<GameConfiguration>("")[0];
        }

        #endregion

        #region Helpers
        
        private void AddFilesAndDirToList(Directory dir)
        {
            AllDirectories.Add(dir);

            if (dir.Files != null)
            {
                AllFiles.AddRange(dir.Files);
            }

            if (dir.Childs != null)
            {
                for (int i = 0; i < dir.Childs.Count; i++)
                {
                    AddFilesAndDirToList(dir.Childs[i]);
                }
            }
        }

        #endregion
    }
}