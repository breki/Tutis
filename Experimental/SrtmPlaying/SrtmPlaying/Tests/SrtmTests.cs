using System.IO;
using System.IO.Compression;
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
        [TestCase("N00E010")]
        public void Test(string cellName)
        {
            string testDir = TestContext.CurrentContext.TestDirectory;

            IFileSystem fileSystem = new WindowsFileSystem();

            string zipFileName = @"D:\hg\tutis\Experimental\SrtmPlaying\SrtmPlaying\data\{0}.SRTMGL1.hgt.zip"
                .Fmt(cellName);

            string tempDir = Path.Combine(testDir, "temp");
            fileSystem.DeleteDirectory(tempDir);
            fileSystem.EnsureDirectoryExists(tempDir);

            ZipFile.ExtractToDirectory(zipFileName, tempDir);

            string cellFileName = Path.Combine(tempDir, @"{0}.hgt".Fmt(cellName));

            ISrtm1CellFileReader cellFileReader = new Hgt1FileReader(fileSystem);
            IRaster cell = cellFileReader.ReadFromFile(cellFileName);

            IPngWriter pngWriter = new PngWriter(new ZLibCompressorUsingSharpZipLib());
            SrtmTilePngFileWriter tileWriter = new SrtmTilePngFileWriter(fileSystem, pngWriter);
            string outputFileName = Path.Combine(testDir, "output", "tile.png");
            tileWriter.WriteToFile(outputFileName, cell);

            PngValidator.ValidatePng(outputFileName);
        }
    }
}
