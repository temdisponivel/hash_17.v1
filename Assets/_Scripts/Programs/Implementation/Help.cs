using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Terminal_;
using Hash17.Utils;

namespace Hash17.Programs.Implementation
{
    public class Help : Program
    {
        protected override IEnumerator InnerExecute()
        {
            BlockInput();

            var programs = Alias.Board.ProgramsByCommand;

            foreach (var program in programs)
            {
                int id;
                if (Alias.Board.CurrentDevice.SpecialPrograms.TryGetValue(program.Value.Id, out id))
                {
                    var prog = Alias.Board.ProgramDefinitionByUniqueId[id];
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
                .InLineFormat(TextBuilder.BuildText("<program_command>", Alias.GameConfig.ProgramColor)));
            Alias.Term.ShowText(message);

            UnblockInput();
        }
    }
}
