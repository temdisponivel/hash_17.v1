using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Files;
using Hash17.FilesSystem.Files;
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
                var parts = param.Value.Split(' ');

                var filePath = param.Value;
                File file;
                Alias.Board.FileSystem.FindFileByPath(filePath, out file);

                if (file != null)
                {
                    if (file.FileType != FileType.Text)
                    {
                        Alias.Term.ShowText(TextBuilder.WarningText("This is not a text file."));
                    }
                    else
                    {
                        if (!file.CanBeRead)
                        {
                            Alias.Term.ShowText(TextBuilder.WarningText("This file is protected. Use the program 'cypher' to decrypt it."));
                        }

                        Alias.Term.BeginIdentation();
                        Alias.Term.ShowText(file.Content, ident: true);
                        Alias.Term.EndIdentation();
                    }
                }
                else
                {
                    Alias.Term.ShowText(TextBuilder.ErrorText("File not found"));
                }
            }

            yield return null;
        }
    }
}
