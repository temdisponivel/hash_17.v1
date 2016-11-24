using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Files;
using Hash17.Terminal_;

namespace Hash17.Programs.Implementation
{
    public class Dir : Program
    {
        protected override IEnumerator InnerExecute()
        {
            yield return null;

            if (HelpOrUnknownParameters(true))
                yield break;

            var fileSystem = Blackboard.Instance.FileSystem;

            ProgramParameter.Param param;
            if (Parameters.TryGetParam("C", out param))
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
                fileSystem.FindDirectory(param.Value, true);
            }
        }
    }
}
