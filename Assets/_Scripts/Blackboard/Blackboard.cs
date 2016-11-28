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
                    SpecialPrograms.Add(programs[i].Id, program.ProgramPrefab.GetComponent<IProgram>());
                else
                    Programs.Add(programs[i].Command, program.ProgramPrefab.GetComponent<IProgram>());
            }
        }

        protected void LoadFileSystem()
        {
            FileSystem = FileSystemScriptableObject.ToFileSystem();
        }

        #endregion
    }
}
