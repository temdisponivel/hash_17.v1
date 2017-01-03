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

            Alias.Term.BeginIdentation();

            var programs = Alias.Board.Programs;

            foreach (var program in programs)
            {
                int id;
                if (Alias.Board.CurrentDevice.SpecialPrograms.TryGetValue(program.Value.Id, out id))
                {
                    var prog = Alias.Board.ProgramDefinitionByUniqueId[id];
                    if (prog != null)
                    {
                        Alias.Term.ShowText(prog.Command);
                        Alias.Term.ShowText(prog.Description, ident: true);
                        yield return null;
                        continue;
                    }
                }

                if (!program.Value.Global)
                    continue;

                Alias.Term.ShowText(program.Key);
                Alias.Term.ShowText(program.Value.Description, ident: true);
                yield return null;
            }

            Alias.Term.EndIdentation();

            UnblockInput();
        }
    }
}
