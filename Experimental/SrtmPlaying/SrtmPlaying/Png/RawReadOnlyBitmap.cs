using System;
using System.Drawing;
using System.Drawing.Imaging;
using LibroLib;

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

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int PixelSize
        {
            get
            {
                switch (bitmap.PixelFormat)
                {
                    case PixelFormat.Format32bppArgb:
                        return 4;

                    case PixelFormat.Indexed:
                    case PixelFormat.Gdi:
                    case PixelFormat.Alpha:
                    case PixelFormat.PAlpha:
                    case PixelFormat.Extended:
                    case PixelFormat.Canonical:
                    case PixelFormat.Undefined:
                    case PixelFormat.Format1bppIndexed:
                    case PixelFormat.Format4bppIndexed:
                    case PixelFormat.Format8bppIndexed:
                    case PixelFormat.Format16bppGrayScale:
                    case PixelFormat.Format16bppRgb555:
                    case PixelFormat.Format16bppRgb565:
                    case PixelFormat.Format16bppArgb1555:
                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppRgb:
                    case PixelFormat.Format32bppPArgb:
                    case PixelFormat.Format48bppRgb:
                    case PixelFormat.Format64bppArgb:
                    case PixelFormat.Format64bppPArgb:
                    case PixelFormat.Max:
                    default:
                        throw new NotImplementedException("Pixel format: {0}".Fmt(bitmap.PixelFormat));
                }
            }
        }

        public IPngBitmapScanline GetScanline(int y)
        {
            return new RawReadOnlyBitmapScanline(
                pBase + y * wwidth, PixelSize, true);
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