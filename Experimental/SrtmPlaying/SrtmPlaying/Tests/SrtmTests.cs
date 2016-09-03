using System;
using System.IO;
using System.IO.Compression;
using Brejc.DemLibrary.Srtm;
using Brejc.Rasters;
using LibroLib;
using LibroLib.FileSystem;
using LibroLib.Text;
using NUnit.Framework;
using SrtmPlaying.Png;
using SrtmPlaying.Srtm;

namespace SrtmPlaying.Tests
{
    public class SrtmTests
    {
        [Explicit]
        [TestCase("N00E006")]
        [TestCase("N00E010")]
        [TestCase("N48E119")]
        [TestCase("S04W079")]
        public void WriteSrtmCellToPng(string cellName)
        {
            string zipFileName = @"D:\hg\tutis\Experimental\SrtmPlaying\SrtmPlaying\data\{0}.SRTMGL1.hgt.zip"
                .Fmt(cellName);

            ZipFile.ExtractToDirectory(zipFileName, tempDir);
            string cellFileName = Path.Combine(tempDir, "{0}.hgt".Fmt(cellName));

            ISrtm1CellFileReader cellFileReader = new Hgt1FileReader(fileSystem);
            IRaster cell = cellFileReader.ReadFromFile(cellFileName);

            IPngWriter pngWriter = new PngWriter();
            SrtmTilePngFileWriter tileWriter = new SrtmTilePngFileWriter(fileSystem, pngWriter, EncodeSrtmElevationToGrayscale);
            string outputFileName = Path.Combine(testDir, "output", "{0}.png".Fmt(cellName));
            tileWriter.WriteToFile(outputFileName, cell);

            PngValidator.ValidatePng(outputFileName);

            long pngSize = fileSystem.GetFileInformation(outputFileName).Length;
            long zipSize = fileSystem.GetFileInformation(zipFileName).Length;
            Console.WriteLine(
                "Cell {0}: {1:P} reduction ({2})".Fmt(
                    cellName, 
                    1d - (double)pngSize / zipSize,
                    FormattingUtils.FormatByteSizeToString(zipSize - pngSize)));
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            fileSystem = new WindowsFileSystem();

            testDir = TestContext.CurrentContext.TestDirectory;
            tempDir = Path.Combine(testDir, "temp");
            fileSystem.DeleteDirectory(tempDir);
            fileSystem.EnsureDirectoryExists(tempDir);
        }

        private static ushort EncodeSrtmElevationToGrayscale(short elevation)
        {
            if (elevation == short.MinValue)
                return 0;

            const short MinElevationSupported = -1000;
            const short MaxElevationSupported = 9000;
            const short Multiplier = 6;
            const ushort SupportedRange = (MaxElevationSupported - MinElevationSupported) * Multiplier;

            if (elevation < MinElevationSupported)
                elevation = MinElevationSupported;
            if (elevation > MaxElevationSupported)
                elevation = MaxElevationSupported;

            ushort grayscaleValue = (ushort)(ushort.MaxValue - SupportedRange + elevation * Multiplier - MinElevationSupported);
            return grayscaleValue;
        }

        private IFileSystem fileSystem;
        private string tempDir;
        private string testDir;
    }
}
