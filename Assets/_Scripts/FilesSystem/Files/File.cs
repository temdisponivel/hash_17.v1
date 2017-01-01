using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Blackboard_;
using Hash17.FilesSystem.Files;
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
                
                if (Blackboard.Instance.UnlockedFiles.Contains(UniqueId))
                    return _content;

                return Convert.ToBase64String(Encoding.ASCII.GetBytes(_content));
            }
            set { _content = value; }
        }

        public string PathString { get; set; }

        public bool CanBeRead
        {
            get { return !IsProtected || Blackboard.Instance.UnlockedFiles.Contains(UniqueId); }
        }

        [JsonIgnore]
        public string Path
        {
            get { return string.Format("{0}{1}{2}", Directory.Path, FileSystem.Instance.DirectorySeparator, Name); }
        }
    }
}
