using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using Gallio.Framework;
using MbUnit.Framework;

namespace Piksla
{
    public class Tests
    {
        [Test]
        public void Test()
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            using (Bitmap bitmap = new Bitmap(512, 512))
            {
                for (int i = 0; i < 10; i++)
                {
                    Color color = Color.Red;
                    for (int y = 0; y < 512; y++)
                    {
                        for (int x = 0; x < 512; x++)
                        {
                            bitmap.SetPixel(x, y, color);
                        }
                    }
                }

                bitmap.Save("test.png", ImageFormat.Png);
            }

            w.Stop();
            TestLog.WriteLine(w.ElapsedMilliseconds);
        }

        [Test]
        public void Test2()
        {
            Stopwatch w = new Stopwatch();
            w.Start();

            UnsafeBitmap bitmap = new UnsafeBitmap(512, 512);
            bitmap.LockBitmap();

            for (int i = 0; i < 10; i++)
            {
                Color color = Color.Red;
                for (int y = 0; y < 512; y++)
                {
                    for (int x = 0; x < 512; x++)
                    {
                        bitmap.SetPixel(x, y, Color.Red);
                    }
                }
            }

            bitmap.UnlockBitmap();
            bitmap.Bitmap.Save("test.png", ImageFormat.Png);

            w.Stop();
            TestLog.WriteLine(w.ElapsedMilliseconds);
        }
    }
}