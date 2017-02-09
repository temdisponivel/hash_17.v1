using System.Collections;
using FH.Util.Extensions;
using Hash17.Files;
using Hash17.FilesSystem.Files;
using Hash17.MockSystem;
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
                DeviceCollection.FileSystem.FindFileByPath(filePath, out file);

                if (file != null)
                {
                    if (file.FileType != FileType.Text)
                    {
                        Alias.Term.ShowText(TextBuilder.WarningText("{0} is not a text file.".InLineFormat(file.Name)));
                    }
                    else
                    {
                        Alias.Term.BeginIdentation();
                        Alias.Term.ShowText(file.Content, ident: true);
                        if (file.CanBeRead)
                            file.Open();
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
