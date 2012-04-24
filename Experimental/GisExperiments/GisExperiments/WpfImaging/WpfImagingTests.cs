using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MbUnit.Framework;
using Color = System.Windows.Media.Color;

namespace GisExperiments.WpfImaging
{
    // FreeImageNET
    // WPF
    // PaintMono
    public class WpfImagingTests
    {
        [Test]
        public void Test()
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(@"..\..\..\samples\sample.png");
            BitmapImage bi = ToWpfBitmap(bitmap);
            //ToWinFormsBitmap(bi).Save("test.png", ImageFormat.Png);

            FormatConvertedBitmap bi2 = new FormatConvertedBitmap ();
            bi2.BeginInit ();
            bi2.Source = bi;
            bi2.DestinationFormat = PixelFormats.Indexed8;
            bi2.DestinationPalette = new BitmapPalette(bi, 256);
            //bi2.DestinationPalette = new BitmapPalette(new[] { Colors.Black, Colors.Red, Colors.White, Colors.Green, Colors.Blue, Colors.Yellow });
            bi2.EndInit ();
            
            PngBitmapEncoder encoder = new PngBitmapEncoder ();
            //encoder.Palette = new BitmapPalette(bi, 256);
            encoder.Frames.Add(BitmapFrame.Create(bi2));
            encoder.Save(new FileStream("test.png", FileMode.Create));
        }

        public static BitmapImage ToWpfBitmap (Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream ())
            {
                bitmap.Save (stream, ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage ();
                result.BeginInit ();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit ();
                result.Freeze ();
                return result;
            }
        }

        public static Bitmap ToWinFormsBitmap (BitmapSource bitmapsource)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(stream);

                using (var tempBitmap = new Bitmap(stream))
                {
                    // According to MSDN, one "must keep the stream open for the lifetime of the Bitmap."
                    // So we return a copy of the new bitmap, allowing us to dispose both the bitmap and the stream.
                    return new Bitmap(tempBitmap);
                }
            }
        }
    }
}
