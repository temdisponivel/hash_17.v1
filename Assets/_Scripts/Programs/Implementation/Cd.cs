﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Files;
using Hash17.Terminal_;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Programs.Implementation
{
    public class Cd : Program
    {
        protected override IEnumerator InnerExecute()
        {
            yield return null;

            if (HelpOrUnknownParameters(true))
                yield break;

            var fileSystem = Alias.Board.FileSystem;

            ProgramParameter.Param param;
            if (Parameters.TryGetParam("c", out param))
            {
                var path = param.Value;

                Directory dir;
                var result = fileSystem.CreateDiretory(path, out dir);

                if (result == FileSystem.OperationResult.DuplicatedName)
                {
                    Alias.Term.ShowText("There already is a directory with this name");
                }
            }
            else if (Parameters.TryGetParam("", out param))
            {
                var dir = fileSystem.FindDirectory(param.Value, true);
                if (dir == null)
                {
                    File file;
                    fileSystem.FindFileByPath(param.Value, out file);
                    if (file != null)
                    {
                        Alias.Term.ShowText(TextBuilder.WarningText("This is a file. Use \"read\" command to read it."));
                    }
                    else
                    {
                        Alias.Term.ShowText(TextBuilder.WarningText("Directory not found."));
                    }
                }
            }
            else if (param == null)
            {
                const int typePad = 6;
                const int namePad = 50;
                const int statusPad = 10;
                const int colorTagSize = 11;
                const int total = typePad + namePad + statusPad - colorTagSize;
                Alias.Term.ShowText("{0}{1}{2}".InLineFormat("TYPE".PadRight(typePad), "NAME".PadRight(namePad - colorTagSize), "STATUS".PadRight(statusPad)));
                Alias.Term.ShowText("".PadRight(total, '-'));

                var dir = fileSystem.CurrentDirectory;
                for (int i = 0; i < dir.Childs.Count; i++)
                {
                    Alias.Term.ShowText("{0}{1}".InLineFormat("DIR:".PadRight(typePad), dir.Childs[i].PrettyName));
                }

                for (int i = 0; i < dir.Files.Count; i++)
                {
                    var fileState = (dir.Files[i].IsProtected ? "Encrypted".PadRight(namePad) : "Normal".PadRight(statusPad));
                    Alias.Term.ShowText("{0}{1}{2}".InLineFormat("FILE:".PadRight(typePad), dir.Files[i].PrettyName.PadRight(namePad), fileState));
                }

                yield return 0;

                Alias.Term.RepositionText();
            }
        }
    }
}