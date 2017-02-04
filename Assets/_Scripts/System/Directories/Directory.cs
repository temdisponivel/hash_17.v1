using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Hash17.Files
{
    [Serializable]
    public class Directory
    {
        
        public virtual string Name { get; set; }
        public virtual Directory Parent { get; set; }

        [SerializeField]
        private List<Directory> _childs;

        [SerializeField]
        private List<File> _files;

        public List<Directory> Childs
        {
            get { return _childs; }
            set { _childs = value; }
        }

        public List<File> Files
        {
            get { return _files; }
            set { _files = value; }
        }

        [JsonIgnore]
        public string Path
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                Directory parent = this;
                while (parent != null)
                {
                    builder = builder.Insert(0, string.Format("{0}/", parent.Name.Replace("/", "")));
                    parent = parent.Parent;
                }
                //if (builder.Length > 1)
                //    builder.Remove(builder.Length - 1, 1);
                return builder.ToString();
            }
        }

        [JsonIgnore]
        public virtual string PrettyName
        {
            get { return TextBuilder.BuildText(Name, Alias.GameConfig.DirectoryColor); }
        }

        public Directory()
        {
            _childs = new List<Directory>();
            _files = new List<File>();
        }

        public virtual Directory FindDirectoryByName(string name)
        {
            return Childs.Find(d => String.Equals(d.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public virtual File FindFileByName(string name)
        {
            return Files.Find(d => String.Equals(d.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public List<File> GetFilesInDirectoriesAndChilds(List<File> toAdd)
        {
            toAdd.AddRange(Files);
            for (int i = 0; i < Childs.Count; i++)
            {
                toAdd = Childs[i].GetFilesInDirectoriesAndChilds(toAdd);
            }
            return toAdd;
        }
    }
}
