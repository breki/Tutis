using System;

namespace SrtmPlaying.Png.ChunkWriters.Idat
{
    public class PngFilterSub : IPngFilter
    {
        public byte[] Filter(
            IPngBitmapScanline scanline, 
            PngImageType imageType, 
            bool useAlpha, 
            int destinationPixelSize, 
            int clipWidth)
        {
            byte[] filtered = new byte[clipWidth * destinationPixelSize];
            int fi = 0;

            for (int xx = 0; xx < clipWidth; xx++)
            {
                switch (imageType)
                {
                    case PngImageType.Rgb8:
                        byte blue = scanline.NextDiff();
                        byte green = scanline.NextDiff();
                        byte red = scanline.NextDiff();

                        filtered[fi++] = red;
                        filtered[fi++] = green;
                        filtered[fi++] = blue;

                        if (scanline.HasAlpha)
                        {
                            byte alpha = scanline.NextDiff();

                            if (useAlpha)
                                filtered[fi++] = alpha;
                        }

                        break;

                    case PngImageType.Grayscale16:
                        byte hi = scanline.NextDiff();
                        byte lo = scanline.NextDiff();

                        filtered[fi++] = hi;
                        filtered[fi++] = lo;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            return filtered;
        }
    }
}