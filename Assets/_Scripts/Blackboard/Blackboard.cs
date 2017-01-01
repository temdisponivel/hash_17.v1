using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Hash17.Devices;
using Hash17.Devices.Firewalls;
using Hash17.Files;
using Hash17.Programs;
using Hash17.Terminal_;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Blackboard_
{
    public class Blackboard : PersistentSingleton<Blackboard>
    {
        public Dictionary<string, Program> Programs = new Dictionary<string, Program>();
        public Dictionary<ProgramId, Program> SpecialPrograms = new Dictionary<ProgramId, Program>();
        public Dictionary<int, Program> ProgramDefinitionByUniqueId = new Dictionary<int, Program>();
        public Dictionary<ProgramId, Program> ProgramDefinitionById = new Dictionary<ProgramId, Program>();
        public DeviceCollection DeviceCollection;

        public List<Device> Devices
        {
            get { return DeviceCollection.Devices; }
        }

        public string OwnedDeviceId;

        private Device _currentDevice;
        public Device CurrentDevice
        {
            get
            {
                if (_currentDevice == null)
                    _currentDevice = Devices.Find(d => d.UniqueId == OwnedDeviceId);

                return _currentDevice;
            }
            set
            {
                _currentDevice = value; 
                Terminal.Instance.UpdateUserNameLocation();
            }
        }
        
        public FileSystem FileSystem
        {
            get
            {
                return CurrentDevice.FileSystem;
            }
        }

        public List<File> AllFiles { get; protected set; }
        public List<Directory> AllDirectories { get; protected set; }

        public Dictionary<FirewallType, IFirewall> Firewalls;

        protected override void Awake()
        {
            LoadAll();
        }

        public void LoadDeviceCollection()
        {
            DeviceCollection = Resources.LoadAll<DeviceCollection>("")[0];
            DeviceCollection.Load();
        }

        public void LoadAll()
        {
            LoadPrograms();
            LoadDeviceCollection();
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
    }
}