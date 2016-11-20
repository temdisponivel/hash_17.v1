using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Programs;
using Hash17.Programs.Implementation;
using Hash17.Utils;

namespace Hash17.Blackboard_
{
    public class Blackboard : PersistentSingleton<Blackboard>
    {
        public Clear ClearProgramPrefab;

        public readonly Dictionary<string, IProgram> Programs = new Dictionary<string, IProgram>();

        protected override void Awake()
        {
            base.Awake();
            Programs.Add("clear", ClearProgramPrefab);
        }
    }
}
