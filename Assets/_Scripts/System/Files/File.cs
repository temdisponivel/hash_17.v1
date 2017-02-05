using System;
using Hash17.FilesSystem.Files;
using Hash17.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Hash17.Files
{
    [Serializable]
    public class File
    {
        #region Properties

        public int UniqueId { get; set; }
        public string Name { get; set; }
        public FileType FileType { get; set; }
        public Directory Directory { get; set; }
        public bool IsProtected { get; set; }
        public string Password { get; set; }

        private string _content;
        public string Content
        {
            get
            {
                // To prevent JSON to serialize wrong data
                if (!Application.isPlaying)
                    return _content;

                if (!IsProtected)
                    return _content;
                
                if (Alias.Campaign.Info.CrackedFiles.Contains(UniqueId))
                    return _content;
                
                return _content.Encrypt(Password);
            }
            set { _content = value; }
        }

        public string PathString { get; set; }

        public bool CanBeRead
        {
            get { return Application.isPlaying && (!IsProtected || Alias.Campaign.Info.CrackedFiles.Contains(UniqueId)); }
        }

        [JsonIgnore]
        public virtual string PrettyName
        {
            get
            {
                Color color = Alias.Config.FileColor;
                if (!CanBeRead)
                    color = Alias.Config.LockedFileColor;
                else if (IsProtected)
                    color = Alias.Config.SecureFileColor;
                return TextBuilder.BuildText(Name, color);
            }
        }

        [JsonIgnore]
        public string Path
        {
            get { return string.Format("{0}{1}", Directory.Path, Name); }
        }

        #endregion

        #region Events

        public static event Action<File> OnFileOpened;

        #endregion

        #region Methods

        public void Open()
        {
            if (OnFileOpened != null)
                OnFileOpened(this);
        }

        #endregion
    }
}
