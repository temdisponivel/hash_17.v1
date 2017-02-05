using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using Hash17.Devices;
using Hash17.Devices.Firewalls;
using Hash17.Devices.Firewalls.Implementation;
using Hash17.Files;
using Hash17.MockSystem;
using Hash17.Programs;
using Hash17.Campaign;
using Hash17.Terminal_;
using Hash17.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Hash17.Blackboard_
{
    public class Blackboard : PersistentSingleton<Blackboard>
    {
        #region Properties

        #region References

        public GameConfiguration GameConfiguration;
        public TextAsset ProgramsSerializedData;
        public TextAsset DevicesSerializedData;

        #endregion

        #region ProgramsByCommand

        public Dictionary<string, Program> ProgramsByCommand = new Dictionary<string, Program>();
        public Dictionary<ProgramId, Program> SpecialPrograms = new Dictionary<ProgramId, Program>();
        public Dictionary<int, Program> ProgramDefinitionByUniqueId = new Dictionary<int, Program>();
        public Dictionary<ProgramId, Program> ProgramDefinitionById = new Dictionary<ProgramId, Program>();

        private List<Program> _programs;
        public List<Program> Programs
        {
            get
            {
                if (_programs == null || _programs.Count == 0)
                {
                    _programs = JsonConvert.DeserializeObject<List<Program>>(ProgramsSerializedData.text, new JsonSerializerSettings()
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        TypeNameHandling = TypeNameHandling.All,
                        TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                    });
                }

                return _programs;
            }
        }

        #endregion

        #region Devices

        public Dictionary<string, Device> DevicesById = new Dictionary<string, Device>();

        private List<Device> _devices;
        public List<Device> Devices
        {
            get
            {
                if (_devices == null || _devices.Count == 0)
                {
                    _devices = JsonConvert.DeserializeObject<List<Device>>(DevicesSerializedData.text, new JsonSerializerSettings()
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        TypeNameHandling = TypeNameHandling.All,
                        TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                    });
                }

                return _devices;
            }
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
        
        #region Files

        public FileSystem FileSystem
        {
            get
            {
                return CurrentDevice.FileSystem;
            }
        }
        public List<File> AllFiles { get; protected set; }
        public List<Directory> AllDirectories { get; set; }

        #endregion

        #region Firewall

        public Dictionary<FirewallType, IFirewall> Firewalls;

        #endregion

        #region System
        
        public readonly SystemVariables SystemVariable = new SystemVariables();
        public readonly CampaignManager CampaignManager = new CampaignManager();

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
            LoadSystemVariables();
        }

        private void LoadPrograms()
        {
            var allPrograms = Programs;

            if (allPrograms == null)
                return;

            for (int i = 0; i < allPrograms.Count; i++)
            {
                var program = allPrograms[i];

                ProgramDefinitionById[program.Id] = program;
                ProgramsByCommand[program.Command] = program;
                ProgramDefinitionByUniqueId[program.UnitqueId] = program;

                if (!program.Global)
                    SpecialPrograms[program.Id] = program.Clone();
            }
        }

        public void LoadDeviceCollection()
        {
            for (int i = 0; i < Devices.Count; i++)
            {
                AddFilesAndDirToList(Devices[i].FileSystem);
                DevicesById[Devices[i].UniqueId] = Devices[i];
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
            if (GameConfiguration == null)
                GameConfiguration = Resources.LoadAll<GameConfiguration>("")[0];
        }

        public void LoadSystemVariables()
        {
            SystemVariable[SystemVariableType.USERNAME] = "unknown";
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

        public void Bake()
        {
            GameConfiguration = Resources.LoadAll<GameConfiguration>("")[0];
            ProgramsSerializedData = Resources.Load<TextAsset>(Alias.GameConfig.CollectionLoadPath + "ProgramCollectionData");
            DevicesSerializedData = Resources.Load<TextAsset>(Alias.GameConfig.CollectionLoadPath + "DeviceCollectionData");
        }

        #endregion
    }
}