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
        #region Properties

        public int UniqueId { get; set; }
        public virtual string Name { get; set; }
        public virtual Directory Parent { get; set; }
        private List<Directory> _childs;
        private List<File> _files;

        public bool IsAvailable
        {
            get { return Alias.Campaign.Info.UnlockedDirectories.Contains(UniqueId); }
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
            get { return TextBuilder.BuildText(Name, Alias.Config.DirectoryColor); }
        }

        #endregion

        #region Contructor

        public Directory()
        {
            _childs = new List<Directory>();
            _files = new List<File>();
        }

        #endregion

        #region Childs and files

        public virtual File FindFileByName(string name)
        {
            return _files.Find(d => String.Equals(d.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public List<File> GetFilesInDirectoriesAndChilds(List<File> toAdd)
        {
            toAdd.AddRange(GetAvailableFiles());
            var childs = GetAvailableChilds();
            for (int i = 0; i < childs.Count; i++)
            {
                toAdd = childs[i].GetFilesInDirectoriesAndChilds(toAdd);
            }
            return toAdd;
        }

        #region Childs

        public virtual Directory FindDirectoryByName(string name)
        {
            return _childs.Find(d => String.Equals(d.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public List<Directory> GetAllChilds()
        {
            return _childs;
        }

        public List<Directory> GetAvailableChilds()
        {
            return _childs.FindAll(c => c.IsAvailable);
        }

        #endregion

        #region Files

        public List<File> GetAllFiles()
        {
            return _files;
        }

        public List<File> GetAvailableFiles()
        {
            return _files.FindAll(f => f.IsAvailable);
        }

        #endregion

        #endregion
    }
}