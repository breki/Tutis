using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace OMetaSharp
{
    public static class ProjectPaths
    {
        public static string Base
        {
            get
            {
                return GetBase(Assembly.GetCallingAssembly());                
            }
        }
        
        public static string GetBase(Assembly assembly)
        {
            var mainModulePath = assembly.Location;
            var relativePathToBase = @"..\..\..\";
            var basePath = Path.Combine(mainModulePath, relativePathToBase);
            var cleanedPath = new DirectoryInfo(basePath);
            string finalPath = cleanedPath.FullName;

            Debug.Assert(finalPath != null);
            return finalPath;
        }

        public static string Subdir(string subdirPath)
        {
            return Subdir(Assembly.GetCallingAssembly(), subdirPath);
        }

        private static string Subdir(Assembly assembly, string subdirPath)
        {
            return (new FileInfo(Path.Combine(GetBase(assembly), subdirPath))).FullName;
        }

        public static string Programs
        {
            get
            {
                return Subdir(Assembly.GetCallingAssembly(), "Programs");
            }
        }

        public static string Grammars
        {
            get
            {
                return Subdir(Assembly.GetCallingAssembly(), "Grammars");
            }
        }

        public static string GeneratedCode
        {
            get
            {
                return Subdir(Assembly.GetCallingAssembly(), "GeneratedCode");
            }
        }
    }
}
