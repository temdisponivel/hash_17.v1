using System.Collections;
using Hash17.Files;
using Hash17.FilesSystem.Files;
using Hash17.Terminal_;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Programs.Implementation
{
    public class Open : Program
    {
        protected override IEnumerator InnerExecute()
        {
            if (HelpOrUnknownParameters(true))
                yield break;

            var param = Parameters.GetFirstParamWithValue();

            if (param == null)
            {
                Alias.Term.ShowText(TextBuilder.ErrorText("You must pass a file path as parameter."));
                yield break;
            }

            var fileName = param.Value;

            File file;
            if (Alias.Board.FileSystem.FindFileByPath(fileName, out file) == FileSystem.OperationResult.Ok)
            {
                UIWidget content;

                if (file.FileType == FileType.Image)
                {
                    var image = new GameObject().AddComponent<UITexture>();
                    image.mainTexture = Resources.Load<Texture>(file.Content);
                    content = image;
                }
                else
                {
                    var label = new GameObject().AddComponent<UILabel>();
                    label.SetupWithHash17Settings();
                    label.text = file.Content;
                    content = label;
                }

                var rootPanelSize = Alias.Term.RootPanel.GetViewSize();
                var window = Window.Create();
                window.Setup(file.Name,
                    content,
                    Window.ContentFitType.Strech,
                    showMaximizeButton: false,
                    startClosed: true);
                window.Size = new Vector2((rootPanelSize.x / 3) * 2, rootPanelSize.y / 2);
            }
            else
            {
                Alias.Term.ShowText(TextBuilder.ErrorText("File not found. Use 'cd' or 'search' programs to find a file."));
                yield break;
            }
        }
    }
}