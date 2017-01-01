using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Files;
using Hash17.FilesSystem.Files;
using Hash17.Terminal_;

namespace Hash17.Programs.Implementation
{
    public class Search : Program
    {
        protected override IEnumerator InnerExecute()
        {
            ProgramParameter.Param param;
            
            bool all = Parameters.TryGetParam("all", out param);
            bool only = Parameters.TryGetParam("only", out param);

            if (!(all || only || Parameters.TryGetParam("", out param)))
            {
                ShowHelp();
                yield break;
            }


            param = Parameters.GetFirstParamWithValue();

            if (param == null)
            {
                ShowHelp();
                yield break;
            }

            var terms = param.Value.Split(';');

            List<File> files;
            if (all)
            {
                files = Blackboard.Instance.AllFiles;
            }
            else
            {
                files = Blackboard.Instance.FileSystem.CurrentDirectory.Files;
            }

            Terminal.Instance.ShowText("Terms found in:");

            for (int i = 0; i < files.Count; i++)
            {
                var currentFile = files[i];

                if (currentFile.FileType != FileType.Text)
                    continue;

                var content = currentFile.Content.ToLower();

                if (only)
                {
                    if (!terms.All(t => content.Contains(t)))
                        continue;
                }
                else if (!terms.Any(t => content.Contains(t)))
                {
                    continue;
                }

                content = currentFile.Content;

                for (int j = 0; j < terms.Length; j++)
                {
                    content = content.Replace(terms[j], string.Format("[b][i]{0}[/i][/b]", terms[j]));
                }

                Terminal.Instance.ShowText(string.Format("{0} - {1}: {2}", files[i].Name, files[i].Directory.Path, content));
            }
        }
    }
}
