using System.Collections;
using System.Text;
using DG.Tweening;
using Hash17.Blackboard_;
using Hash17.Files;
using Hash17.Terminal_;
using Hash17.Utils;

namespace Hash17.Programs.Implementation
{
    public class Cypher : Program
    {
        protected override IEnumerator InnerExecute()
        {
            if (HelpOrUnknownParameters(true))
                yield break;

            BlockInput();

            ProgramParameter.Param param;
            if (Parameters.TryGetParam("", out param))
            {
                var parts = param.Value.Split(' ');

                if (parts.Length < 2)
                {
                    Terminal.Showtext(TextBuilder.WarningText("Invalid number of parameters."));
                    ShowHelp();
                    UnblockInput();
                    yield break;
                }

                var filePath = parts[0];
                var passWord = parts[1];

                File file;
                Blackboard.Instance.FileSystem.FindFileByPath(filePath, out file);
                if (file == null)
                {
                    Terminal.Showtext(TextBuilder.WarningText(string.Format("File {0} not found.", filePath)));
                    UnblockInput();
                    yield break;
                }

                if (file.IsProtected)
                {
                    if (file.Password != passWord)
                    {
                        UnblockInput();
                        Terminal.Showtext("Invalid password...");
                        yield break;
                    }

                    Terminal.Showtext("Decrypting file...");
                }
                else
                {
                    Terminal.Showtext("Encrypting file...");
                }

                var textToShow = new StringBuilder();
                var length = file.Content.Length;
                for (int i = 0; i < length; i++)
                {
                    textToShow.Append(".");
                }

                Terminal.Instance.ShowTextWithInterval(textToShow.ToString(), .5f/30f, () => OnFinishDecryptingCallback(file, passWord));
            }
        }

        protected void OnFinishDecryptingCallback(File file, string passWord)
        {
            if (!Blackboard.Instance.UnlockedFiles.Contains(file.UniqueId))
                Blackboard.Instance.UnlockedFiles.Add(file.UniqueId);

            if (file.IsProtected)
            {
                Terminal.Showtext("File decrypted. You can read this file now.");
            }
            else
            {
                file.IsProtected = true;
                file.Password = passWord;
                Terminal.Showtext("File encrypted. You can read the file, but other people can't (unless they have the password).");
            }

            UnblockInput();
        }
    }
}