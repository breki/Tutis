using System;
using System.Drawing;
using System.Threading.Tasks;
using Brejc.Imaging;

namespace GisExperiments.Resampling
{
    public interface IBitmapResamplingAlgorithm
    {
        Bitmap ReduceBitmapSize (Bitmap bitmap, int reductionFactor);
    }

    public class SuperSampling : IBitmapResamplingAlgorithm
    {
        public Bitmap ReduceBitmapSize (Bitmap bitmap, int reductionFactor)
        {
            UnsafeBitmap source = new UnsafeBitmap(bitmap);
            source.LockBitmap();

            int width = (int)Math.Floor(source.Bitmap.Width/(double)reductionFactor);
            int height = (int)Math.Floor (source.Bitmap.Height / (double)reductionFactor);
            UnsafeBitmap dest = new UnsafeBitmap(width, height);
            dest.LockBitmap();

            // split processing into batches of rows
            int parallels = Environment.ProcessorCount;
            int rowsPerFrame = height/parallels;
            rowsPerFrame = Math.Max(4, rowsPerFrame);
            int totalFrames = (int)Math.Ceiling(((double)height)/rowsPerFrame);

            Parallel.For (0, totalFrames, frame =>
            {
                int maxY = Math.Min (height, (frame + 1) * rowsPerFrame);

                for (int dstY = frame * rowsPerFrame; dstY < maxY; dstY++)
                    ProcessRow (source, dest, reductionFactor, width, dstY);
            });

            dest.UnlockBitmap();
            source.UnlockBitmap();
            return dest.Bitmap;
        }

