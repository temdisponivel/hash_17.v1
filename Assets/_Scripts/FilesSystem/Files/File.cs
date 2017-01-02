using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Blackboard_;
using Hash17.FilesSystem.Files;
using Hash17.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Hash17.Files
{
    [Serializable]
    public class File
    {
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
                
                if (Alias.Board.UnlockedFiles.Contains(UniqueId))
                    return _content;

                return _content.Encrypt(Password);
            }
            set { _content = value; }
        }

        public string PathString { get; set; }

        public bool CanBeRead
        {
            get { return !IsProtected || Alias.Board.UnlockedFiles.Contains(UniqueId); }
        }

        [JsonIgnore]
        public virtual string PrettyName
        {
            get
            {
                Color color = Alias.GameConfig.FileColor;
                if (!CanBeRead)
                    color = Alias.GameConfig.LockedFileColor;
                else if (IsProtected)
                    color = Alias.GameConfig.SecureFileColor;
                return TextBuilder.BuildText(Name, color);
            }
        }

        [JsonIgnore]
        public string Path
        {
            get { return string.Format("{0}{1}{2}", Directory.Path, FileSystem.Instance.DirectorySeparator, Name); }
        }
    }
}
