using System.IO;
using System.Text;

namespace Freude.Tests
{
    public static class TestHelper
    {
        public static string GetDataDirPath (string path, int depthLevel = 0)
        {
            string resultPath;
#if NCRUNCH
             resultPath = @"D:\hg\Maperitive\current\Azurite\data";
#else
            StringBuilder s = new StringBuilder ();
            for (int i = 0; i < depthLevel; i++)
                s.Append (@"..\");

            s.Append (@"data\");
            resultPath = s.ToString ();
#endif

            return Path.Combine (resultPath, path);
        }

        public static string GetSolutionDirPath (string path, int depthLevel = 0)
        {
            string resultPath;
#if NCRUNCH
             resultPath = @"D:\hg\tutis\Projects\Freude";
#else
            StringBuilder s = new StringBuilder ();
            for (int i = 0; i < depthLevel; i++)
                s.Append (@"..\");

            resultPath = s.ToString ();
#endif

            return Path.Combine (resultPath, path);
        }
    }
}