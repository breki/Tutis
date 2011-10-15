using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Brejc.Geometry;
using GisExperiments.Curves;
using Karta.Painting;
using Karta.Painting.Gdi;
using MbUnit.Framework;

namespace GisExperiments
{
    public class SplineTests
    {
        [Test]
        public void CompareWithDrawCurve()
        {
            Render(
                "CardinalSpline.png",
                g =>
                    {
                        PointF[] points = new[] { new PointF (100, 500), new PointF (500, 400), new PointF (900, 700) };

                        for (double tension = 0; tension <= 1; tension += 0.1)
                        {
                            Pen pen = new Pen (Color.FromArgb (150, Color.Blue), 1);
                            g.DrawCardinalSpline (pen, points, tension, 1);
                            pen = new Pen (Color.FromArgb (150, Color.Red), 1);
                            g.DrawCurve (pen, points, (float)tension);
                        }

                        Pen pen2 = new Pen (Color.Red, 1);
                        foreach (PointF point in points)
                            g.DrawRectangle (pen2, point.X - 3, point.Y - 3, 7, 7);
                        
                    });
        }

        [Test]
        public void DrawSplineNicely()
        {
            Render (
                "CardinalSplineNice.png",
                g =>
                {
                    PointF[] points = new[] { new PointF (100, 500), new PointF (500, 400), new PointF (900, 700) };

                    double tension = 0.85;
                    Pen pen = new Pen (Color.FromArgb (150, Color.Blue), 1);
                    g.DrawCardinalSpline (pen, points, tension, 1);

                    Pen pen2 = new Pen (Color.Red, 1);
                    foreach (PointF point in points)
                        g.DrawRectangle (pen2, point.X - 3, point.Y - 3, 7, 7);
                });
        }

        [Test]
        public void InterpolateCardinalSpline()
        {
            Render (
                "CardinalSplineInterpolation.png",
                g =>
                {
                    IPointF2[] points = new IPointF2[] { new PointF2 (100, 500), new PointF2 (500, 400), new PointF2 (900, 700) };

                    float tension = 0.85f;
                    CurveInterpolationParameters p = new CurveInterpolationParameters(tension, 10);
                    CardinalSplineInterpolator interpolator = new CardinalSplineInterpolator();
                    LineStringInfo lineStringInfo = interpolator.InterpolateCurve(points, p);

                    //Assert.AreEqual(3, lineStringInfo.Points.Count);

                    Pen linePen = new Pen (Color.Blue, 1);
                    Pen pointPen = new Pen (Color.Red, 1);

                    for (int i = 0; i < lineStringInfo.Points.Count; i++)
                    {
                        IPointF2 point = lineStringInfo.Points[i];
                        if (i < lineStringInfo.Points.Count - 1)
                        {
                            IPointF2 point2 = lineStringInfo.Points[i+1];
                            g.DrawLine (linePen, new PointF (point.X, point.Y), new PointF (point2.X, point2.Y));
                        }

                        g.DrawRectangle (pointPen, point.X - 3, point.Y - 3, 7, 7);
                    }
                });
            
        }

        [Test]
        public void TextOnCurve ()
        {
            Render (
                "TextOnCurve.png",
                g =>
                {
                    IPointF2[] points = new IPointF2[] { new PointF2 (100, 500), new PointF2 (500, 400), new PointF2 (900, 700) };

                    float tension = 0.85f;
                    CurveInterpolationParameters p = new CurveInterpolationParameters (tension, 30);
                    CardinalSplineInterpolator interpolator = new CardinalSplineInterpolator ();
                    LineStringInfo lineStringInfo = interpolator.InterpolateCurve (points, p);

                    Pen linePen = new Pen (Color.Blue, 1);

                    for (int i = 0; i < lineStringInfo.Points.Count-1; i++)
                    {
                        IPointF2 point = lineStringInfo.Points[i];
                        IPointF2 point2 = lineStringInfo.Points[i + 1];
                        g.DrawLine (linePen, new PointF (point.X, point.Y), new PointF (point2.X, point2.Y));
                    }

                    float[] coords = new float[lineStringInfo.Points.Count * 2];
                    for (int i = 0; i < lineStringInfo.Points.Count; i++)
                    {
                        coords[i*2] = lineStringInfo.Points[i].X;
                        coords[i*2+1] = lineStringInfo.Points[i].Y;
                    }

                    GraphicPolylineAnalysis pa = new GraphicPolylineAnalysis(coords);
                    pa.Analyze();

                    string text = "MEDITERRANEAN SEA";

                    Font font = new Font ("Times New Roman", 40, FontStyle.Italic, GraphicsUnit.Pixel);

                    IKerningDatabase kerningDatabase = new KerningDatabase();
                    FastGdiTextMeasurer textMeasurer = new FastGdiTextMeasurer(kerningDatabase);
                    PaintStyling style = new PaintStyling();
                    style.Set(StylingNames.FontFamily, font.FontFamily.Name);
                    style.Set(StylingNames.FontSize, font.Size);
                    style.Set(StylingNames.FontItalic, true);
                    textMeasurer.SetTypeface(style);
                    float textWidth = textMeasurer.MeasureTextWidth(text);
                    float[] charWidths = textMeasurer.MeasureTextCharWidths(text);
                    char[] chars = text.ToCharArray();

                    Brush brush = new SolidBrush(Color.Black);
                    StringFormat stringFormat = new StringFormat(StringFormat.GenericDefault);
                    stringFormat.LineAlignment = StringAlignment.Center;
                    stringFormat.Alignment = StringAlignment.Center;

                    pa.MoveTo(0);
                    for (int i = 0; i < text.Length; i++)
                    {
                        float angle = pa.CurrentAngle;
                        
                        //IPointF2 point = lineStringInfo.CalculatePointAtLength(position, out angle);

                        foreach (TextRenderingHint hint in Enum.GetValues (typeof (TextRenderingHint)))
                        {
                            g.TextRenderingHint = hint;

                            g.ResetTransform ();
                            g.TranslateTransform(pa.CurrentPoint.X, pa.CurrentPoint.Y + ((int)hint) * 50);

                            if (angle != 0)
                                g.RotateTransform(angle);

                            g.DrawString(chars[i].ToString(), font, brush, 0, 0, stringFormat);
                        }

                        pa.MoveBy (charWidths[i] + 10);
                    }
                });
        }

        private static void Render(string fileName, Action<Graphics> renderAction)
        {
            using (Bitmap b = new Bitmap(1000, 1000))
            using (Graphics g = Graphics.FromImage(b))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.Clear(Color.White);

                renderAction(g);

                b.Save (fileName);
            }
        }
    }
}