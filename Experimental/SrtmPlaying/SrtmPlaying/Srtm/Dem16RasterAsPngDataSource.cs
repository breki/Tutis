using System;
using Brejc.Rasters;
using SrtmPlaying.Png;

namespace SrtmPlaying.Srtm
{
    public class Dem16RasterAsPngDataSource : IPngBitmapDataSource
    {
        public Dem16RasterAsPngDataSource(IRaster raster, Func<short, ushort> valueTransformFunc)
        {
            if (raster.RasterValueType != RasterValueType.Int16)
                throw new ArgumentException("This class only supports 16-bit rasters.");

            this.raster = raster;
            this.valueTransformFunc = valueTransformFunc;
        }

        public int Width { get { return raster.RasterWidth; } }
        public int Height { get { return raster.RasterHeight; } }
        public int PixelSize { get { return 2; } }

        public unsafe byte* GetRawScanline(int y)
        {
            throw new NotSupportedException();
        }

        public IPngBitmapScanline GetScanline(int y)
        {
            return new Dem16RasterAsPngScanline(raster, valueTransformFunc, y);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
            }

            disposed = true;
        }

        private readonly IRaster raster;
        private readonly Func<short, ushort> valueTransformFunc;
        private bool disposed;
    }
}