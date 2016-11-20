using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Terminal_;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Programs.Implementation
{
    public class Clear : Program
    {
        protected override IEnumerator InnerExecute()
        {
            if (AskedForHelp(true))
            {
                yield break;
            }

            if (ValidateUnknowParameters(true))
            {
                yield break;
            }

            ProgramParameter.Param param;
            if (Parameters.TryGetParam("C", out param))
            {
                int quant = 0;
                if (!param.IsOption && int.TryParse(param.Value, out quant) && quant > 0)
                {
                    Terminal.Instance.Clear(quant);
                }
                else
                {
                    Terminal.Instance.ShowText(TextBuilder.WarningText(string.Format("Invalid value for parameter -C.\n{0}", GetUsage())));
                }
            }
            else
            {
                Terminal.Instance.ClearAll();
            }

            yield break;
        }
    }
}