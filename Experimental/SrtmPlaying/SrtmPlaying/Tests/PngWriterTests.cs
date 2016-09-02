using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using LibroLib;
using LibroLib.FileSystem;
using NUnit.Framework;
using SrtmPlaying.Png;

namespace SrtmPlaying.Tests
{
    public class PngWriterTests
    {
        [Test]
        public void WritePng()
        {
            //Bitmap originalBitmap = (Bitmap)Image.FromFile (Constants.DataSamplesPath + "Bitmaps/l-33-2.jpg");
            //Bitmap originalBitmap = (Bitmap)Image.FromFile (Constants.DataSamplesPath + "Bitmaps/05331.jpg");
            //Bitmap originalBitmap = (Bitmap)Image.FromFile (Constants.DataSamplesPath + "Bitmaps/SimpleGeoref.png");
            //Bitmap originalBitmap = (Bitmap)Image.FromFile (Constants.DataSamplesPath + "Bitmaps/EuropeMercator.png");
            Bitmap originalBitmap = (Bitmap)Image.FromFile (@"D:\hg\tutis\Experimental\SrtmPlaying\SrtmPlaying\data\SimpleColored.png");
            //Bitmap originalBitmap = (Bitmap)Image.FromFile (@"D:\hg\tutis\Experimental\SrtmPlaying\SrtmPlaying\data\05331.jpg");

            string outputFileName = Path.GetFullPath(Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "output/pngwriter_sharp.png"));

            using (Bitmap workingBitmap = new Bitmap (originalBitmap.Width, originalBitmap.Height))
            {
                using (Graphics gfx = Graphics.FromImage(workingBitmap))
                    gfx.DrawImage(originalBitmap, 0, 0);

                IPngWriter pngWriter = new PngWriter(new ZLibCompressorUsingSharpZipLib());

                PngWriterSettings settings = new PngWriterSettings();
                settings.CompressionLevel = 9;
                settings.Transparency = PngTransparency.AutoDetect;

                Stopwatch watch = new Stopwatch();

                watch.Restart();
                pngWriter.WritePng (workingBitmap, settings, outputFileName, new WindowsFileSystem ());
            }

            ValidatePng(outputFileName);
        }

        [Test]
        public void WritePngPart()
        {
            Bitmap originalBitmap = (Bitmap)Image.FromFile (@"D:\hg\tutis\Experimental\SrtmPlaying\SrtmPlaying\data\05331.jpg");
            using (Bitmap workingBitmap = new Bitmap (originalBitmap.Width, originalBitmap.Height))
            {
                using (Graphics gfx = Graphics.FromImage (workingBitmap))
                    gfx.DrawImage (originalBitmap, 0, 0);

                IPngWriter pngWriter = new PngWriter (new ZLibCompressorUsingSharpZipLib());

                PngWriterSettings settings = new PngWriterSettings ();
                settings.CompressionLevel = 9;
                settings.Transparency = PngTransparency.Opaque;

                Stopwatch watch = new Stopwatch ();

                watch.Restart ();
                string outputFileName = "output/pngwriter_sharp_part.png";

                using (IPngBitmapDataSource raw = new RawReadOnlyBitmap(workingBitmap))
                using (Stream stream = new WindowsFileSystem().OpenFileToWrite(outputFileName))
                {
                    pngWriter.WritePngPart(raw, 120, 160, 1200, 800, settings, stream);
                }

                Debug.WriteLine ("sharp: {0} ms", watch.ElapsedMilliseconds);

                //watch.Restart ();
                //workingBitmap.Save ("output/pngwriter_net.png", ImageFormat.Png);
                //Debug.WriteLine ("net: {0} ms", watch.ElapsedMilliseconds);
            }
        }

        private static void ValidatePng(string outputFileName)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(
                Path.Combine(TestContext.CurrentContext.TestDirectory,
                    @"..\..\..\tools\pngcheck\pngcheck.exe"),
                "-vt {0}".Fmt(outputFileName))
            {
                CreateNoWindow = true,
                ErrorDialog = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            StringBuilder outputText = new StringBuilder();
            using (Process process = Process.Start(processStartInfo))
            {
                // ReSharper disable once PossibleNullReferenceException
                process.ErrorDataReceived += (o, e) => { outputText.Append(e.Data); };
                process.OutputDataReceived += (o, e) => { outputText.Append(e.Data); };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.Start();
                process.WaitForExit(20*1000);

                Assert.AreEqual(0, process.ExitCode, outputText.ToString());

                Console.Write(outputText);
            }
        }
    }
}