using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using ZedGraph;

namespace GnuCashUtils.Framework.Reporting
{
    public sealed class ReportGenerationHelper
    {
        static public string GenerateThumbnailImage (GeneratedReport report)
        {
            if (report == null)
                throw new ArgumentNullException ("report");                
            
            Image.GetThumbnailImageAbort myCallback =
                new Image.GetThumbnailImageAbort (ThumbnailCallback);

            using (Image originalImage = new Bitmap (report.ImageFileName))
            {
                using (Image thumbnailImage = originalImage.GetThumbnailImage (200, 150, myCallback, IntPtr.Zero))
                {
                    string thumbnailImageFileName = String.Format (System.Globalization.CultureInfo.InvariantCulture,
                        "{0}_thumb{1}", Path.GetFileNameWithoutExtension (report.ImageFileName), Path.GetExtension (report.ImageFileName));
                    thumbnailImageFileName = Path.Combine (Path.GetDirectoryName (report.ImageFileName), thumbnailImageFileName);

                    thumbnailImage.Save (thumbnailImageFileName);

                    return thumbnailImageFileName;
                }
            }
        }

        [CLSCompliant (false)]
        static public ZedGraphControl CreateGraph ()
        {
            ZedGraphControl zedGraph = new ZedGraphControl ();

            zedGraph.GraphPane.Border.IsVisible = false;
            zedGraph.GraphPane.Fill = new Fill (Color.White, Color.PeachPuff, 45.0f);
            zedGraph.GraphPane.Chart.Fill.Type = FillType.None;
            zedGraph.GraphPane.Title.FontSpec.IsItalic = true;
            //zedGraph.GraphPane.TitleGap = 0;
            zedGraph.GraphPane.Legend.FontSpec.Size = 8;

            zedGraph.GraphPane.YAxis.Scale.Mag = 0;

            return zedGraph;
        }

        static public Color GetColor (int rgb)
        {
            return Color.FromArgb ((int)((uint)0xff000000 | (uint)rgb));
        }

        static public Color GetAdjustedColor (int rgb, double factor)
        {
            int red = (rgb >> 16) & 0xff;
            int green = (rgb >> 8) & 0xff;
            int blue = (rgb >> 0) & 0xff;

            red = Math.Min ((int)(red * factor), 255);
            green = Math.Min ((int)(green * factor), 255);
            blue = Math.Min ((int)(blue * factor), 255);

            uint argb = 0xff000000 | ((uint)red << 16) | ((uint)green << 8) | (uint)blue;

            return Color.FromArgb ((int)argb);
        }

        static public string ConstructTimeframeString (ReportParameters parameters)
        {
            StringBuilder title = new StringBuilder ();
            string space = String.Empty;

            if (parameters.StartTime.HasValue)
            {
                title.AppendFormat (System.Globalization.CultureInfo.InvariantCulture,
                    "from {0}", parameters.StartTime.Value.ToShortDateString());
                space = " ";
            }

            if (parameters.EndTime.HasValue)
                title.AppendFormat (System.Globalization.CultureInfo.InvariantCulture,
                    "{0}till {1}",
                    space,
                    parameters.EndTime.Value.ToShortDateString ());

            return title.ToString ();
        }

        static public DateTime FindSegmentStartTime (DateTime time, ReportTimescale timescale)
        {
            switch (timescale)
            {
                case ReportTimescale.Daily:
                    return time.Date;
                case ReportTimescale.Monthly:
                    return time.AddDays (-(time.Day - 1)).Date;
                case ReportTimescale.Quarterly:
                    if (time.Month < 4)
                        return new DateTime (time.Year, 1, 1);
                    if (time.Month < 7)
                        return new DateTime (time.Year, 4, 1);
                    if (time.Month < 10)
                        return new DateTime (time.Year, 7, 1);
                    return new DateTime (time.Year, 10, 1);
                default:
                    throw new NotSupportedException ();
            }
        }

        static public DateTime FindSegmentEndTime (DateTime segmentStartTime, ReportTimescale timescale)
        {
            switch (timescale)
            {
                case ReportTimescale.Daily:
                    return segmentStartTime.AddDays (1);
                case ReportTimescale.Monthly:
                    return segmentStartTime.AddMonths (1);
                case ReportTimescale.Quarterly:
                    return segmentStartTime.AddMonths (3);
                case ReportTimescale.Weekly:
                    return segmentStartTime.AddDays (7);
                case ReportTimescale.TwoWeekly:
                    return segmentStartTime.AddDays (14);
                case ReportTimescale.Yearly:
                    return segmentStartTime.AddYears (1);

                default:
                    throw new NotSupportedException ();
            }
        }

        public static string GetSegmentName (DateTime segmentStartTime, DateTime segmentEndTime, ReportTimescale reportTimescale)
        {
            string format = String.Empty;
            switch (reportTimescale)
            {
                case ReportTimescale.Daily:
                    format = "{0:m}";
                    break;
                case ReportTimescale.Monthly:
                    format = "{0:y}";
                    break;
                case ReportTimescale.Quarterly:
                    int quarter = (segmentStartTime.Month - 1) / 4 + 1;
                    StringBuilder formatBuilder = new StringBuilder ("Q{0}", quarter);
                    formatBuilder.Append (" {0:yyyy}");
                    format = formatBuilder.ToString();
                    break;
                case ReportTimescale.Weekly:
                case ReportTimescale.TwoWeekly:
                    format = "{0:m} - {1:m}";
                    break;
                case ReportTimescale.Yearly:
                    format = "{0:yyyy}";
                    break;

                default:
                    throw new NotSupportedException ();
            }

            return String.Format (System.Globalization.CultureInfo.CurrentCulture,
                format, segmentStartTime, segmentEndTime);
        }

        static public int GetOrdinalValueForTimescale (DateTime firstSegmentStartTime, DateTime segmentStartTime, 
            ReportTimescale reportTimescale)
        {
            TimeSpan diff = segmentStartTime - firstSegmentStartTime;

            switch (reportTimescale)
            {
                case ReportTimescale.Daily:
                    return (int)diff.TotalDays;
                case ReportTimescale.Monthly:
                    return (segmentStartTime.Year * 12 + segmentStartTime.Month)
                        - (firstSegmentStartTime.Year * 12 + firstSegmentStartTime.Month);
                case ReportTimescale.Quarterly:
                    return ((segmentStartTime.Year * 12 + segmentStartTime.Month)
                        - (firstSegmentStartTime.Year * 12 + firstSegmentStartTime.Month)) / 3;
                case ReportTimescale.TwoWeekly:
                    return (int)(diff.TotalDays / 14);
                case ReportTimescale.Weekly:
                    return (int)(diff.TotalDays / 7);
                case ReportTimescale.Yearly:
                    return segmentStartTime.Year - firstSegmentStartTime.Year;

                default:
                    throw new NotSupportedException ();
            }
        }

        static private bool ThumbnailCallback ()
        {
            return false;
        }

        private ReportGenerationHelper () { }
    }
}
