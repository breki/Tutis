using System;
using Brejc.Rasters;
using SrtmPlaying.Png;

namespace SrtmPlaying.Srtm
{
    public class Dem16RasterAsPngScanline : IPngBitmapScanline
    {
        public Dem16RasterAsPngScanline(IRasterArray raster, Func<short, ushort> valueTransformFunc, int y)
        {
            scanlineData = new byte[raster.RasterWidth * 2];

            for (int x = 0; x < raster.RasterWidth; x++)
            {
                short rasterValue = raster.GetCellValueInt16(x, y) ?? -1;
                ushort transformedValue = valueTransformFunc(rasterValue);

                scanlineData[x * 2] = (byte)(transformedValue >> 8);
                scanlineData[x * 2 + 1] = (byte)transformedValue;
            }
        }

        public bool HasAlpha { get { return false; } }

        public byte NextByte()
        {
            return scanlineData[cursor++];
        }

        private readonly byte[] scanlineData;
        private int cursor;
    }
}