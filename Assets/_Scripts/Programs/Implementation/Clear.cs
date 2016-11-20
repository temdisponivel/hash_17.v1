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
        protected override IEnumerator InnerExecute(string parameters)
        {
            yield return null;

            string value = string.Empty;
            bool clearQuant = false;
            int quant = 0;
            if (Interpreter.ContainsParameter(parameters, false, "C", out value))
            {
                clearQuant = int.TryParse(value, out quant) && quant > 0;
            }

            yield return 0;

            if (clearQuant)
            {
                Terminal.Instance.Clear(quant);
            }
            else
            {
                Terminal.Instance.ClearAll();
            }
        }
    }
}