﻿using System;
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
                Blackboard.Instance.FileSystem.FindFileByPath(filePath, out file);

                if (file != null)
                {
                    if (file.FileType != FileType.Text)
                    {
                        Terminal.Showtext(TextBuilder.WarningText("This is not a text file."));
                    }
                    else
                    {
                        if (!file.CanBeRead)
                        {
                            string message = "This file is protected. Use the program 'cypher' to decrypt it \n The following are the encrypted file:";
                            Terminal.Showtext(TextBuilder.WarningText(message));
                        }

                        Terminal.Instance.ShowText(TextBuilder.WarningText(string.Format("{0}:", file.Name)));
                        Terminal.Instance.ShowText(file.Content);
                    }
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
