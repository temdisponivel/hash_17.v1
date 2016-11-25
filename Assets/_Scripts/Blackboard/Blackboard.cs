using System.Collections.Generic;
using Hash17.Files;
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
        public readonly FileSystem FileSystem = new FileSystem();

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
                Programs.Add(programs[i].Command, Resources.Load<GameObject>(programs[i].PrefabPath).GetComponent<IProgram>());
            }
        }

        void LoadFileSystem()
        {
            Directory dir;
            FileSystem.CreateDiretory("teste", out dir);

            File file;
            FileSystem.CreateFile(dir, "teste", out file);
            FileSystem.UpdateFileContent(string.Format("{0}{1}{2}", dir.Path, FileSystem.DirectorySeparator, file.Name), "ALO ALO");
        }

        #endregion
    }
}
