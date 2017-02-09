using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using FH.Util.Extensions;
using Hash17.MockSystem;
using Hash17.Utils;
using Debug = UnityEngine.Debug;

namespace Hash17.Programs.Implementation
{
    public class Set : Program
    {
        protected override IEnumerator InnerExecute()
        {
            yield return 0;

            if (Parameters.Params.Count < 1)
            {
                ShowHelp();
                yield break;
            }

            var parts = Parameters.Params[0].Value.Split(' ');

            if (parts.Length < 2)
            {
                if (parts.Length < 1)
                {
                    ShowHelp();
                }
                else
                {
                    Alias.Term.ShowText("You need to supply a value for the variable '{0}'".InLineFormat(parts[0]));
                }

                yield break;
            }

            var variable = parts[0];
            var value = parts[1];

            if (string.IsNullOrEmpty(value))
            {
                Alias.Term.ShowText("You need to supply a value for the variable {0}".InLineFormat(Parameters.Params[0]));
            }

            Alias.SysVariables[value] = value;
        }
    }
}