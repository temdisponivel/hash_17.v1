﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hash17.Files
{
    public class Directory
    {
        public virtual string Name { get; internal set; }
        public virtual Directory Parent { get; internal set; }
        internal List<Directory> Childs = new List<Directory>();
        internal List<File> Files = new List<File>();

        public string Path
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                Directory parent = this;
                while (parent != null)
                {
                    builder = builder.Insert(0, string.Format("{0}/", parent.Name.Remove('/')));
                    parent = parent.Parent;
                }
                builder.Remove(builder.Length - 1, 1);
                return builder.ToString();
            }
        }

        public Directory FindDirectoryByName(string name)
        {
            return Childs.Find(d => String.Equals(d.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public File FindFileByName(string name)
        {
            return Files.Find(d => String.Equals(d.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}