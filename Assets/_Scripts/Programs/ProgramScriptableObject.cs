using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Hash17.Programs
{
    [CreateAssetMenu(fileName = "New Program", menuName = "Hash17/Create program", order = 0)]
    public class ProgramScriptableObject : ScriptableObject
    {
        public ProgramId Id;
        public string Command;
        public string Description;
        public string Usage;
        public string PrefabPath;
        public string[] KnownParametersAndOptions;
    }
}
