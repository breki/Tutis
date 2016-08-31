using System.Drawing;
using System.Drawing.Imaging;
using Brejc.DemLibrary.Srtm;
using Brejc.Rasters;
using LibroLib.FileSystem;
using NUnit.Framework;

namespace SrtmPlaying.Tests
{
    public class SrtmTests
    {
        [Test]
        public void Test()
        {
            ISrtm1CellFileReader cellFileReader = new Hgt1FileReader(new WindowsFileSystem());
            IRaster cell = cellFileReader.ReadFromFile(@"D:\brisi\N00E006.hgt");

            using (Bitmap bitmap = new Bitmap(
                cell.RasterWidth, cell.RasterHeight, PixelFormat.Format16bppGrayScale))
            {
                // Lock the unmanaged bits for efficient writing.
                var data = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadWrite,
                    bitmap.PixelFormat);

                //for (int y = 0; y < cell.CellHeight; y++)
                //{
                //    for (int x = 0; x < cell.CellWidth; x++)
                //    {
                //        short value = cell.GetCellValueInt16(x, y) ?? 0;
                //        //bitmap.SetPixel(x, y, Color.FromArgb((byte)(value & 0xff), (byte)(value >> 8), 0));
                //        bitmap.SetPixel(x, y, Color.FromArgb((byte)value, (byte)value, (byte)value));
                //    }
                //}

                bitmap.Save("test.png");
            }
        }
    }
}
