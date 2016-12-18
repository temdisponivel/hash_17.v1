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
        public Dictionary<string, IProgram> Programs = new Dictionary<string, IProgram>();
        public Dictionary<ProgramId, IProgram> SpecialPrograms = new Dictionary<ProgramId, IProgram>();
        public Dictionary<ProgramId, ProgramScriptableObject> ProgramDefinitionById = new Dictionary<ProgramId, ProgramScriptableObject>();
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
        }

        private void LoadPrograms()
        {
            var allPrograms = Resources.LoadAll<ProgramScriptableObject>("").ToList();

            for (int i = 0; i < allPrograms.Count; i++)
            {
                var program = allPrograms[i];

                ProgramDefinitionById[program.Id] = program;

                if (program.AvailableInGamePlay)
                    Programs[program.Command] = program.ProgramPrefab.GetComponent<IProgram>();
                else
                    SpecialPrograms[program.Id] = program.ProgramPrefab.GetComponent<IProgram>();
            }
        }
    }
}