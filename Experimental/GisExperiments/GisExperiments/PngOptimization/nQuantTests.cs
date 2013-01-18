using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MbUnit.Framework;
using nQuant;

namespace GisExperiments.PngOptimization
{
    public class nQuantTests
    {
        [Test]
        public void Test()
        {
            WuQuantizer quantizer = new WuQuantizer ();
            using (Bitmap bitmap = (Bitmap)Bitmap.FromFile(@"../../../samples/sample.png"))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                const int Runs = 20;
                for (int i = 0; i < Runs; i++)
                {
                    int alphaTransparency = 0;
                    int alphaFader = 0;
                    using (Image quantized = quantizer.QuantizeImage (bitmap, alphaTransparency, alphaFader))
                        quantized.Save ("output_nQuant.png", ImageFormat.Png);                    
                }

                Debug.WriteLine("nQuant: {0} ms/image", sw.ElapsedMilliseconds / Runs);

                sw.Start();
                for (int i = 0; i < Runs; i++)
                {
                    using (Stream file = File.Open("output_net8.png", FileMode.Create))
                        WriteBitmapAsPng8(bitmap, file);
                }

                Debug.WriteLine (".net 8bit: {0} ms/image", sw.ElapsedMilliseconds / Runs);
            }
        }

        private static void WriteBitmapAsPng8 (Bitmap bitmap, Stream stream)
        {
            BitmapImage wpfBitmap = ToWpfBitmap (bitmap);
            FormatConvertedBitmap wpfDepth8Bitmap = new FormatConvertedBitmap ();
            wpfDepth8Bitmap.BeginInit ();
            wpfDepth8Bitmap.Source = wpfBitmap;
            wpfDepth8Bitmap.DestinationFormat = PixelFormats.Indexed8;
            wpfDepth8Bitmap.DestinationPalette = new BitmapPalette (wpfBitmap, 256);
            wpfDepth8Bitmap.EndInit ();

            PngBitmapEncoder encoder = new PngBitmapEncoder ();
            encoder.Frames.Add (BitmapFrame.Create (wpfDepth8Bitmap));
            encoder.Save (stream);
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
    }
}