using System;
using Brejc.Rasters;
using SrtmPlaying.Png;

namespace SrtmPlaying.Srtm
{
    public class Dem16RasterAsPngDataSource : IPngBitmapDataSource
    {
        [CLSCompliant(false)]
        public Dem16RasterAsPngDataSource(IRaster raster, Func<short, ushort> valueTransformFunc)
        {
            if (raster.RasterValueType != RasterValueType.Int16)
                throw new ArgumentException("This class only supports 16-bit rasters.");

            this.raster = raster;
            this.valueTransformFunc = valueTransformFunc;
        }

        public bool IsRaw { get { return false; } }
        public int Width { get { return raster.RasterWidth; } }
        public int Height { get { return raster.RasterHeight; } }

        [CLSCompliant(false)]
        public unsafe byte* GetRawScanline(int y)
        {
            throw new NotSupportedException();
        }

        public byte[] GetScanline(int y)
        {
            byte[] scanlineData = new byte[raster.RasterWidth * 2];

            for (int x = 0; x < raster.RasterWidth; x++)
            {
                short rasterValue = raster.GetCellValueInt16(x, y) ?? -1;
                ushort transformedValue = valueTransformFunc(rasterValue);

                scanlineData[x*2] = (byte)(transformedValue >> 8);
                scanlineData[x*2 + 1] = (byte)transformedValue;
            }

            return scanlineData;
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