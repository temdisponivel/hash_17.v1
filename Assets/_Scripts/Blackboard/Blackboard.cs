using System.Collections.Generic;
using Hash17.Files;
using Hash17.Files.SO;
using Hash17.Programs;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Blackboard_
{
    public class Blackboard : PersistentSingleton<Blackboard>
    {
        #region Properties

        public readonly Dictionary<string, IProgram> Programs = new Dictionary<string, IProgram>();
        public readonly Dictionary<ProgramId, IProgram> SpecialPrograms = new Dictionary<ProgramId, IProgram>();
        public readonly Dictionary<ProgramId, ProgramScriptableObject> ProgramDefinitionById = new Dictionary<ProgramId, ProgramScriptableObject>();
        public FileSystem FileSystem = new FileSystem();

        [SerializeField]
        public ProgramScriptableObject[] ProgramsScriptableObjects;

        [SerializeField]
        public FileSystemScriptableObject FileSystemScriptableObject;

        public List<File> AllFiles { get; protected set; }
        public List<Directory> AllDirectories { get; protected set; }

        #endregion

        #region setup 

        protected override void Awake()
        {
            base.Awake();
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
