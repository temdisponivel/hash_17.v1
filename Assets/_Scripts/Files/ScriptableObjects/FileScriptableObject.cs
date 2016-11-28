using System.Collections.Generic;
using UnityEngine;

namespace Hash17.Files.SO
{
    [CreateAssetMenu(fileName = "New File", menuName = "Hash17/Files/Create file")]
    public class FileScriptableObject : ScriptableObject
    {
        public string Name;
        public string Content;

        public File ToFile()
        {
            var file = new File();
            file.Name = Name;
            file.Content = Content;
            return file;
        }
    }
}
