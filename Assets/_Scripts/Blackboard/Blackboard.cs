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
        public Dictionary<ProgramId, Program> ProgramDefinitionById = new Dictionary<ProgramId, Program>();
        public DeviceCollectionScriptableObject DeviceCollectionScriptableObject;

        public List<Device> Devices
        {
            get { return DeviceCollectionScriptableObject.Devices; }
        }

        public string OwnedDeviceId;

        private Device _currentDevice;
        public Device CurrentDevice
        {
            get
            {
                if (_currentDevice == null)
                    _currentDevice = Devices.Find(d => d.Id == OwnedDeviceId);

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
            DeviceCollectionScriptableObject = Resources.LoadAll<DeviceCollectionScriptableObject>("")[0];
        }

        public void LoadAll()
        {
            LoadPrograms();
            LoadDeviceCollection();
        }

        private void LoadPrograms()
        {
            var allPrograms = Resources.LoadAll<ProgramCollection>("")[0].Programs;

            for (int i = 0; i < allPrograms.Count; i++)
            {
                var program = allPrograms[i];

                ProgramDefinitionById[program.Id] = program;

                if (program.AvailableInGamePlay)
                    Programs[program.Command] = program.Clone();
                else
                    SpecialPrograms[program.Id] = program.Clone();
            }
        }
    }
}