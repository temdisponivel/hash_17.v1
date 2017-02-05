using System.Collections.Generic;
using Hash17.Programs;

namespace Hash17.MockSystem
{
    public class ProgramCollection : Collection<Program>
    {
        #region Properties

        public readonly Dictionary<string, Program> ProgramsByCommand = new Dictionary<string, Program>();
        public readonly Dictionary<int, Program> ProgramsById = new Dictionary<int, Program>();
        public readonly Dictionary<ProgramId, Program> SpecialPrograms = new Dictionary<ProgramId, Program>();

        #endregion

        #region Overrides

        public override void Add(Program item)
        {
            ProgramsByCommand[item.Command] = item;
            ProgramsById[item.UniqueId] = item;
            if (!item.Global)
                SpecialPrograms[item.Id] = item;
            base.Add(item);
        }

        #endregion
    }
}