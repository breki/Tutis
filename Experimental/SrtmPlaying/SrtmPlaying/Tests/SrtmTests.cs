using System.IO;
using Brejc.DemLibrary.Srtm;
using Brejc.Rasters;
using LibroLib;
using LibroLib.FileSystem;
using NUnit.Framework;
using SrtmPlaying.Png;
using SrtmPlaying.Srtm;

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

            IPngWriter pngWriter = new PngWriter();
            SrtmTilePngFileWriter tileWriter = new SrtmTilePngFileWriter(fileSystem, pngWriter);
            tileWriter.WriteToFile(Path.Combine(testDir, "output", "tile.png"), cell);
        }
    }
}
