using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Terminal_;

namespace Hash17.Programs.Implementation
{
    public class Search : Program
    {
        protected override IEnumerator InnerExecute()
        {
            ProgramParameter.Param param;
            if (Parameters.TryGetParam("", out param))
            {
                if (string.IsNullOrEmpty(param.Value))
                {
                    ShowHelp();
                    yield break;
                }

                var terms = param.Value.Split(';');
                var files = Blackboard.Instance.FileSystem.CurrentDirectory.Files;

                Terminal.Instance.ShowText("Terms found in:");

                for (int i = 0; i < files.Count; i++)
                {
                    for (int j = 0; j < terms.Length; j++)
                    {
                        if (!files[i].Content.Contains(terms[j]))
                        {
                            continue;
                        }

                        Terminal.Instance.ShowText(string.Format("{0} - {1}: {2}", files[i].Name, files[i].Directory.Path, files[i].Content));
                    }
                }
            }
        }
    }
}
