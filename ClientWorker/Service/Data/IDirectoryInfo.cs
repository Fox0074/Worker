using System;
using System.Collections.Generic;
using System.IO;

namespace Interfaces
{
    [Serializable]
    public class IDirectoryInfo
    {
        public List<string> Directories { get; set; } = new List<string>();
        public List<IFileInfo> FilesInfo { get; set; } = new List<IFileInfo>();
    }
    [Serializable]
    public class IFileInfo
    {
        public string fullName;
        public long length;
        public FileAttributes attributes;
        public DateTime creationTime;
        public string directory;
        public string name;
        public DateTime lastAccessTime;
        public DateTime lastWriteTime;

        public IFileInfo(FileInfo fileInfo)
        {
            fullName = fileInfo.FullName;
            length = fileInfo.Length;
            attributes = fileInfo.Attributes;
            creationTime = fileInfo.CreationTime;
            directory = fileInfo.Directory.FullName;
            name = fileInfo.Name;
            lastAccessTime = fileInfo.LastAccessTime;
            lastWriteTime = fileInfo.LastWriteTime;
        }

    }
}
