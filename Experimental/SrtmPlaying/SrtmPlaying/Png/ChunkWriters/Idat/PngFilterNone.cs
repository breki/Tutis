using System;

namespace SrtmPlaying.Png.ChunkWriters.Idat
{
    public class PngFilterNone : IPngFilter
    {
        public void Filter(
            IPngBitmapScanline scanline, 
            PngImageType imageType, 
            bool useAlpha,
            int clipWidth, 
            byte[] filtered)
        {
            int fi = 0;

            for (int xx = 0; xx < clipWidth; xx++)
            {
                switch (imageType)
                {
                    case PngImageType.Rgb8:
                        byte blue = scanline.NextByte();
                        byte green = scanline.NextByte();
                        byte red = scanline.NextByte();

                        filtered[fi++] = red;
                        filtered[fi++] = green;
                        filtered[fi++] = blue;

                        if (scanline.HasAlpha)
                        {
                            byte alpha = scanline.NextByte();
                            if (useAlpha)
                                filtered[fi++] = alpha;
                        }

                        break;

                    case PngImageType.Grayscale16:
                        byte hi = scanline.NextByte();
                        byte lo = scanline.NextByte();
                        filtered[fi++] = hi;
                        filtered[fi++] = lo;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}