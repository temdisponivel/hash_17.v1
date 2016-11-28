using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Hash17.Files.SO
{
    [CreateAssetMenu(fileName = "New File", menuName = "Hash17/Files/Create directory")]
    public class DirectoryScriptableObject : ScriptableObject
    {
        public string Name;
        public List<DirectoryScriptableObject> Directories;
        public List<FileScriptableObject> Files;

        public Directory ToDirectory()
        {
            var dir = new Directory();
            dir.Name = Name;
            dir.Childs = Directories.Select(d => d.ToDirectory()).ToList();
            dir.Files = Files.Select(f => f.ToFile()).ToList();

            for (int i = 0; i < dir.Childs.Count; i++)
            {
                dir.Childs[i].Parent = dir;
            }

            for (int i = 0; i < dir.Files.Count; i++)
            {
                dir.Files[i].Directory = dir;
            }

            return dir;
        }
    }
}
