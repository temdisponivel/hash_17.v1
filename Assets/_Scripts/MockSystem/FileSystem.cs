using System;
using System.Collections.Generic;
using Hash17.Data;
using Hash17.Files;
using Hash17.Utils;
using Newtonsoft.Json;

namespace Hash17.MockSystem
{
    [Serializable]
    public class FileSystem : Directory
    {
        #region Inner types

        public enum OperationResult
        {
            Ok,
            DuplicatedName,
            InvalidValue,
            NotFound,
        }

        #endregion

        #region Properties

        public static event Action OnChangeCurrentDirectory;

        [JsonIgnore]
        private Directory _currentDirectory;

        [JsonIgnore]
        public Directory CurrentDirectory
        {
            get
            {
                if (_currentDirectory == null)
                    return this;
                return _currentDirectory;
            }
            private set
            {
                _currentDirectory = value;

                if (OnChangeCurrentDirectory != null)
                    OnChangeCurrentDirectory();
            }
        }

        [JsonIgnore]
        public char DirectorySeparator
        {
            get { return '/'; }
        }
        
        public override string Name
        {
            get { return "/"; }
            set { }
        }

        [JsonIgnore]
        public List<File> AllFiles
        {
            get
            {
                return GetFilesInDirectoriesAndChilds(new List<File>());
            }
        }
        
        #endregion

        #region Files

        private void GetFileNameAndPath(string pathWithName, out string fileName, out string path)
        {
            var parts = pathWithName.Split('/');
            fileName = parts[Math.Max(0, parts.Length - 1)];
            path = pathWithName.Substring(0, pathWithName.Length - fileName.Length);
        }

        #region Get

        public List<File> GetFiles()
        {
            if (CurrentDirectory == null)
                CurrentDirectory = this;

            return GetFilesInDirectory(CurrentDirectory);
        }

        public List<File> GetFilesInDirectory(Directory dir)
        {
            return new List<File>(dir.GetAvailableFiles());
        }

        public OperationResult FindFileByPath(string pfilePathAndNameath, out File fileFound)
        {
            string fileName, filePath;
            GetFileNameAndPath(pfilePathAndNameath, out fileName, out filePath);

            Directory dir = CurrentDirectory;
            fileFound = null;
            if (!string.IsNullOrEmpty(filePath))
            {
                dir = FindDirectory(filePath);
                if (dir == null)
                    return OperationResult.NotFound;
            }

            fileFound = dir.FindFileByName(fileName);
            if (fileFound == null)
                return OperationResult.NotFound;
            return OperationResult.Ok;
        }

        #endregion

        #region Create

        public OperationResult CreateFile(string name, out File file)
        {
            return CreateFile(this, name, out file);
        }

        public OperationResult CreateFile(Directory parent, string name, out File file)
        {
            if (parent == null)
            {
                file = null;
                return OperationResult.NotFound;
            }

            if (string.IsNullOrEmpty(name))
            {
                file = null;
                return OperationResult.InvalidValue;
            }

            if (parent.FindFileByName(name) != null)
            {
                file = null;
                return OperationResult.DuplicatedName;
            }

            parent.GetAllFiles().Add(file = new File()
            {
                Directory = parent,
                Name = name,
                Content = string.Empty,
            });

            return OperationResult.Ok;
        }

        public OperationResult AddFileWithoutValidation(string path, File fileToAdd)
        {
            Directory parent;
            CreateDiretory(path, out parent);
            AddFileWithoutValidation(parent, fileToAdd);
            return OperationResult.Ok;
        }

        public OperationResult AddFileWithoutValidation(Directory parent, File fileToAdd)
        {
            parent.GetAllFiles().Add(fileToAdd);
            fileToAdd.Directory = parent;
            return OperationResult.Ok;
        }

        #endregion

        #region Edit

        #region Delete

        public OperationResult DeleteFile(string name)
        {
            return DeleteFile(this, name);
        }

        public OperationResult DeleteFile(Directory parent, string name)
        {
            File file = parent.FindFileByName(name);
            if (file == null)
                return OperationResult.NotFound;

            parent.GetAllFiles().Remove(file);
            return OperationResult.Ok;
        }

        #endregion

        #region Update

        #region Content

        public OperationResult UpdateFileContent(string filePathAndName, string newContent)
        {
            string fileName, filePath;
            GetFileNameAndPath(filePathAndName, out fileName, out filePath);
            return UpdateFileContent(filePath, fileName, newContent);
        }

        public OperationResult UpdateFileContent(string filePath, string fileName, string newContent)
        {
            if (string.IsNullOrEmpty(filePath))
                return OperationResult.NotFound;

            if (string.IsNullOrEmpty(fileName))
                return OperationResult.NotFound;

            if (string.IsNullOrEmpty(newContent))
                return OperationResult.InvalidValue;

            var dir = FindDirectory(filePath);
            if (dir == null)
                return OperationResult.NotFound;

            var file = dir.FindFileByName(fileName);

            if (file == null)
                return OperationResult.NotFound;

            file.Content = newContent;

            return OperationResult.Ok;
        }

