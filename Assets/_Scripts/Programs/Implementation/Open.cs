using System.Collections;
using Hash17.Files;
using Hash17.FilesSystem.Files;
using Hash17.MockSystem;
using MockSystem;
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

        private Window _windowOpened;
        private Object _resourceLoaded;

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
            if (DeviceCollection.FileSystem.FindFileByPath(fileName, out file) == FileSystem.OperationResult.Ok)
            {
                UIWidget content;

                var window = Window.Create();
                
                if (file.CanBeRead && file.FileType == FileType.Image)
                {
                    var image = Object.Instantiate(Resources.Load<GameObject>(TexturePrefabPath)).GetComponent<UITexture>();
                    var texture = Resources.Load<Texture>(file.Content);
                    _resourceLoaded = texture;
                    image.mainTexture = texture;
                    content = image;
                    image.width = texture.width;
                    image.height = texture.height;

                    content.rightAnchor.target = window.ContentPanel.transform;
                    content.leftAnchor.target = window.ContentPanel.transform;
                    content.topAnchor.target = window.ContentPanel.transform;
                    content.bottomAnchor.target = window.ContentPanel.transform;
                }
                else
                {
                    var label = Object.Instantiate(Resources.Load<GameObject>(LabelPrefabPath)).GetComponent<UILabel>();
                    label.SetupWithHash17Settings();
                    label.text = file.Content;
                    label.overflowMethod = UILabel.Overflow.ResizeHeight;
                    label.rightAnchor.target = window.ContentPanel.transform;
                    label.leftAnchor.target = window.ContentPanel.transform;
                    content = label;
                }

                window.LosesFocus = true;

                window.Setup(file.Name,
                    content,
                    showMaximizeButton: false,
                    startClosed: true);

                window.OnClose += OnWindowClose;
                _windowOpened = window;

                file.Open();
            }
            else
            {
                Alias.Term.ShowText(TextBuilder.ErrorText("File not found. Use 'cd' or 'search' programs to find a file."));
                yield break;
            }
        }

        private void OnWindowClose()
        {
            _windowOpened.OnClose -= OnWindowClose;

            if (_resourceLoaded != null)
                Resources.UnloadAsset(_resourceLoaded);
        }
    }
}