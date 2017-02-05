using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Files;
using Hash17.FilesSystem.Files;
using Hash17.Terminal_;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Programs.Implementation
{
    public class Init : Program
    {
        protected override IEnumerator InnerExecute()
        {
            ManuallyFinished = true;

            BlockInput();

            Alias.Term.ShowUserLocationLabel = false;

            var textAssetRequest = Resources.LoadAsync<TextAsset>(AditionalData);

            yield return textAssetRequest;

            var initTextAsset = textAssetRequest.asset as TextAsset;

            var text = initTextAsset.text.HandleSystemVariables();
            var textAndTime = text.GetStringAndTime();

            Alias.Term.ShowTimedText(textAndTime, callback: FinishShowingMessage);
        }

        private void FinishShowingMessage()
        {
            Alias.Term.ShowUserLocationLabel = true;
            UnblockInput();

            if (!Alias.Campaign.HasSetUserName)
            {
                var message = TextBuilder.WarningText(
                            "Please, set your {0} using the '{1}' program."
                            .InLineFormat("username".Colorize(Alias.GameConfig.UserNameColor), "set".Colorize(Alias.GameConfig.ProgramColor)));

                Alias.Term.ShowText(message);

            }

            var helpMessage = "If you need any help, just type '{0}'.".InLineFormat("help".Colorize(Alias.GameConfig.ProgramColor));

            Alias.Term.ShowText(helpMessage);

            FinishExecution();
        }
    }
}
