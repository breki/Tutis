using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using Brejc.Cartography;
using Brejc.Common.FileSystem;
using Brejc.Common.Props;
using Brejc.Geometry;
using Brejc.MapProjections;
using Karta.DataSources.WebMaps;
using Karta.MapProjections.WebMercator;
using SpatialitePlaying.CustomPbf;
using NUnit.Framework;
using SpatialitePlaying.NodeIndexBuilding1.Features;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;
using SpatialitePlaying.NodeIndexBuilding1.RTrees;
using SpatialitePlaying.NodeIndexBuilding1.WaysIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1
{
    public class RTreeTests
    {
        [Test]
        public void StoreNodesInBinaryFile()
        {
            AreaFeatures areaFeatures = new AreaFeatures();
            areaFeatures
                .AddCategory(1, "natural", "wood")
                .AddCategory(1, "landuse", "forest");

            OsmFileProcessor processor = new OsmFileProcessor(areaFeatures, new WindowsFileSystem());

            WindowsFileSystem fileSystem = new WindowsFileSystem ();
            using (OsmPbfReader2 osmReader = new OsmPbfReader2 ())
            {
                //osmReader.Settings.SkipRelations = true;

                //osmReader.Read (@"D:\brisi\isle-of-man-latest.osm.pbf", fileSystem, processor);
                //osmReader.Read (@"D:\brisi\slovenia-latest.osm.pbf", fileSystem, processor);
                osmReader.Read (@"D:\brisi\austria-latest.osm.pbf", fileSystem, processor);
                //osmReader.Read (@"D:\brisi\germany-latest.osm.pbf", fileSystem, processor);
            }
        }

        [Test]
        public void RunQuery()
        {
            IFileSystem fileSystem = new WindowsFileSystem();

            ISpatialQuery spatialQuery = new SpatialQuery (fileSystem);
            Console.WriteLine ("Opening r-tree...");
            spatialQuery.Connect ("experiment", "ways");

            IWaysBTreeIndex waysIdIndex = new WaysBTreeIndex(fileSystem);
            waysIdIndex.Connect("experiment");

            //Bounds2 bounds = new Bounds2 (15.4023050143043, 47.0599307403552, 15.4653307104719, 47.0833220386827);
            //Bounds2 bounds = new Bounds2 (15.5438069890848, 46.5102421428415, 15.6504793872489, 46.5756640758013); // Maribor

            //Console.WriteLine ("Running r-tree query...");
            //Console.WriteLine ("Result: {0} ways", wayIds.Count);

            WebMercatorProjection proj = new WebMercatorProjection(new SimpleUserSettings(), null);
            VirtualMapViewport viewport = new VirtualMapViewport (256, 256);
            proj.AssignToViewport (viewport);
            IMapNavigator navigator = proj.CreateNavigator ();

            Bounds2 tilingBounds 
                = new Bounds2(9.48981294106731,46.3329457738125,17.2092414970457,49.0976261744078); // Austria
                //= new Bounds2 (13.3487707952443, 45.4058476045408, 16.6817730184221, 46.9012278022979); // Slovenia
            int zoomLevel = 12;

            int maxTileCoord = TilingHelper.MaxTileForZoom (zoomLevel);

            // calculate tile coordinates for the first (lower left) tile
            Point2<int> firstTileCoords = TilingHelper.CalculateTileCoordinatesWgs84 (
                tilingBounds.MinX,
                tilingBounds.MaxY,
                zoomLevel);

            Point2<int> lastTileCoords = TilingHelper.CalculateTileCoordinatesWgs84 (
                tilingBounds.MaxX,
                tilingBounds.MinY,
                zoomLevel);

            int tilesGenerated = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            using (Bitmap bitmap = new Bitmap(256, 256))
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                for (int tileY = firstTileCoords.Y; tileY <= lastTileCoords.Y; tileY += 1)
                {
                    for (int tileX = firstTileCoords.X; tileX <= lastTileCoords.X; tileX += 1)
                    {
                        Bounds2 tileBounds = TilingHelper.CalculateTileBoundsWgs84(tileX, tileY, zoomLevel);

                        Mbr queryMbr = new Mbr (tileBounds, 1000);
                        //Console.WriteLine ("Running r-tree query...");
                        List<long> wayIds = spatialQuery.FindObjects (queryMbr);
                        wayIds.Sort ((a, b) => a.CompareTo (b));
                        IDictionary<long, WayData> waysData = waysIdIndex.FetchWays (wayIds);

                        gfx.Clear (Color.Wheat);

                        navigator.ZoomToArea (tileBounds, 1);

                        foreach (WayData way in waysData.Values)
                        {
                            IPointD2List points = way.GetPointsList ();
                            IPointF2List vpoints = proj.Project (points);

                            PointF[] gdiPoints = ConvertToGdiPointsF (vpoints);
                            gfx.FillPolygon (Brushes.DarkSeaGreen, gdiPoints);
                        }

                        string fileName = ConstructTileFileName(zoomLevel, tileX, tileY);
                        fileSystem.EnsureDirectoryExists(Path.GetDirectoryName(fileName));
                        bitmap.Save (fileName, ImageFormat.Png);

                        tilesGenerated++;
                        if (tilesGenerated%100 == 0)
                            Debug.WriteLine("Generated {0} tiles ({1} t/s)", tilesGenerated, tilesGenerated / sw.Elapsed.TotalSeconds);
                    }
                }
            }
        }

        private static PointF[] ConvertToGdiPointsF (IPointF2List points)
        {
            PointF[] gdiPoints = new PointF[points.PointsCount];

            points.ForEachPoint ((i, x, y) => gdiPoints[i] = new PointF (x, y));

            return gdiPoints;
        }

        private static string ConstructTileFileName (int zoomLevel, int tileX, int tileY)
        {
            return string.Format (
                CultureInfo.InvariantCulture,
                @"tiles{3}{0}{3}{1}{3}{2}.png",
                zoomLevel,
                tileX,
                tileY,
                Path.DirectorySeparatorChar);
        }

    }
}
