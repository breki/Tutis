using System;

namespace SrtmPlaying.Png.ChunkWriters.Idat
{
    public class PngFilterSub : IPngFilter
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
        }
    }
}