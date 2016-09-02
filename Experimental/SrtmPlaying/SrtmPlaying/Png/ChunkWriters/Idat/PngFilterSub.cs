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
                        byte blue = scanline.NextByte();
                        if (xx > 0)
                            blue -= scanline[si - pngInfo.SourcePixelSize - 1];
                        byte green = scanline.NextByte();
                        if (xx > 0)
                            green -= scanline[si - pngInfo.SourcePixelSize - 1];
                        byte red = scanline.NextByte();
                        if (xx > 0)
                            red -= scanline[si - pngInfo.SourcePixelSize - 1];

                        filtered[fi++] = red;
                        filtered[fi++] = green;
                        filtered[fi++] = blue;

                        if (scanline.HasAlpha)
                        {
                            byte alpha = scanline.NextByte();

                            if (useAlpha)
                            {
                                if (xx > 0)
                                    alpha -= scanline[si - pngInfo.SourcePixelSize - 1];

                                filtered[fi++] = alpha;
                            }
                        }

                        break;

                    case PngImageType.Grayscale16:
                        byte hi = scanline.NextByte();
                        if (xx > 0)
                            hi -= scanline[si - pngInfo.SourcePixelSize - 1];
                        byte lo = scanline.NextByte();
                        if (xx > 0)
                            lo -= scanline[si - pngInfo.SourcePixelSize - 1];

                        filtered[fi++] = hi;
                        filtered[fi++] = lo;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            throw new NotImplementedException();
        }
    }
}