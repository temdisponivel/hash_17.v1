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
        public bool StartUnlocked { get; set; }

        [JsonIgnore]
        public bool IsAvailable 
        {
            get { return Application.isPlaying && Alias.Campaign.Info.UnlockedFiles.Contains(UniqueId); }
        }

        [JsonProperty("_c")]
        private string _content;

        [JsonIgnore]
        public string Content
        {
            get
            {
                if (Alias.Campaign.Info.DecryptedFiles.Contains(UniqueId))
                    return _content;
                
                return _content.Encrypt(Password);
            }
            set { _content = value; }
        }

        public string PathString { get; set; }

        [JsonIgnore]
        public bool CanBeRead
        {
            get { return IsAvailable && (!IsProtected || Alias.Campaign.Info.DecryptedFiles.Contains(UniqueId)); }
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
        public static event Action<File> OnFileDecrypted;

        #endregion

        #region Methods

        public void Open()
        {
            if (OnFileOpened != null)
                OnFileOpened(this);
        }

        public void Decrypt()
        {
            if (OnFileDecrypted != null)
                OnFileDecrypted(this);
        }

        #endregion
    }
}
