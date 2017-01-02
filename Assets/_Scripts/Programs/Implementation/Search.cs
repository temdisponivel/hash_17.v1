using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hash17.Files;
using Hash17.FilesSystem.Files;
using Hash17.Utils;

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

            param.Value = param.Value.ToLower();

            var terms = param.Value.Split(';');
            terms = terms.Distinct().ToArray();

            List<File> files;
            if (all)
            {
                files = Alias.Board.AllFiles;
            }
            else
            {
                files = Alias.Board.FileSystem.CurrentDirectory.Files;
            }

            Alias.Term.ShowText("Files found:");

            Alias.Term.BeginIdentation();

            for (int i = 0; i < files.Count; i++)
            {
                var currentFile = files[i];

                if (currentFile.FileType != FileType.Text)
                    continue;

                var content = currentFile.Content.ToLower();

                var nameValidated = false;
                if (!Validate(terms, content, only))
                {
                    content = currentFile.Name;
                    if (!Validate(terms, content, only))
                        continue;

                    nameValidated = true;
                }

                content = currentFile.Content.SubString(10, 10, terms);
                string name = currentFile.Name;

                for (int j = 0; j < terms.Length; j++)
                {
                    if (nameValidated)
                        name = name.Replace(terms[j], string.Format("[b][i]{0}[/i][/b]", terms[j]));
                    else
                        content = content.Replace(terms[j], string.Format("[b][i]{0}[/i][/b]", terms[j]));
                }

                Alias.Term.ShowText(string.Format("Name: {0} | Path: {1}", name, files[i].Directory.Path));
                if (!nameValidated)
                {
                    Alias.Term.ShowText("Content:");
                    Alias.Term.ShowText(content, ident: true);
                }
            }

            Alias.Term.EndIdentation();

            Alias.Term.ShowText("Programs found:");

            Alias.Term.BeginIdentation();
            foreach (var program in Alias.Board.Programs)
            {
                var prog = program.Value;
                if (Validate(terms, prog.Command, only) || Validate(terms, prog.Description, only))
                {
                    string name = prog.Command;
                    var content = prog.Description;

                    for (int j = 0; j < terms.Length; j++)
                    {
                        content = content.Replace(terms[j], string.Format("[b][i]{0}[/i][/b]", terms[j]));
                        name = name.Replace(terms[j], string.Format("[b][i]{0}[/i][/b]", terms[j]));
                    }

                    Alias.Term.ShowText(name);
                    Alias.Term.ShowText(content, ident: true);
                }
            }

            Alias.Term.EndIdentation();
        }

        private bool Validate(string[] terms, string toValidate, bool only)
        {
            if (only)
            {
                if (!terms.All(t => toValidate.Contains(t)))
                    return false;
            }
            else if (!terms.Any(t => toValidate.Contains(t)))
            {
                return false;
            }

            return true;
        }
    }
}
