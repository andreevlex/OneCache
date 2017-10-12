using System;
using System.IO;
using System.Linq;

namespace OneCache
{
    public class Cache
    {
        public static long GetDirectorySize(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);
            return directoryInfo.GetFiles().Sum(file => file.Length) +
                   directoryInfo.GetDirectories().Sum(dir => GetDirectorySize(dir.FullName));
        }

        public static int GetAge(string directory)
        {
            TimeSpan ts = DateTime.Now - File.GetLastAccessTime(directory);
            return ts.Days;
        }

        public string Path { get; set; }
        public string UUID { get; set; }
        public long Size { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", UUID, Size);
        }
    }
}
