using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Terminal_;

namespace Hash17.Programs.Implementation
{
    public class Help : Program
    {
        protected override IEnumerator InnerExecute()
        {
            BlockInput();

            Terminal.Instance.ShowText("-----------");

            foreach (var program in Blackboard.Instance.Programs)
            {
                Terminal.Instance.ShowText(program.Value.GetDescription());
                Terminal.Instance.ShowText(program.Value.GetUsage());
                Terminal.Instance.ShowText("\n");
                yield return null;
            }

            UnblockInput();
        }
    }
}
