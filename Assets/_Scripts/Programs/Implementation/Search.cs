using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Files;
using Hash17.Terminal_;

namespace Hash17.Programs.Implementation
{
    public class Search : Program
    {
        protected override IEnumerator InnerExecute()
        {
            ProgramParameter.Param param;
            bool all = false;
            if (!(Parameters.TryGetParam("", out param) || (all = Parameters.TryGetParam("all", out param))) || string.IsNullOrEmpty(param.Value))
            {
                ShowHelp();
                yield break;
            }

            var terms = param.Value.Split(';');

            List<File> files;
            if (all)
            {
                files = new List<File>(FileSystem.Instance.Files);
                Directory root = FileSystem.Instance;
                var toSee = new List<Directory>();
                toSee.Add(root);
                for (int j = 0; j < toSee.Count; j++)
                {
                    root = toSee[j];
                    files.AddRange(root.Files);
                    for (int i = 0; i < root.Childs.Count; i++)
                    {
                        toSee.Add(root.Childs[i]);
                    }
                }
            }
            else
            {
                files = Blackboard.Instance.FileSystem.CurrentDirectory.Files;
            }

            Terminal.Instance.ShowText("Terms found in:");

            for (int i = 0; i < files.Count; i++)
            {
                var currentFile = files[i];

                var content = currentFile.Content.ToLower();

                if (!terms.Any(t => content.Contains(t)))
                    continue;

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
