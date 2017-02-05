using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hash17.Files;
using Hash17.FilesSystem.Files;
using Hash17.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Hash17.Programs.Implementation
{
    public class Search : Program
    {
        protected override IEnumerator InnerExecute()
        {
            ProgramParameter.Param param;

            bool any = Parameters.TryGetParam("any", out param);
            bool only = Parameters.TryGetParam("only", out param);

            if (!(any | only | (any = Parameters.TryGetParam("", out param))))
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
            var filesToShow = new List<File>();
            var programsToShow = new List<Program>();

            bool hasFiles = false;
            for (int i = 0; i < files.Count; i++)
            {
                var currentFile = files[i];
                var content = currentFile.Content.ToLower();

                if (currentFile.FileType != FileType.Text || !Validate(terms, content, only))
                {
                    content = currentFile.Name.ToLower();
                    if (!Validate(terms, content, only))
                        continue;
                }

                filesToShow.Add(currentFile);
            }
            
            foreach (var program in Alias.Board.ProgramsByCommand)
            {
                var prog = program.Value;
                if (Validate(terms, prog.Command.ToLower(), only) || Validate(terms, prog.Description.ToLower(), only))
                {
                    programsToShow.Add(prog);
                }
            }

            Alias.Term.ShowText(TextBuilder.MessageText("Files that constains (in name or content) the terms: {0}".InLineFormat(param.Value)));
            Alias.Term.ShowText(string.Empty);

            {
                const int namePad = 30;
                const int total = namePad + namePad;

                Alias.Term.ShowText("{0}{1}".InLineFormat("NAME".PadRight(namePad), "PATH"));
                Alias.Term.ShowText("".PadRight(total, '-'));

                for (int i = 0; i < filesToShow.Count; i++)
                {
                    var unusedCharsCount = 0;
                    var finalName = filesToShow[i].PrettyName.HighlightTerms(terms);
                    var originalName = filesToShow[i].Name;
                    unusedCharsCount = finalName.Length - originalName.Length;
                    var name = finalName.PadRight(namePad + unusedCharsCount);
                    var path = filesToShow[i].Path.HighlightTerms(terms);
                    Alias.Term.ShowText("{0}{1}".InLineFormat(name, path));
                }
            }
            
            Alias.Term.ShowText(string.Empty);
            Alias.Term.ShowText(string.Empty);

            Alias.Term.ShowText(TextBuilder.MessageText("Programs that constains (in name or description) the terms: {0}".InLineFormat(param.Value)));
            Alias.Term.ShowText(string.Empty);

            {
                const int commandPad = 30;
                const int total = commandPad + commandPad;

                Alias.Term.ShowText("{0}{1}".InLineFormat("COMMAND".PadRight(commandPad), "DESCRIPTION"));
                Alias.Term.ShowText("".PadRight(total, '-'));

                for (int i = 0; i < programsToShow.Count; i++)
                {
                    var unusedCharsCount = 0;
                    var finalName = programsToShow[i].PrettyCommand.HighlightTerms(terms);
                    var originalName = programsToShow[i].Command;
                    unusedCharsCount = finalName.Length - originalName.Length;

                    var name = finalName.PadRight(commandPad + unusedCharsCount);
                    var desc = programsToShow[i].Description.HighlightTerms(terms);
                    Alias.Term.ShowText("{0}{1}".InLineFormat(name, desc));
                }
            }

            yield return 0;

            Alias.Term.RepositionText();
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
