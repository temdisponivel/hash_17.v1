using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hash17.Files.SO
{
    [CreateAssetMenu(fileName = "New File System", menuName = "Hash17/Files/Create file system")]
    public class FileSystemScriptableObject : DirectoryScriptableObject
    {
        public FileSystem ToFileSystem()
        {
            var fileSystem = new FileSystem();
            var thisAsDir = ToDirectory();
            fileSystem.Childs = thisAsDir.Childs;
            fileSystem.Files = thisAsDir.Files;
            return fileSystem;
        }
    }
}
