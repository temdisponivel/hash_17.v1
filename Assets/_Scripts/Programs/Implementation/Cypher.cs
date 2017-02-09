using System.Collections;
using System.Text;
using DG.Tweening;
using Hash17.Data;
using Hash17.Devices;
using Hash17.Files;
using Hash17.MockSystem;
using MockSystem;
using Hash17.Utils;
using UnityEngine;

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
                    Alias.Term.ShowText(TextBuilder.WarningText("You should supply a password for encryption/decryption."));
                    ShowHelp();
                    UnblockInput();
                    yield break;
                }

                var filePath = parts[0];
                var passWord = parts[1];

                File file;
                DeviceCollection.FileSystem.FindFileByPath(filePath, out file);
                if (file == null)
                {
                    Alias.Term.ShowText(TextBuilder.WarningText(string.Format("File '{0}' not found.", filePath)));
                    UnblockInput();
                    yield break;
                }

                if (file.IsProtected)
                {
                    if (file.Password != passWord)
                    {
                        UnblockInput();
                        Alias.Term.ShowText("Invalid password.");
                        yield break;
                    }
                }

                var textToShow = new StringBuilder();
                var length = Mathf.Max(file.Content.Length, 50);
                for (int i = 0; i < length; i++)
                {
                    textToShow.Append(".");
                }

                Alias.Term.ShowTypeWriterText(textToShow.ToString(), callback: () => OnFinishDecryptingCallback(file, passWord));
            }
        }

        protected void OnFinishDecryptingCallback(File file, string passWord)
        {
            file.Decrypt();

            if (file.IsProtected)
                Alias.Term.ShowText("File decrypted. You can open this file now.");
            else
                Alias.Term.ShowText("File encrypted. You can open the file, but other people can't (unless they have the password).");

            //file.IsProtected = !file.IsProtected;
            file.Password = passWord;

            UnblockInput();
        }
    }
}