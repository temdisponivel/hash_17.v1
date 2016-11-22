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

            Terminal.Instance.ShowText("PARAMETERS: ");

            for (int i = 0; i < Parameters.Params.Count; i++)
            {
                var current = Parameters.Params[i];
                Terminal.Instance.ShowText(TextBuilder.WarningText(string.Format("NAME: {0} - VALUE: {1} - PREFIX: {2}", current.Name, current.Value, current.Prefix)));
                yield return null;
            }
            

            UnblockInput();
        }
    }
}
