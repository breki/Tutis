using System;
using System.Collections.Generic;
using System.Drawing;

namespace GisExperiments
{
    public static class Spline
    {
        public static void DrawCardinalSpline (this Graphics g, Pen pen, IList<PointF> points, double a, double smoothness)
        {
            if (points.Count < 3)
                return;

            int i = 0;
            PointF v0;
            PointF vector0;
            PointF v1 = points[i++];
            PointF pom = points[i++];
            PointF vector1 = new PointF (pom.X - v1.X, pom.Y - v1.Y);
            PointF p;
            PointF q;
            PointF r;
            PointF s;

            // Color red = new Color(154, 40, 30);

            for (; i < points.Count; i++)
            {
                v0 = v1;
                vector0 = vector1;
                v1 = pom;
                pom = points[i];
                vector1 = new PointF (pom.X - v0.X, pom.Y - v0.Y);

                p = v0;
                q = new PointF ((float)(v0.X + vector0.X * (1 - a)), (float)(v0.Y + vector0.Y * (1 - a)));
                r = new PointF ((float)(v1.X - vector1.X * (1 - a)), (float)(v1.Y - vector1.Y * (1 - a)));
                s = v1;

                g.DrawBezierArc (pen, p, q, r, s, smoothness);
            }

            p = v1;
            q = new PointF ((float)(v1.X + vector1.X * (1 - a)), (float)(v1.Y + vector1.Y * (1 - a)));
            r = new PointF ((float)((v1.X - pom.X) * (1 - a) + pom.X), (float)((v1.Y - pom.Y) * (1 - a) + pom.Y));
            s = pom;

            g.DrawBezierArc (pen, p, q, r, s, smoothness);
        }

        private static void DrawBezierArc (this Graphics g, Pen pen, PointF p, PointF q, PointF r, PointF s, double smoothness)
        {
            if (ArcSize (p, s) <= smoothness)
                g.DrawLine(pen, p, s);
                //g.DrawRectangle(pen, p.X, p.Y, 1, 1);
            else
            {
                PointF pq = Midpoint (p, q);
                PointF qr = Midpoint (q, r);
                PointF rs = Midpoint (r, s);

                PointF pqr = Midpoint (pq, qr);
                PointF qrs = Midpoint (qr, rs);

                PointF pqrs = Midpoint (pqr, qrs);

                DrawBezierArc (g, pen, p, pq, pqr, pqrs, smoothness);
                DrawBezierArc (g, pen, pqrs, qrs, rs, s, smoothness);
            }
        }

        private static PointF Midpoint (PointF a, PointF b)
        {
            return new PointF((a.X + b.X)/2, (a.Y + b.Y)/2);
        }

        private static double ArcSize (PointF p, PointF s)
        {
            return Math.Sqrt((p.X - s.X) * (p.X - s.X) + (p.Y - s.Y) * (p.Y - s.Y));
        }
    }
}