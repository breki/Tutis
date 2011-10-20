using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Brejc.Common.FileSystem;
using Brejc.Geometry.Rasters;
using Brejc.Imaging;
using Brejc.MapProjections;
using Brejc.Rasters;
using Brejc.SpatialReferencing;
using Karta.Painting;
using Karta.Painting.Gdi;
using MbUnit.Framework;

namespace GisExperiments.CellularAutomata
{
    public class Tests
    {
        [Test]
        public void Test()
        {
            WindowsFileSystem fileSystem = new WindowsFileSystem();
            IBitmapFileManager bitmapFileManager = new GdiBitmapFileManager(new WorldFileHandler(fileSystem), fileSystem);
            ISrsRegistry srsRegistry = new SrsRegistry(new DummyMapProjectionFactory());
            IBitmapRasterFileLoader loader = new BitmapRasterFileLoader(fileSystem, bitmapFileManager, srsRegistry);
            IBitmapRasterFileSaver saver = new BitmapRasterFileSaver(fileSystem, bitmapFileManager, srsRegistry);
            IBitmapRaster originalRaster = (IBitmapRaster)loader.ReadFromFile(@"..\..\..\samples\Rocks.png");
            //IBitmapRaster originalRaster = (IBitmapRaster)loader.ReadFromFile (@"D:\MyStuff\Dropbox\Work\Dusan\Rocks\Rocks.png");
            //IBitmapRaster originalRaster = (IBitmapRaster)loader.ReadFromFile(@"D:\MyStuff\My Dropbox\Work\Dusan\Rocks\Rocks.png");

            fileSystem.DeleteDirectory("Rocks");

            IRaster currentStateRaster = RasterFromBitmap(
                originalRaster,
                (r, x, y, v) =>
                {
                    if (v > 0)
                        r.SetCellValue(x, y, 0x1ff);
                    else
                        r.SetCellValue(x, y, null);
                });
            originalRaster.Dispose();

            IRaster nextStateRaster = new GenericInt16Raster(
                originalRaster.RasterWidth,
                originalRaster.RasterHeight,
                originalRaster.Origin,
                originalRaster.CellWidth,
                originalRaster.CellHeight,
                0);
            nextStateRaster.Srid = currentStateRaster.Srid;
            nextStateRaster.Initialize();

            for (int phase = 0; ; phase++)
            {
                if (phase == 0)
                {
                    IBitmapRaster bitmap = MakeBitmapRaster(currentStateRaster, false);
                    saver.Write(bitmap, @"Rocks\Rocks-" + phase + ".png", new List<string>());
                    bitmap.Dispose();
                }

                bool anyChanges = ProcessRaster(currentStateRaster, nextStateRaster);

                if (!anyChanges || (phase % 10 == 0 && phase > 0))
                {
                    IBitmapRaster bitmap = MakeBitmapRaster(nextStateRaster, !anyChanges);
                    saver.Write(bitmap, @"Rocks\Rocks-" + phase + ".png", new List<string>());
                    bitmap.Dispose();
                }

                if (!anyChanges)
                    break;

                IRaster swap = currentStateRaster;
                currentStateRaster = nextStateRaster;
                nextStateRaster = swap;
            }
        }

        private static IRaster RasterFromBitmap(IBitmapRaster originalRaster, Action<IRaster, int, int, byte> action)
        {
            UnsafeBitmap bm = new UnsafeBitmap(((GdiBitmap)originalRaster.NativeBitmap).Bitmap);
            bm.LockBitmap();

            GenericInt16Raster raster = new GenericInt16Raster(
                originalRaster.RasterWidth,
                originalRaster.RasterHeight,
                originalRaster.Origin,
                originalRaster.CellWidth,
                originalRaster.CellHeight,
                -1);
            raster.Srid = originalRaster.Srid;
            raster.Initialize();

            Parallel.For(
                0,
                raster.RasterHeight,
                y =>
                    {
                        for (int x = 0; x < raster.RasterWidth; x++)
                            action(raster, x, y, bm.GetPixel(x, y).Alpha);
                    });

            bm.UnlockBitmap();

            return raster;
        }

