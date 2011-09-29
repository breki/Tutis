using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Gallio.Framework;
using MbUnit.Framework;

namespace Piksla
{
    public class BitmapMergingTests
    {
        [Test]
        public void MeasureMerging()
        {
            Bitmap bitmap1 = new Bitmap(500, 500);
            Bitmap bitmap2 = new Bitmap(500, 500);

            Graphics g1 = Graphics.FromImage(bitmap1);
            g1.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g1.SmoothingMode = SmoothingMode.HighQuality;

            Graphics g2 = Graphics.FromImage(bitmap2);
            g2.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g2.SmoothingMode = SmoothingMode.HighQuality;

            Stopwatch s = new Stopwatch();
            s.Start();

            for (int i = 0; i < 1000; i++)
            {
                g1.Clear(Color.White);
                g2.Clear(Color.FromArgb(0x0, 0xff, 0xff, 0xff));

                Brush redBrush = new SolidBrush(Color.Red);
                Brush blueBrush = new SolidBrush(Color.Blue);

                g1.FillRectangle(redBrush, 10, 10, 110, 110);
                g2.FillRectangle(blueBrush, 90, 90, 190, 190);
                g1.CompositingMode = CompositingMode.SourceOver;
                g1.CompositingQuality = CompositingQuality.HighQuality;
                g1.DrawImageUnscaled(bitmap2, 0, 0);
            }

            TestLog.WriteLine(s.ElapsedMilliseconds);

            bitmap1.Save("bitmap1.png", ImageFormat.Png);
            bitmap2.Save("bitmap2.png", ImageFormat.Png);
        }

        [Test]
        public void MeasureDirect()
        {
            Bitmap bitmap3 = new Bitmap(500, 500);

            Graphics g3 = Graphics.FromImage(bitmap3);
            g3.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g3.SmoothingMode = SmoothingMode.HighQuality;

            Brush redBrush = new SolidBrush(Color.Red);
            Brush blueBrush = new SolidBrush(Color.Blue);

            Stopwatch s = new Stopwatch();
            s.Start();

            for (int i = 0; i < 1000; i++)
            {
                g3.Clear(Color.White);

                g3.CompositingMode = CompositingMode.SourceOver;
                g3.CompositingQuality = CompositingQuality.HighQuality;
                g3.FillRectangle(redBrush, 10, 10, 110, 110);
                g3.FillRectangle(blueBrush, 90, 90, 190, 190);
            }

            TestLog.WriteLine(s.ElapsedMilliseconds);

            bitmap3.Save("bitmap3.png", ImageFormat.Png);
        }
    }

    //g.DrawImage();
    //g.DrawImageUnscaled();
    //g.CompositingMode= CompositingMode.SourceOver;
    //g.CompositingMode= CompositingMode.SourceCopy;
    //g.CompositingQuality = CompositingQuality.HighQuality;
    // g.InterpolationMode = InterpolationMode.Low;

    // http://stackoverflow.com/questions/264720/gdi-graphicsdrawimage-really-slow

    // http://freeimage.sourceforge.net/index.html

    // TextureBrush myBrush = new TextureBrush(bmp)

    // http://msdn.microsoft.com/en-us/library/1bttkazd(VS.71).aspx

    // http://msdn.microsoft.com/en-us/library/6tf7sa87(v=VS.71).aspx
}