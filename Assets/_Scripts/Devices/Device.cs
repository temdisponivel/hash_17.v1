using System;
using System.Collections;
using System.Collections.Generic;
using Hash17.Devices.Networks;
using Hash17.Devices.Security;
using Hash17.Devices.Security.Implementation;
using Hash17.Files;
using Hash17.Files.SO;
using Hash17.Programs;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Devices
{
    public class Device : Singleton<Device>, IDevice
    {
        #region Properties

        public string Name { get; set; }
        public string Address { get; set; }
        public INetwork Network { get; set; }
        public string Password { get; set; }
        
        public IDictionary<string, IProgram> Programs { get; set; }
        public IDictionary<ProgramId, IProgram> SpecialPrograms { get; set; }
        public Dictionary<ProgramId, ProgramScriptableObject> ProgramDefinitionById { get; set; }
        public FileSystem FileSystem { get; set; }

        [SerializeField]
        public ProgramScriptableObject[] ProgramsScriptableObjects;

        [SerializeField]
        public FileSystemScriptableObject FileSystemScriptableObject;

        public List<File> AllFiles { get; protected set; }
        public List<Directory> AllDirectories { get; protected set; }

        #endregion

        #region Interfaces

        public void Access(string firewallPassCode, Action<bool, string, IDevice> callback)
        {
            if (firewallPassCode != Password)
            {
                callback(true, Password, this);
                return;
            }

            new BasicFirewall().Crack((result) =>
            {
                var passCode = string.Empty;
                IDevice device = null;
                if (result)
                {
                    passCode = Password;
                    device = this;
                }

                callback(result, passCode, device);
            });
        }

        #endregion

        #region setup 

        protected void Awake()
        {
            LoadProgramsDefinitions();
            LoadFileSystem();
        }

        void LoadProgramsDefinitions()
        {
            var programs = ProgramsScriptableObjects;
            for (int i = 0; i < programs.Length; i++)
            {
                var program = programs[i];
                ProgramDefinitionById.Add(program.Id, program);

                if (program.AvailableInGamePlay)
                    Programs.Add(programs[i].Command, program.ProgramPrefab.GetComponent<IProgram>());
                else
                    SpecialPrograms.Add(programs[i].Id, program.ProgramPrefab.GetComponent<IProgram>());
            }
        }

        protected void LoadFileSystem()
        {
            FileSystem = FileSystemScriptableObject.ToFileSystem();
            UpdateAllDirectories();
            UpdateAllFiles();
        }

        public void UpdateAllFiles()
        {
            var files = new List<File>(FileSystem.Files);
            for (int i = 0; i < AllDirectories.Count; i++)
            {
                var currentdir = AllDirectories[i];
                files.AddRange(currentdir.Files);
            }

            AllFiles = files;
        }

        public void UpdateAllDirectories()
        {
            var root = FileSystem as Directory;
            var toSee = new List<Directory>();
            toSee.Add(root);
            for (int j = 0; j < toSee.Count; j++)
            {
                root = toSee[j];
                for (int i = 0; i < root.Childs.Count; i++)
                {
                    toSee.Add(root.Childs[i]);
                }
            }

            AllDirectories = toSee;
        }

        #endregion
    }
}
