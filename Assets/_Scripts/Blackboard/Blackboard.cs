using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Programs;
using Hash17.Programs.Implementation;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Blackboard_
{
    public class Blackboard : PersistentSingleton<Blackboard>
    {
        public Clear ClearProgramPrefab;

        public readonly Dictionary<string, IProgram> Programs = new Dictionary<string, IProgram>();
        public readonly Dictionary<int, ProgramScriptableObject> ProgramDefinitionById = new Dictionary<int, ProgramScriptableObject>();

        protected override void Awake()
        {
            base.Awake();
            Programs.Add("clear", ClearProgramPrefab);
            LoadProgramsDefinitions();
        }

        void LoadProgramsDefinitions()
        {
            var programs = Resources.LoadAll<ProgramScriptableObject>("Programs");
            for (int i = 0; i < programs.Length; i++)
            {
                ProgramDefinitionById.Add(programs[i].Id, programs[i]);
            }
        }
    }
}
