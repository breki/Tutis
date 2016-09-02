using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SrtmPlaying.Png
{
    public sealed unsafe class RawReadOnlyBitmap : IPngBitmapDataSource
    {
        public RawReadOnlyBitmap (Bitmap bitmap)
        {
            this.bitmap = bitmap;
            width = bitmap.Width;
            height = bitmap.Height;
            LockBitmap();
        }

        public bool IsRaw { get { return true; } }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public byte* GetRawScanline (int y)
        {
            return pBase + y*wwidth;
        }

        public byte[] GetScanline(int y)
        {
            throw new NotSupportedException();
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        private void Dispose (bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    if (pBase != null)
                        UnlockBitmap();
                }

                disposed = true;
            }
        }

        private void LockBitmap ()
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds (ref unit);
            Rectangle bounds = new Rectangle (
                (int)boundsF.X,
                (int)boundsF.Y,
                (int)boundsF.Width,
                (int)boundsF.Height);

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length. 
            pixelDataSize = 4;
            wwidth = (int)boundsF.Width * pixelDataSize;
            if (wwidth % 4 != 0)
                wwidth = 4 * (wwidth / 4 + 1);

            bitmapData =
                bitmap.LockBits (bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            pBase = (byte*)bitmapData.Scan0.ToPointer ();
        }

        private void UnlockBitmap ()
        {
            bitmap.UnlockBits (bitmapData);
            bitmapData = null;
            pBase = null;
        }

        private readonly Bitmap bitmap;
        private bool disposed;
        private int wwidth;
        private BitmapData bitmapData;
        private byte* pBase = null;
        private int pixelDataSize;
        private readonly int width;
        private readonly int height;
    }
}