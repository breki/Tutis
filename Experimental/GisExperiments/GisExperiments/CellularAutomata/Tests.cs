using System;
using System.Collections.Generic;
using System.Drawing;
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
            //IBitmapRaster originalRaster = (IBitmapRaster)loader.ReadFromFile (@"D:\hg\tutis\Experimental\GisExperiments\samples\Rocks.png");
            IBitmapRaster originalRaster = (IBitmapRaster)loader.ReadFromFile (@"D:\MyStuff\Dropbox\Work\Dusan\Rocks\Rocks.png");

            fileSystem.DeleteDirectory("Rocks");

            // first mark all starting pixels with full value
            Color fullValue = Color.White;
            ForEachCell(
                originalRaster, 
                (x, y, v) =>
                {
                    Color c = Color.FromArgb (v.Value);
                    if (c.A > 0)
                        originalRaster.SetCellValue (x, y, fullValue.ToArgb ());
                });

            int phase = 0;
            saver.Write(originalRaster, @"Rocks\Rocks-" + phase + ".png", new List<string>());

            IRaster previousRaster = originalRaster; 
            for (phase++; ; phase++)
            {
                IRaster processingRaster = CloneRaster(previousRaster);
                bool anyChanges = ProcessRaster (previousRaster, processingRaster);
                saver.Write (processingRaster, @"Rocks\Rocks-" + phase + ".png", new List<string> ());

                if (!anyChanges)
                    break;

                previousRaster = processingRaster;
            }
        }

        private static IRaster CloneRaster(IRaster sourceRaster)
        {
            Bitmap bitmap = new Bitmap(sourceRaster.RasterWidth, sourceRaster.RasterHeight);
            GdiBitmap bitmapWrapper = new GdiBitmap(bitmap);
            BitmapRaster raster = new BitmapRaster(
                bitmapWrapper, 
                sourceRaster.Srid,
                sourceRaster.Origin,
                sourceRaster.CellWidth,
                sourceRaster.CellHeight);

            ForEachCell(sourceRaster, raster.SetCellValue);

            return raster;
        }

        private static bool ProcessRaster (IRaster previousRaster, IRaster workingRaster)
        {
            const int ThresholdValue = 90;

            bool anyChanges = false;
            ForEachCell(
                previousRaster,
                (x, y, v) =>
                    {
                        Color cv = Color.FromArgb(v.Value);

                        if (cv.A != 0)
                            return;

                        float nv = CalculateNeighboursAvg(previousRaster, x, y);
                        if (nv > ThresholdValue)
                        {
                            int value = Math.Min(255, (int)(nv*1.7));

                            Color c = Color.FromArgb(0xff, value, 100, 255);
                            workingRaster.SetCellValue (x, y, c.ToArgb ());
                            anyChanges = true;
                        }
                    });

            return anyChanges;
        }

        private static float CalculateNeighboursAvg(IRaster raster, int x, int y)
        {
            if (x == 0 || y == 0 || x == raster.RasterWidth - 1 || y == raster.RasterHeight - 1)
                return 0;

            int total = 0;
            total += GetCellValue(raster, x - 1, y - 1);
            total += GetCellValue (raster, x, y - 1);
            total += GetCellValue (raster, x + 1, y - 1);
            total += GetCellValue (raster, x - 1, y);
            total += GetCellValue (raster, x + 1, y);
            total += GetCellValue (raster, x - 1, y + 1);
            total += GetCellValue (raster, x, y + 1);
            total += GetCellValue (raster, x + 1, y + 1);
            return total/8.0f;
        }

        private static int GetCellValue(IRaster raster, int x, int y)
        {
            int argb = raster.GetCellValueInt32(x, y).Value;
            return Color.FromArgb(argb).R;
        }

        private static void ForEachCell (IRaster raster, Action<int, int, int?> action)
        {
            for (int y = 0; y < raster.RasterHeight; y++)
                for (int x = 0; x < raster.RasterWidth; x++)
                    action(x, y, raster.GetCellValueInt32(x, y));
        }
    }
}