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
        public readonly Dictionary<string, IProgram> Programs = new Dictionary<string, IProgram>();
        public readonly Dictionary<int, ProgramScriptableObject> ProgramDefinitionById = new Dictionary<int, ProgramScriptableObject>();

        protected override void Awake()
        {
            base.Awake();
            LoadProgramsDefinitions();
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
    }
}
