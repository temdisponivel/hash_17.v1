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

            var programs = Alias.Programs.ProgramsByCommand;

            foreach (var program in programs)
            {
                int id;
                if (DeviceCollection.CurrentDevice.SpecialPrograms.TryGetValue(program.Value.Id, out id))
                {
                    var prog = Alias.Programs.ProgramsById[id];
                    if (prog != null)
                    {
                        Alias.Term.ShowText("Program command: {0} ".InLineFormat(prog.PrettyCommand));
                        Alias.Term.ShowText(prog.Description, ident: true);
                        yield return null;
                        continue;
                    }
                }

                if (!program.Value.Global)
                    continue;

                Alias.Term.ShowText("Program command: {0} ".InLineFormat(program.Value.PrettyCommand));
                Alias.Term.ShowText(program.Value.Description, ident: true);
                yield return null;
            }

            var message = TextBuilder.WarningText("You can see more about a program using the command '{0} -h'.\n Eg. 'open -h'."
                .InLineFormat(TextBuilder.BuildText("<program_command>", Alias.Config.ProgramColor)));
            Alias.Term.ShowText(message);

            UnblockInput();
        }
    }
}
