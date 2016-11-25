using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Files;
using Hash17.Terminal_;
using Hash17.Utils;

namespace Hash17.Programs.Implementation
{
    public class Read : Program
    {
        protected override IEnumerator InnerExecute()
        {
            if (HelpOrUnknownParameters(true))
                yield break;

            ProgramParameter.Param param;
            if (Parameters.TryGetParam("", out param))
            {
                var filePath = param.Value;
                File file;
                Blackboard.Instance.FileSystem.FindFileByPath(filePath, out file);

                if (file != null)
                {
                    Terminal.Instance.ShowText(TextBuilder.WarningText(string.Format("{0}:", file.Name)));
                    Terminal.Instance.ShowText(file.Content);
                }
                else
                {
                    Terminal.Instance.ShowText(TextBuilder.ErrorText(string.Format("File not found")));
                }
            }

            yield return null;
        }
    }
}