        private static bool ProcessRaster (IRaster currentState, IRaster nextState)
        {
            const int ThresholdValue = 86;

            bool anyChanges = false;
            ForEachCell16(
                currentState,
                (x, y, v) =>
                    {
                        if (v != null)
                        {
                            nextState.SetCellValue(x, y, v);
                            return;
                        }

                        float nv = CalculateNeighboursAvg(currentState, x, y);
                        if (nv > ThresholdValue)
                        {
                            byte value = (byte)Math.Min(255, (int)Math.Round(nv*3.1));
                            //byte value = (byte)Math.Min(255, (int)(nv*nv*0.0178));
                            nextState.SetCellValue (x, y, value);
                            anyChanges = value != 0;
                        }
                        else
                            nextState.SetCellValue(x, y, null);
                    });

            return anyChanges;
        }

        private static IBitmapRaster MakeBitmapRaster(IRaster sourceRaster, bool final)
        {
            UnsafeBitmap bm = new UnsafeBitmap(sourceRaster.RasterWidth, sourceRaster.RasterHeight);

            bm.LockBitmap();
            ForEachCell16(
                sourceRaster,
                (x, y, v) =>
                    {
                        if (v == 0x1ff)
                            bm.SetPixel(x, y, Color.White);
                        else if (v.HasValue)
                        {
                            if (!final)
                                bm.SetPixel(x, y, Color.FromArgb(0xff, 0, v.Value, 0));
                            else
                                bm.SetPixel(x, y, Color.Red);
                        }
                    });
            bm.UnlockBitmap();
            GdiBitmap bitmapWrapper = new GdiBitmap(bm.Bitmap);
            BitmapRaster raster = new BitmapRaster(
                bitmapWrapper,
                sourceRaster.Srid,
                sourceRaster.Origin,
                sourceRaster.CellWidth,
                sourceRaster.CellHeight);

            return raster;
        }

        private static float CalculateNeighboursAvg(IRaster raster, int x, int y)
        {
            if (x == 0 || y == 0 || x == raster.RasterWidth - 1 || y == raster.RasterHeight - 1)
                return 0;

            int total = 0;
            int xa = 0;
            int ya = 0;

            int v;
            v = GetCellValue(raster, x - 1, y - 1);
            total += v;
            xa -= v;
            ya -= v;

            v = GetCellValue(raster, x, y - 1);
            total += v;
            ya -= v;

            v = GetCellValue(raster, x + 1, y - 1);
            total += v;
            xa += v;
            ya -= v;

            v = GetCellValue(raster, x - 1, y);
            total += v;
            xa -= v;

            v = GetCellValue(raster, x + 1, y);
            total += v;
            xa += v;

            v = GetCellValue(raster, x - 1, y + 1);
            total += v;
            xa -= v;
            ya += v;

            v = GetCellValue(raster, x, y + 1);
            total += v;
            ya += v;

            v = GetCellValue(raster, x + 1, y + 1);
            total += v;
            xa += v;
            ya += v;

            float xf = Math.Abs(xa) / (256f * 3);
            float yf = Math.Abs(ya) / (256f * 3);
            float mf = xf + yf + 0.2f;

            return total / mf / 8.0f;
        }

        private static byte GetCellValue(IRaster raster, int x, int y)
        {
            short? v = raster.GetCellValueInt16(x, y);
            if (v.HasValue)
                return (byte)(v.Value & 0xff);

            return 0;
        }

        private static void ForEachCell16(IRaster raster, Action<int, int, short?> action)
        {
            Parallel.For(
                0,
                raster.RasterHeight,
                y =>
                    {
                        for (int x = 0; x < raster.RasterWidth; x++)
                            action(x, y, raster.GetCellValueInt16(x, y));
                    });
        }
    }
}