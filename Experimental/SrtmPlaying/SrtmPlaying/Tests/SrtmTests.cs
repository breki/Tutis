using System.IO;
using Brejc.DemLibrary.Srtm;
using Brejc.Rasters;
using LibroLib;
using LibroLib.FileSystem;
using NUnit.Framework;

namespace SrtmPlaying.Tests
{
    public class SrtmTests
    {
        //[Explicit]
        [TestCase("N00E006")]
        public void Test(string cellName)
        {
            string testDir = TestContext.CurrentContext.TestDirectory;

            IFileSystem fileSystem = new WindowsFileSystem();

            string zipFileName = @"D:\hg\tutis\Experimental\SrtmPlaying\SrtmPlaying\data\{0}.SRTMGL1.hgt.zip"
                .Fmt(cellName);

            string tempDir = Path.Combine(testDir, "temp");
            fileSystem.DeleteDirectory(tempDir);
            fileSystem.EnsureDirectoryExists(tempDir);

            System.IO.Compression.ZipFile.ExtractToDirectory(zipFileName, tempDir);

            string cellFileName = Path.Combine(tempDir, @"{0}.hgt".Fmt(cellName));

            ISrtm1CellFileReader cellFileReader = new Hgt1FileReader(fileSystem);
            IRaster cell = cellFileReader.ReadFromFile(cellFileName);
        }

        [Test]
        public void Test()
        {
        }
    }
}
