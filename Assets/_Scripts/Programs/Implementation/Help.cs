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

            var programs = Blackboard.Instance.Programs;

            foreach (var program in programs)
            {
                Terminal.Showtext(program.Key);
                Terminal.Showtext("       " + program.Value.Description);
                yield return null;
            }
            
            UnblockInput();
        }
    }
}
