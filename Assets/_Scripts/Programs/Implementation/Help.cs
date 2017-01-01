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

            Terminal.Instance.BeginIdentation();

            var programs = Blackboard.Instance.Programs;

            foreach (var program in programs)
            {
                int id;
                if (Blackboard.Instance.CurrentDevice.SpecialPrograms.TryGetValue(program.Value.Id, out id))
                {
                    var prog = Blackboard.Instance.ProgramDefinitionByUniqueId[id];
                    if (prog != null)
                    {
                        Terminal.Showtext(prog.Command);
                        Terminal.Showtext(prog.Description, true);
                        yield return null;
                        continue;
                    }
                }

                if (!program.Value.Global)
                    continue;

                Terminal.Showtext(program.Key);
                Terminal.Showtext(program.Value.Description, true);
                yield return null;
            }

            Terminal.Instance.EndIdentation();

            UnblockInput();
        }
    }
}
