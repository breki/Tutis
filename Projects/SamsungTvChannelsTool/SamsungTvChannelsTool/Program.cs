using Brejc.Common.FileSystem;

namespace SamsungTvChannelsTool
{
    public class Program
    {
        public static void Main (string[] args)
        {
            IFileSystem fileSystem = new WindowsFileSystem();
            ScmFileReader reader = new ScmFileReader(fileSystem);
            ChannelsInfo channels = reader.ReadScmFile(args[0]);
        }
    }
}
