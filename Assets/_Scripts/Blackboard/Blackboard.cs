using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Hash17.Devices;
using Hash17.Files;
using Hash17.Files.SO;
using Hash17.Programs;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Blackboard_
{
    public class Blackboard : PersistentSingleton<Blackboard>
    {
        public readonly Dictionary<string, IProgram> Programs = new Dictionary<string, IProgram>();
        public readonly Dictionary<ProgramId, IProgram> SpecialPrograms = new Dictionary<ProgramId, IProgram>();
        public readonly Dictionary<ProgramId, ProgramScriptableObject> ProgramDefinitionById = new Dictionary<ProgramId, ProgramScriptableObject>();

        public FileSystem FileSystem
        {
            get { return CurrentConnectedDevice.FileSystem; }
        }

        public IDevice CurrentConnectedDevice;
        public IDevice OwnDevice;

        [SerializeField]
        public ProgramScriptableObject[] ProgramsScriptableObjects;

        [SerializeField]
        public FileSystemScriptableObject[] FileSystemScriptableObjects;

        public List<File> AllFiles { get; protected set; }
        public List<Directory> AllDirectories { get; protected set; }

        protected override void Awake()
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
            UpdateAllDirectories();
            UpdateAllFiles();
        }

        public void UpdateAllFiles()
        {
            var fileSystems = FileSystemScriptableObjects;
            var files = new List<File>();
            for (int i = 0; i < fileSystems.Length; i++)
            {
                var fileSystem = fileSystems[i].ToFileSystem();
                files.AddRange(fileSystem.Files);
                for (int j = 0; j < AllDirectories.Count; j++)
                {
                    var currentdir = AllDirectories[j];
                    files.AddRange(currentdir.Files);
                }
            }

            AllFiles = files;
        }

        public void UpdateAllDirectories()
        {
            var fileSystems = FileSystemScriptableObjects;
            var toSee = new List<Directory>();
            for (int i = 0; i < fileSystems.Length; i++)
            {
                var fileSystem = fileSystems[i].ToFileSystem();
                var root = fileSystem as Directory;
                toSee.Add(root);
                for (int j = 0; j < toSee.Count; j++)
                {
                    root = toSee[j];
                    for (int k = 0; k < root.Childs.Count; k++)
                    {
                        toSee.Add(root.Childs[k]);
                    }
                }
            }

            AllDirectories = toSee;
        }
    }
}