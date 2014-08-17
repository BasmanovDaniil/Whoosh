using UnityEngine;

namespace ProceduralToolkit
{
    public class Distance
    {
        public static float PointToLine(Vertex2 point, Segment2 segment)
        {
            return PointToLine(point, segment.a, segment.b);
        }

        public static float PointToLine(Vertex2 point, Vertex2 lineStart, Vertex2 lineEnd)
        {
            Vertex2 v = lineEnd - lineStart;
            Vertex2 w = point - lineStart;

            float c1 = Vertex2.Dot(w, v);
            float c2 = Vertex2.Dot(v, v);
            float b = c1/c2;

            Vertex2 pb = lineStart + b * v;
            return Vertex2.Distance(point, pb);
        }

        public static float PointToLineSegment(Vertex2 point, Segment2 segment)
        {
            return PointToLineSegment(point, segment.a, segment.b);
        }

        public static float PointToLineSegment(Vertex2 point, Vertex2 segmentStart, Vertex2 segmentEnd)
        {
            Vertex2 v = segmentEnd - segmentStart;
            Vertex2 w = point - segmentStart;

            float c1 = Vertex2.Dot(w, v);
            if (c1 <= 0)
                return Vertex2.Distance(point, segmentStart);

            float c2 = Vertex2.Dot(v, v);
            if (c2 <= c1)
                return Vertex2.Distance(point, segmentEnd);

            float b = c1/c2;
            Vertex2 pb = segmentStart + b * v;
            return Vertex2.Distance(point, pb);
        }

        public static float LineSegmentToLineSegment(Segment2 S1, Segment2 S2)
        {
            Vertex2 u = S1.b - S1.a;
            Vertex2 v = S2.b - S2.a;
            Vertex2 w = S1.a - S2.a;
            float a = Vertex2.Dot(u, u); // always >= 0
            float b = Vertex2.Dot(u, v);
            float c = Vertex2.Dot(v, v); // always >= 0
            float d = Vertex2.Dot(u, w);
            float e = Vertex2.Dot(v, w);
            float D = a*c - b*b; // always >= 0
            float sN, sD = D; // sc = sN / sD, default sD = D >= 0
            float tN, tD = D; // tc = tN / tD, default tD = D >= 0

            // compute the line parameters of the two closest points
            if (D < Mathf.Epsilon)
            {
                // the lines are almost parallel
                sN = 0; // force using point P0 on segment S1
                sD = 1; // to prevent possible division by 0.0 later
                tN = e;
                tD = c;
            }
            else
            {
                // get the closest points on the infinite lines
                sN = (b*e - c*d);
                tN = (a*e - b*d);
                if (sN < 0)
                {
                    // sc < 0 => the s=0 edge is visible
                    sN = 0;
                    tN = e;
                    tD = c;
                }
                else if (sN > sD)
                {
                    // sc > 1  => the s=1 edge is visible
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if (tN < 0)
            {
                // tc < 0 => the t=0 edge is visible
                tN = 0;
                // recompute sc for this edge
                if (-d < 0)
                    sN = 0;
                else if (-d > a)
                    sN = sD;
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD)
            {
                // tc > 1  => the t=1 edge is visible
                tN = tD;
                // recompute sc for this edge
                if ((-d + b) < 0)
                    sN = 0;
                else if ((-d + b) > a)
                    sN = sD;
                else
                {
                    sN = (-d + b);
                    sD = a;
                }
            }
            // finally do the division to get sc and tc
            float sc = (Mathf.Abs(sN) < Mathf.Epsilon ? 0 : sN/sD);
            float tc = (Mathf.Abs(tN) < Mathf.Epsilon ? 0 : tN/tD);

            // get the difference of the two closest points
            Vertex2 dP = w + (sc * u) - (tc * v); // =  S1(sc) - S2(tc)

            return dP.magnitude; // return the closest distance
        }

        public static float IntLineSegmentToIntLineSegment(IntSegment2 S1, IntSegment2 S2)
        {
            IntVertex2 u = S1.b - S1.a;
            IntVertex2 v = S2.b - S2.a;
            IntVertex2 w = S1.a - S2.a;
            float a = IntVertex2.Dot(u, u); // always >= 0
            float b = IntVertex2.Dot(u, v);
            float c = IntVertex2.Dot(v, v); // always >= 0
            float d = IntVertex2.Dot(u, w);
            float e = IntVertex2.Dot(v, w);
            float D = a*c - b*b; // always >= 0
            float sN, sD = D; // sc = sN / sD, default sD = D >= 0
            float tN, tD = D; // tc = tN / tD, default tD = D >= 0

            // compute the line parameters of the two closest points
            if (D < Mathf.Epsilon)
            {
                // the lines are almost parallel
                sN = 0; // force using point P0 on segment S1
                sD = 1; // to prevent possible division by 0.0 later
                tN = e;
                tD = c;
            }
            else
            {
                // get the closest points on the infinite lines
                sN = (b*e - c*d);
                tN = (a*e - b*d);
                if (sN < 0)
                {
                    // sc < 0 => the s=0 edge is visible
                    sN = 0;
                    tN = e;
                    tD = c;
                }
                else if (sN > sD)
                {
                    // sc > 1  => the s=1 edge is visible
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if (tN < 0)
            {
                // tc < 0 => the t=0 edge is visible
                tN = 0;
                // recompute sc for this edge
                if (-d < 0)
                    sN = 0;
                else if (-d > a)
                    sN = sD;
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD)
            {
                // tc > 1  => the t=1 edge is visible
                tN = tD;
                // recompute sc for this edge
                if ((-d + b) < 0)
                    sN = 0;
                else if ((-d + b) > a)
                    sN = sD;
                else
                {
                    sN = (-d + b);
                    sD = a;
                }
            }
            // finally do the division to get sc and tc
            float sc = (Mathf.Abs(sN) < Mathf.Epsilon ? 0 : sN/sD);
            float tc = (Mathf.Abs(tN) < Mathf.Epsilon ? 0 : tN/tD);

            // get the difference of the two closest points
            IntVertex2 dP = w + (sc*u) - (tc*v); // =  S1(sc) - S2(tc)

            return dP.magnitude; // return the closest distance
        }
    }
}