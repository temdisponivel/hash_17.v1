using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Data;
using Hash17.MockSystem;
using MockSystem;
using Hash17.Utils;

namespace Hash17.Programs.Implementation
{
    public class Help : Program
    {
        protected override IEnumerator InnerExecute()
        {
            BlockInput();

            var programs = Alias.Programs.GetAvailablePrograms();

            for (int i = 0; i < programs.Count; i++)
            {
                var program = programs[i];
                int id;
                if (DeviceCollection.CurrentDevice.SpecialPrograms.TryGetValue(program.Type, out id))
                {
                    Program prog;
                    if (Alias.Programs.GetProgramById(id, out prog))
                    {
                        Alias.Term.ShowText("Program command: {0} ".InLineFormat(prog.PrettyCommand));
                        Alias.Term.ShowText(prog.Description, ident: true);
                        yield return null;
                        continue;
                    }
                }

                if (!program.Global)
                    continue;

                Alias.Term.ShowText("Program command: {0} ".InLineFormat(program.PrettyCommand));
                Alias.Term.ShowText(program.Description, ident: true);
                yield return null;
            }

            var message = TextBuilder.WarningText("You can see more about a program using the command '{0} -h'.\n Eg. 'open -h'."
                .InLineFormat(TextBuilder.BuildText("<program_command>", Alias.Config.ProgramColor)));
            Alias.Term.ShowText(message);

            UnblockInput();
        }
    }
}
