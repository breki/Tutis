using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace LineByLine.Console
{
    public class FullPath : IPathBuilder
    {
        public FullPath(string path)
        {
            if (Path.IsPathRooted(path))
                fullPath = path;
            else
                fullPath = Path.GetFullPath(path);
        }

        public bool DirectoryExists
        {
            get { return Directory.Exists(fullPath); }
        }

        public bool EndsWithDirectorySeparator
        {
            get
            {
                return fullPath.EndsWith(
                    Path.DirectorySeparatorChar.ToString(),
                    StringComparison.OrdinalIgnoreCase)
                    || fullPath.EndsWith(
                    Path.AltDirectorySeparatorChar.ToString(),
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        public string FileName
        {
            get { return Path.GetFileName(fullPath); }
        }

        public int Length
        {
            get { return fullPath.Length; }
        }

        /// <summary>
        /// Gets the path that is a parent to the current path in this object.
        /// </summary>
        /// <value>The parent path.</value>
        public FullPath ParentPath
        {
            get
            {
                return new FullPath(Path.GetDirectoryName(fullPath));
            }
        }

        public FileFullPath AddFileName (string fileNameFormat, params object[] args)
        {
            string fileName = string.Format(
                CultureInfo.InvariantCulture,
                fileNameFormat,
                args);
            return new FileFullPath(CombineWith(new LocalPath(fileName)).ToString());
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
        public FullPath AddRaw(string rawString)
        {
            return new FullPath(fullPath + rawString);
        }

        public FullPath CombineWith (LocalPath localPath)
        {
            return new FullPath(Path.Combine(fullPath, localPath.ToString()));
        }

        public FullPath CombineWith(string localPath)
        {
            return CombineWith(new LocalPath(localPath));
        }

        public LocalPath DebasePath(FullPath basePath)
        {
            if (!IsSubpathOf(basePath))
                return null;

            if (basePath.Length > 0 && !basePath.EndsWithDirectorySeparator)
                basePath = basePath.AddRaw(Path.DirectorySeparatorChar.ToString());

            string debased = fullPath.Substring(basePath.Length);
            if (debased.StartsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase)
                || debased.StartsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase))
                debased = debased.Substring(1);

            return new LocalPath(debased);
        }

        public void EnsureExists()
        {
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            FullPath that = (FullPath)obj;
            return string.Equals(fullPath, that.fullPath);
        }

        public override int GetHashCode()
        {
            return fullPath.GetHashCode();
        }

        public bool IsSubpathOf (FullPath basePath)
        {
            return fullPath.StartsWith(basePath.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return fullPath;
        }

        public FileFullPath ToFileFullPath()
        {
            return new FileFullPath(fullPath);
        }

        private string fullPath;
    }
}