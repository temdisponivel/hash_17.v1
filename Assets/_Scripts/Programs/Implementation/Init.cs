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
using UnityEditor.VersionControl;
using UnityEngine;

namespace Hash17.Programs.Implementation
{
    public class Init : Program
    {
        private TextAsset _initTextAsset;
        private ResourceRequest _textAssetRequest;

        private bool _wasCanceled;

        protected override IEnumerator InnerExecute()
        {
            BlockInput();
            _textAssetRequest = Resources.LoadAsync<TextAsset>(AditionalData);
            
            Alias.Term.ShowText("Retrieving message from Vox Populi FTP Server...");
            Alias.Term.ShowText(string.Format("Connecting to {0}", Alias.Board.GameConfiguration.VoxPopuliServer));
            Alias.Term.ShowTypeWriterText(".....", .3f, callback: FinishShowingInitialText);
            yield return null;
        }

        private void FinishShowingInitialText()
        {
            Alias.Term.ShowText("Message received:");
            StartCoroutine(MessageCoroutine());
        }

        private IEnumerator MessageCoroutine()
        {
            yield return _textAssetRequest;
            _initTextAsset = _textAssetRequest.asset as TextAsset;
            Alias.Term.BeginIdentation();
            yield return Alias.Term.ShowTypeWriterTextWithCancel(_initTextAsset.text, startOnNewLine: true, callback: FinishShowingMessage);
            Alias.Term.EndIdentation();
        }

        private void FinishShowingMessage()
        {
            File file;
            Alias.Board.FileSystem.CreateFile("vox_populi_msg.txt", out file);
            file.Content = _initTextAsset.text;
            file.FileType = FileType.Text;
            Alias.Term.ShowText(TextBuilder.WarningText("This message was saved to file 'vox_populi_msg.txt' and can be opened using 'read' program."));
            UnblockInput();
            Alias.Term.RunProgram(Alias.Board.ProgramDefinitionById[ProgramId.Timer], "10");
        }
    }
}