        private static void ProcessRow(UnsafeBitmap source, UnsafeBitmap dest, int reductionFactor, int width, int dstY)
        {
            double srcTop = dstY*reductionFactor;
            double srcTopFloor = Math.Floor(srcTop);
            double srcTopWeight = 1 - (srcTop - srcTopFloor);
            int srcTopInt = (int) srcTopFloor;

            double srcBottom = (dstY + 1)*reductionFactor;
            double srcBottomFloor = Math.Floor(srcBottom - 0.00001);
            double srcBottomWeight = srcBottom - srcBottomFloor;
            int srcBottomInt = (int) srcBottomFloor;

            for (int dstX = 0; dstX < width; dstX++)
            {
                double srcLeft = dstX*reductionFactor;
                double srcLeftFloor = Math.Floor(srcLeft);
                double srcLeftWeight = 1 - (srcLeft - srcLeftFloor);
                int srcLeftInt = (int) srcLeftFloor;

                double srcRight = (dstX + 1)*reductionFactor;
                double srcRightFloor = Math.Floor(srcRight - 0.00001);
                double srcRightWeight = srcRight - srcRightFloor;
                int srcRightInt = (int) srcRightFloor;

                double blueSum = 0;
                double greenSum = 0;
                double redSum = 0;
                double alphaSum = 0;

                PixelData pixel;
                // left fractional edge
                for (int srcY = srcTopInt + 1; srcY < srcBottomInt; ++srcY)
                {
                    pixel = source.GetPixel(srcLeftInt, srcY);

                    double a = pixel.Alpha;
                    blueSum += pixel.Blue*srcLeftWeight*a;
                    greenSum += pixel.Green*srcLeftWeight*a;
                    redSum += pixel.Red*srcLeftWeight*a;
                    alphaSum += pixel.Alpha*srcLeftWeight;
                }

                // right fractional edge
                for (int srcY = srcTopInt + 1; srcY < srcBottomInt; ++srcY)
                {
                    try
                    {
                        pixel = source.GetPixel(srcRightInt, srcY);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    double a = pixel.Alpha;
                    blueSum += pixel.Blue*srcRightWeight*a;
                    greenSum += pixel.Green*srcRightWeight*a;
                    redSum += pixel.Red*srcRightWeight*a;
                    alphaSum += pixel.Alpha*srcRightWeight;
                }

                // top fractional edge
                for (int srcX = srcLeftInt + 1; srcX < srcRightInt; ++srcX)
                {
                    pixel = source.GetPixel(srcX, srcTopInt);

                    double a = pixel.Alpha;
                    blueSum += pixel.Blue*srcTopWeight*a;
                    greenSum += pixel.Green*srcTopWeight*a;
                    redSum += pixel.Red*srcTopWeight*a;
                    alphaSum += pixel.Alpha*srcTopWeight;
                }

                // bottom fractional edge
                for (int srcX = srcLeftInt + 1; srcX < srcRightInt; ++srcX)
                {
                    pixel = source.GetPixel(srcX, srcBottomInt);

                    double a = pixel.Alpha;
                    blueSum += pixel.Blue*srcBottomWeight*a;
                    greenSum += pixel.Green*srcBottomWeight*a;
                    redSum += pixel.Red*srcBottomWeight*a;
                    alphaSum += pixel.Alpha*srcBottomWeight;
                }

                // center area
                for (int srcY = srcTopInt + 1; srcY < srcBottomInt; ++srcY)
                {
                    for (int srcX = srcLeftInt + 1; srcX < srcRightInt; ++srcX)
                    {
                        pixel = source.GetPixel(srcX, srcY);

                        double a = pixel.Alpha;
                        blueSum += pixel.Blue*a;
                        greenSum += pixel.Green*a;
                        redSum += pixel.Red*a;
                        alphaSum += pixel.Alpha;
                    }
                }

                // four corner pixels
                pixel = source.GetPixel(srcLeftInt, srcTopInt);

                double srcTLA = pixel.Alpha;
                blueSum += pixel.Blue*(srcTopWeight*srcLeftWeight)*srcTLA;
                greenSum += pixel.Green*(srcTopWeight*srcLeftWeight)*srcTLA;
                redSum += pixel.Red*(srcTopWeight*srcLeftWeight)*srcTLA;
                alphaSum += pixel.Alpha*(srcTopWeight*srcLeftWeight);

                pixel = source.GetPixel(srcRightInt, srcTopInt);
                double srcTRA = pixel.Alpha;
                blueSum += pixel.Blue*(srcTopWeight*srcRightWeight)*srcTRA;
                greenSum += pixel.Green*(srcTopWeight*srcRightWeight)*srcTRA;
                redSum += pixel.Red*(srcTopWeight*srcRightWeight)*srcTRA;
                alphaSum += pixel.Alpha*(srcTopWeight*srcRightWeight);

                pixel = source.GetPixel(srcLeftInt, srcBottomInt);
                double srcBLA = pixel.Alpha;
                blueSum += pixel.Blue*(srcBottomWeight*srcLeftWeight)*srcBLA;
                greenSum += pixel.Green*(srcBottomWeight*srcLeftWeight)*srcBLA;
                redSum += pixel.Red*(srcBottomWeight*srcLeftWeight)*srcBLA;
                alphaSum += pixel.Alpha*(srcBottomWeight*srcLeftWeight);

                pixel = source.GetPixel(srcRightInt, srcBottomInt);
                double srcBRA = pixel.Alpha;
                blueSum += pixel.Blue*(srcBottomWeight*srcRightWeight)*srcBRA;
                greenSum += pixel.Green*(srcBottomWeight*srcRightWeight)*srcBRA;
                redSum += pixel.Red*(srcBottomWeight*srcRightWeight)*srcBRA;
                alphaSum += pixel.Alpha*(srcBottomWeight*srcRightWeight);

                double area = (srcRight - srcLeft)*(srcBottom - srcTop);

                double alpha = alphaSum/area;
                double blue;
                double green;
                double red;

                if (alpha == 0)
                {
                    blue = 0;
                    green = 0;
                    red = 0;
                }
                else
                {
                    blue = blueSum/alphaSum;
                    green = greenSum/alphaSum;
                    red = redSum/alphaSum;
                }

                // add 0.5 so that rounding goes in the direction we want it to
                blue += 0.5;
                green += 0.5;
                red += 0.5;
                alpha += 0.5;

                dest.SetPixel(dstX, dstY, Color.FromArgb((int) alpha, (int) red, (int) green, (int) blue));
            }
        }
    }
}