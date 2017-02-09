using System.Collections.Generic;
using System.Runtime.InteropServices;
using Hash17.Programs;
using Hash17.Utils;

namespace Hash17.MockSystem
{
    public class ProgramCollection : Collection<Program>
    {
        #region Inner Type

        public enum ProgramRequestResult
        {
            Ok,
            NonExisting,
            NonGlobal,
        }

        #endregion
        
        #region Helpers

        public ProgramRequestResult GetProgramAndParameters(string commandLine, out Program program, out string parameters)
        {
            string programName;
            Interpreter.GetProgram(commandLine, out programName, out parameters);

            // If the program doesn't exists
            if (!Alias.Programs.GetProgramByCommand(programName, out program))
                return ProgramRequestResult.NonExisting;

            var device = DeviceCollection.CurrentDevice;

            // validation needed bcause this method can be called from the game start (where there's no current device)
            if (device != null)
            {
                var deviceProgramId = 0;

                // If the program exists, validate if the current device has a 
                // specific version of it
                if (device.SpecialPrograms.TryGetValue(program.Type, out deviceProgramId))
                {
                    var progBkp = program;

                    // if it doesn't, set the old version of the program again
                    if (!Alias.Programs.GetProgramById(deviceProgramId, out program))
                    {
                        program = progBkp;

                        // if the program is not global and current device don't have it, error
                        if (!program.Global)
                            return ProgramRequestResult.NonGlobal;
                    }
                }
            }

            return ProgramRequestResult.Ok;
        }

        public List<Program> GetAvailablePrograms()
        {
            return _all.FindAll(p => p.IsAvailable);
        }

        public bool GetProgramById(int uniqueId, out Program program)
        {
            program = _all.Find(p => p.UniqueId == uniqueId);
            return program != null && program.IsAvailable;
        }

        public bool GetProgramByCommand(string command, out Program program)
        {
            program = _all.Find(p => p.Command == command);
            return program != null && program.IsAvailable;
        }

        public bool GetSpecialProgramByType(ProgramType type, out Program program)
        {
            program = _all.Find(p => p.Type == type);
            return program != null && program.Global && program.IsAvailable;
        }

        public override void Add(Program item)
        {
            base.Add(item);
            if (item.StartUnlocked)
                Alias.Campaign.Info.UnlockedPrograms.Add(item.UniqueId);
        }

        #endregion
    }
}