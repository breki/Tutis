using System.Collections.Generic;
using Brejc.Geometry;

namespace GisExperiments.Curves
{
    public class CardinalSplineInterpolator : ICurveInterpolator
    {
        public LineStringInfo InterpolateCurve(IList<IPointF2> points, CurveInterpolationParameters parameters)
        {
            if (points.Count < 2)
                return null;

            if (points.Count == 2)
                return new LineStringInfo(points[0], points[1]);

            LineStringInfo lineStringInfo = new LineStringInfo();
            int i = 0;
            IPointF2 v0;
            IPointF2 vector0;
            IPointF2 v1 = points[i++];
            IPointF2 pom = points[i++];
            IPointF2 vector1 = new PointF2 (pom.X - v1.X, pom.Y - v1.Y);
            IPointF2 p;
            IPointF2 q;
            IPointF2 r;
            IPointF2 s;

            float tension = 1 - parameters.Tension;
            float smoothnessSquare = parameters.Smoothness*parameters.Smoothness;

            for (; i < points.Count; i++)
            {
                v0 = v1;
                vector0 = vector1;
                v1 = pom;
                pom = points[i];
                vector1 = new PointF2 (pom.X - v0.X, pom.Y - v0.Y);

                p = v0;
                q = new PointF2 (v0.X + vector0.X * tension, v0.Y + vector0.Y * tension);
                r = new PointF2 (v1.X - vector1.X * tension, v1.Y - vector1.Y * tension);
                s = v1;

                InterpolateBezierArc (p, q, r, s, lineStringInfo, smoothnessSquare);
            }

            p = v1;
            q = new PointF2 (v1.X + vector1.X * tension, v1.Y + vector1.Y * tension);
            r = new PointF2 ((v1.X - pom.X) * tension + pom.X, (v1.Y - pom.Y) * tension + pom.Y);
            s = pom;

            InterpolateBezierArc (p, q, r, s, lineStringInfo, smoothnessSquare);
            lineStringInfo.AddPoint (s);

            return lineStringInfo;
        }

        private static void InterpolateBezierArc (
            IPointF2 p, 
            IPointF2 q, 
            IPointF2 r, 
            IPointF2 s, 
            LineStringInfo lineStringInfo, 
            double smoothnessSquare)
        {
            double arcLengthSquare = ArcLengthSquare (p, s);
            if (arcLengthSquare <= smoothnessSquare)
                lineStringInfo.AddPoint(p);
            else
            {
                IPointF2 pq = Midpoint (p, q);
                IPointF2 qr = Midpoint (q, r);
                IPointF2 rs = Midpoint (r, s);

                IPointF2 pqr = Midpoint (pq, qr);
                IPointF2 qrs = Midpoint (qr, rs);

                IPointF2 pqrs = Midpoint (pqr, qrs);

                InterpolateBezierArc (p, pq, pqr, pqrs, lineStringInfo, smoothnessSquare);
                InterpolateBezierArc (pqrs, qrs, rs, s, lineStringInfo, smoothnessSquare);
            }
        }

        private static IPointF2 Midpoint (IPointF2 a, IPointF2 b)
        {
            return new PointF2((a.X + b.X)/2, (a.Y + b.Y)/2);
        }

        private static double ArcLengthSquare (IPointF2 p, IPointF2 s)
        {
            return (p.X - s.X) * (p.X - s.X) + (p.Y - s.Y) * (p.Y - s.Y);
        }
    }
}