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

            bool any = Parameters.TryGetParam("any", out param);
            bool only = Parameters.TryGetParam("only", out param);

            if (!(any || only || Parameters.TryGetParam("", out param)))
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
            if (any)
            {
                files = Alias.Board.CurrentDevice.FileSystem.AllFiles;
            }
            else
            {
                files = Alias.Board.FileSystem.CurrentDirectory.Files;
            }

            Alias.Term.ShowText("Files found - (use 'open <file_path>' to open one of them):");

            Alias.Term.BeginIdentation();

            bool hasFiles = false;
            for (int i = 0; i < files.Count; i++)
            {
                var currentFile = files[i];
                
                var content = currentFile.Content.ToLower();

                var nameValidated = false;
                if (currentFile.FileType != FileType.Text || !Validate(terms, content, only))
                {
                    content = currentFile.Name;
                    if (!Validate(terms, content, only))
                        continue;

                    nameValidated = true;
                }

                hasFiles = true;

                content = currentFile.Content.SubString(15, 15, terms);
                string name = currentFile.Name;

                for (int j = 0; j < terms.Length; j++)
                {
                    if (nameValidated)
                        name = name.Replace(terms[j], string.Format("[b][i]{0}[/i][/b]", terms[j]));
                    else
                        content = content.Replace(terms[j], string.Format("[b][i]{0}[/i][/b]", terms[j]));
                }

                Alias.Term.ShowText(string.Format("Name: {0} | Path: {1}", name, files[i].Path));
                if (!nameValidated)
                {
                    Alias.Term.ShowText("Content:");
                    Alias.Term.ShowText(content, ident: true);
                }

                Alias.Term.ShowText("-----------------------------------");
            }

            if (!hasFiles)
            {
                Alias.Term.ShowText("None.");
            }

            Alias.Term.EndIdentation();

            Alias.Term.ShowText("Programs found:");

            Alias.Term.BeginIdentation();
            bool hasProgram = false;
            foreach (var program in Alias.Board.ProgramsByCommand)
            {
                var prog = program.Value;
                if (Validate(terms, prog.Command.ToLower(), only) || Validate(terms, prog.Description.ToLower(), only))
                {
                    hasProgram = true;

                    string name = prog.Command;
                    var content = prog.Description;

                    for (int j = 0; j < terms.Length; j++)
                    {
                        content = content.Replace(terms[j], string.Format("[b][i]{0}[/i][/b]", terms[j]));
                        name = name.Replace(terms[j], string.Format("[b][i]{0}[/i][/b]", terms[j]));
                    }

                    Alias.Term.ShowText(name);
                    Alias.Term.ShowText(content, ident: true);

                    Alias.Term.ShowText("-----------------------------------");
                }
            }

            if (!hasProgram)
            {
                Alias.Term.ShowText("None.");
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
