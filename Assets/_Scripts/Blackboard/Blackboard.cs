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
        public readonly Dictionary<ProgramId, ProgramScriptableObject> ProgramDefinitionById = new Dictionary<ProgramId, ProgramScriptableObject>();
        public FileSystem FileSystem = new FileSystem();

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
            var programs = Resources.LoadAll<ProgramScriptableObject>("");
            for (int i = 0; i < programs.Length; i++)
            {
                ProgramDefinitionById.Add(programs[i].Id, programs[i]);

                string path = programs[i].PrefabPath;
                if (string.IsNullOrEmpty(path))
                {
                    path = string.Format("Programs/{0}/{0}", programs[i].Id);
                }

                Programs.Add(programs[i].Command, Resources.Load<GameObject>(path).GetComponent<IProgram>());
            }
        }

        protected void LoadFileSystem()
        {
            var fileSystem = Resources.LoadAll<FileSystemScriptableObject>("")[0];
            FileSystem = fileSystem.ToFileSystem();
        }

        #endregion
    }
}
