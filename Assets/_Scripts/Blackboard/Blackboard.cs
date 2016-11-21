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
            Directory dir = FileSystem;
            for (int i = 0; i < 5; i++)
            {
                FileSystem.CreateDiretory(FileSystem, "teste " + i, out dir);
            }

            for (int i = 0; i < FileSystem.Childs.Count; i++)
            {
                for (int j = 0; j < 5; j++)
                    FileSystem.CreateDiretory(FileSystem.Childs[i], "teste " + j, out dir);
            }

            Debug.Log(FileSystem.FindDirectory("c:/Teste 0/Teste 1/../../Teste 0", true));
        }

        #endregion
    }
}
