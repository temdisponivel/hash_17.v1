using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Hash17.Files
{
    [Serializable]
    public class File
    {
        public string Name { get; set; }
        public Directory Directory { get; set; }
        public string Content { get; set; }
        public string PathString { get; set; }

        [JsonIgnore]
        public string Path
        {
            get { return string.Format("{0}{1}{2}", Directory.Path, FileSystem.Instance.DirectorySeparator, Name); }
        }
    }
}
