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
        public override string AditionalData
        {
            get { return base.AditionalData; }
            set
            {
                base.AditionalData = value;
                var parts = base.AditionalData.Split(',');
                LabelPrefabPath = parts[0];
                TexturePrefabPath = parts[1];
            }
        }

        public string LabelPrefabPath;
        public string TexturePrefabPath;

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

                var rootPanelSize = Alias.Term.RootPanel.GetViewSize();

                var windowWidth = (rootPanelSize.x / 3) *2;
                var windowHeight = rootPanelSize.y/2;

                var window = Window.Create();
                
                if (file.FileType == FileType.Image)
                {
                    var image = Object.Instantiate(Resources.Load<GameObject>(TexturePrefabPath)).GetComponent<UITexture>();
                    var texture = Resources.Load<Texture>(file.Content);
                    image.mainTexture = texture;
                    content = image;
                    image.width = texture.width;
                    image.height = texture.height;
                    windowWidth = texture.width;
                    windowHeight = texture.height;
                }
                else
                {
                    var label = Object.Instantiate(Resources.Load<GameObject>(LabelPrefabPath)).GetComponent<UILabel>();
                    label.SetupWithHash17Settings();
                    label.text = file.Content;
                    label.overflowMethod = UILabel.Overflow.ResizeHeight;
                    label.rightAnchor.target = window.transform;
                    label.leftAnchor.target = window.transform;
                    content = label;
                    windowWidth = label.width;
                }

                window.LosesFocus = true;

                window.Setup(file.Name,
                    content,
                    showMaximizeButton: false,
                    startClosed: true);
                window.Size = new Vector2(windowWidth, windowHeight);
            }
            else
            {
                Alias.Term.ShowText(TextBuilder.ErrorText("File not found. Use 'cd' or 'search' programs to find a file."));
                yield break;
            }
        }
    }
}