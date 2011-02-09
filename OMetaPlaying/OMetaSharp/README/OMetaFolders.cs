using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OMetaSharp
{
    internal class OMetaFolders
    {
        public static string Root
        {
            get
            {
                // Go one above the README project root
                return ProjectPaths.Subdir("..");
            }
        }

        public static string Examples
        {
            get
            {
                return Path.Combine(Root, "Examples");
            }
        }

        public static string HostLanguages(string host)
        {
            return Path.Combine(Path.Combine(Root, "HostLanguages"), host);
        }

        public static string Runtime
        {
            get
            {
                return Path.Combine(Root, "Runtime");
            }
        }
    }
}