        #endregion content

        #region Name

        public OperationResult UpdateFileName(string filePathAndName, string newName)
        {
            string fileName, filePath;
            GetFileNameAndPath(filePathAndName, out fileName, out filePath);
            return UpdateFileName(filePath, fileName, newName);
        }

        public OperationResult UpdateFileName(string filePath, string fileName, string newName)
        {
            if (string.IsNullOrEmpty(filePath))
                return OperationResult.NotFound;

            if (string.IsNullOrEmpty(fileName))
                return OperationResult.NotFound;

            if (string.IsNullOrEmpty(newName))
                return OperationResult.InvalidValue;

            var dir = FindDirectory(filePath);
            if (dir == null)
                return OperationResult.NotFound;

            var file = dir.FindFileByName(fileName);

            if (file == null)
                return OperationResult.NotFound;

            file.Name = newName;

            return OperationResult.Ok;
        }

        #endregion name

        #region Directory

        public OperationResult UpdateFileDirectory(string filePathAndName, string newDirectory)
        {
            var parts = filePathAndName.Split('/');
            var fileName = parts[Math.Max(0, parts.Length - 1)];
            var filePath = filePathAndName.Substring(0, filePathAndName.Length - fileName.Length);

            return UpdateFileDirectory(filePath, fileName, newDirectory);
        }

        public OperationResult UpdateFileDirectory(string filePath, string fileName, string newDirectory)
        {
            if (string.IsNullOrEmpty(newDirectory))
                return OperationResult.InvalidValue;

            var dir = FindDirectory(filePath);
            if (dir == null)
                return OperationResult.NotFound;

            var file = dir.FindFileByName(fileName);

            if (file == null)
                return OperationResult.NotFound;

            var newDir = FindDirectory(newDirectory);
            if (newDir == null)
                return OperationResult.NotFound;
            
            newDir.GetAllFiles().Add(file);
            dir.GetAllFiles().Remove(file);

            file.Directory = newDir;

            return OperationResult.Ok;
        }

        #endregion Directory

        #endregion update

        #endregion edit

        #endregion files

        #region Directories

        #region Get

        public List<Directory> GetDirectories()
        {
            if (CurrentDirectory == null)
                CurrentDirectory = this;

            return GetDirectoriesIn(CurrentDirectory);
        }

        public List<Directory> GetDirectoriesIn(Directory directory)
        {
            if (directory == null)
                return null;

            return new List<Directory>(directory.GetAvailableChilds());
        }

        public Directory FindDirectory(string path, bool navigateToResult = false)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            Directory root = CurrentDirectory;
            int increment = 0;
            if (path.StartsWith(Name)) // if the path starts asking for system file
            {
                root = this;
                increment = 1;
            }

            if (path.EndsWith("/"))
                path = path.Substring(0, path.Length - 1);

            var parts = path.Split(DirectorySeparator);

            for (int i = 0 + increment; i < parts.Length; i++)
            {
                if (parts[i] == "..")
                {
                    root = root.Parent;
                    continue;
                }

                root = root.FindDirectoryByName(parts[i]);

                if (root == null)
                    break;
            }

            if (navigateToResult && root != null)
                CurrentDirectory = root;

            return root;
        }

        #endregion

        #region Create

        public OperationResult CreateDiretory(string path, out Directory dir)
        {
            dir = null;

            if (string.IsNullOrEmpty(path))
                return OperationResult.InvalidValue;

            Directory parent = CurrentDirectory;

            if (path.StartsWith(Name))
                parent = this;

            var parts = path.Split('/');
            for (int i = 0; i < parts.Length; i++)
            {
                var current = parts[i];

                if (string.IsNullOrEmpty(current))
                    continue;

                var currentDir = parent.FindDirectoryByName(current);
                if (currentDir != null)
                {
                    parent = currentDir;
                    continue;
                }

                var result = CreateDiretory(parent, current, out dir);

                parent = dir;

                if (result != OperationResult.Ok)
                    return result;
            }

            dir = parent;

            return OperationResult.Ok;
        }

        public OperationResult CreateDiretory(Directory parent, string name, out Directory dir)
        {
            if (parent == null)
            {
                dir = null;
                return OperationResult.NotFound;
            }

            if (string.IsNullOrEmpty(name))
            {
                dir = null;
                return OperationResult.InvalidValue;
            }

            if (parent.FindDirectoryByName(name) != null)
            {
                dir = null;
                return OperationResult.DuplicatedName;
            }

            parent.GetAllChilds().Add(dir = new Directory()
            {
                Name = name,
                Parent = parent,
            });

            return OperationResult.Ok;
        }

        #endregion

        #endregion
    }
}
