using System;
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

            var fileSystem = Blackboard.Instance.FileSystem;

            ProgramParameter.Param param;
            if (Parameters.TryGetParam("c", out param))
            {
                var path = param.Value;

                Directory dir;
                var result = fileSystem.CreateDiretory(path, out dir);

                if (result == FileSystem.OperationResult.DuplicatedName)
                {
                    Terminal.Instance.ShowText("There already is a directory with this name");
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
                        Terminal.Showtext(TextBuilder.WarningText("This is a file. Use \"read\" command to read it."));
                    }
                    else
                    {
                        Terminal.Showtext(TextBuilder.WarningText("Directory not found."));
                    }
                }
            }
            else if (param == null)
            {
                var dir = fileSystem.CurrentDirectory;
                for (int i = 0; i < dir.Childs.Count; i++)
                {
                    Terminal.Instance.ShowText(TextBuilder.BuildText(dir.Childs[i].Name, Color.blue));
                }

                for (int i = 0; i < dir.Files.Count; i++)
                {
                    Terminal.Instance.ShowText(TextBuilder.BuildText(dir.Files[i].Name, Color.yellow));
                }
            }
        }
    }
}
