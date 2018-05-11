using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Interfaces
{
    [Serializable]
    public class IDirectoryInfo
    {
        public List<string> Directories { get; set; } = new List<string>();
        public List<FileInfo> FilesInfo { get; set; } = new List<FileInfo>();
    }
}
