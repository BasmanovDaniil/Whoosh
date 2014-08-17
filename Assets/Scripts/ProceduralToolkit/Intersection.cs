using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralToolkit
{
    public enum IntersectionType
    {
        None,
        Point,
        Line,
        Ray,
        Segment,
        Polyline
    }

    public class Intersection
    {
        #region SegmentToRay overloads

        public static bool SegmentToRay(Segment2 segment, Ray2D ray)
        {
            Vertex2 intersection;
            return SegmentToRay(segment.a, segment.b, ray.origin, ray.origin + ray.direction, out intersection);
        }

        public static bool SegmentToRay(Segment2 segment, Vertex2 rayStart, Vertex2 rayEnd)
        {
            Vertex2 intersection;
            return SegmentToRay(segment.a, segment.b, rayStart, rayEnd, out intersection);
        }

        public static bool SegmentToRay(Segment2 segment, Ray2D ray, out Vertex2 intersection)
        {
            return SegmentToRay(segment.a, segment.b, new Vertex2(ray.origin), new Vertex2(ray.origin + ray.direction),
                out intersection);
        }

        public static bool SegmentToRay(Segment2 segment, Vertex2 rayStart, Vertex2 rayEnd, out Vertex2 intersection)
        {
            return SegmentToRay(segment.a, segment.b, rayStart, rayEnd, out intersection);
        }

        #endregion SegmentToRay overloads

        public static bool SegmentToRay(Vertex2 segmentStart, Vertex2 segmentEnd, Vertex2 rayStart, Vertex2 rayEnd,
            out Vertex2 intersection)
        {
            intersection = Vertex2.zero;
            float r, s;

            if (LineToLine(rayStart, rayEnd, segmentStart, segmentEnd, out r, out s))
            {
                if (r >= 0)
                {
                    if (s >= 0 && s <= 1)
                    {
                        intersection = rayStart + (rayEnd - rayStart)*r;
                        return true;
                    }
                }
            }

            return false;
        }

        #region IntSegmentToRay overloads

        public static bool IntSegmentToRay(IntSegment2 segment, Ray2D ray)
        {
            IntVertex2 intersection;
            return IntSegmentToRay(segment.a, segment.b, ray, out intersection);
        }

        public static bool IntSegmentToRay(IntVertex2 v0, IntVertex2 v1, Ray2D ray)
        {
            IntVertex2 intersection;
            return IntSegmentToRay(v0, v1, ray.origin, ray.origin + ray.direction, out intersection);
        }

        public static bool IntSegmentToRay(IntSegment2 segment, Ray2D ray, out IntVertex2 intersection)
        {
            return IntSegmentToRay(segment.a, segment.b, ray, out intersection);
        }

        public static bool IntSegmentToRay(IntVertex2 v0, IntVertex2 v1, Ray2D ray, out IntVertex2 intersection)
        {
            return IntSegmentToRay(v0, v1, ray.origin, ray.origin + ray.direction, out intersection);
        }

        #endregion IntSegmentToRay overloads

        public static bool IntSegmentToRay(IntVertex2 segmentStart, IntVertex2 segmentEnd, Vertex2 rayStart,
            Vertex2 rayEnd, out IntVertex2 intersection)
        {
            intersection = IntVertex2.zero;
            float r, s;

            if (LineToLine(rayStart, rayEnd, (Vertex2) segmentStart, (Vertex2) segmentEnd, out r, out s))
            {
                if (r >= 0)
                {
                    if (s >= 0 && s <= 1)
                    {
                        var inter = rayStart + (rayEnd - rayStart)*r;
                        intersection = new IntVertex2(inter.x, inter.y);
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool LineToLine(Vertex2 line1Start, Vertex2 line1End, Vertex2 line2Start, Vertex2 line2End,
            out Vertex2 intersection)
        {
            intersection = Vertex2.zero;
            float r, s;

            if (LineToLine(line1Start, line1End, line2Start, line2End, out r, out s))
            {
                intersection = line1Start + (line1End - line1Start)*r;
                return true;
            }
            return false;
        }

        private static bool LineToLine(Vertex2 vertex1, Vertex2 vertex2, Vertex2 vertex3, Vertex2 vertex4, out float r,
            out float s)
        {
            r = 0;
            s = 0;
            //Make sure the lines aren't parallel
            Vertex2 line1 = vertex2 - vertex1;
            Vertex2 line2 = vertex4 - vertex3;

            //if (vertex1to2.x * -vertex3to4.y + vertex1to2.y * vertex3to4.x != 0)
            //{
            //if (line1.y/line1.x != line2.y/line2.x)
            //{

            //}

            float d = PGUtils.PerpDot(line1, line2);

            if (d != 0)
            {
                Vertex2 vertex3to1 = vertex1 - vertex3;
                r = (vertex3to1.y*line2.x - vertex3to1.x*line2.y)/d;
                s = (vertex3to1.y*line1.x - vertex3to1.x*line1.y)/d;
                return true;
            }
            else
            {
                //Parallel
            }

            return false;
        }

        public static bool SegmentToPoint(Segment2 segment, Vertex2 point)
        {
            return SegmentToPoint(segment.a, segment.b, point);
        }

        public static bool SegmentToPoint(Vertex2 segmentStart, Vertex2 segmentEnd, Vertex2 point)
        {
            if (segmentStart.x != segmentEnd.x)
            {
                // S is not  vertical
                if (segmentStart.x <= point.x && point.x <= segmentEnd.x)
                    return true;
                if (segmentStart.x >= point.x && point.x >= segmentEnd.x)
                    return true;
            }
            else
            {
                // S is vertical, so test y  coordinate
                if (segmentStart.y <= point.y && point.y <= segmentEnd.y)
                    return true;
                if (segmentStart.y >= point.y && point.y >= segmentEnd.y)
                    return true;
            }
            return false;
        }

        #region SegmentToSegment overloads

        public static bool SegmentToSegment(Segment2 segment0, Segment2 segment1, bool testBoundingBox = false)
        {
            Segment2 intersection;
            return SegmentToSegmentE(segment0.a, segment0.b, segment1.a, segment1.b, out intersection,
                testBoundingBox) != IntersectionType.None;
        }

        public static bool SegmentToSegment(Vertex2 start0, Vertex2 end0, Vertex2 start1, Vertex2 end1,
            bool testBoundingBox = false)
        {
            Segment2 intersection;
            return SegmentToSegmentE(start0.x, start0.y, end0.x, end0.y, start1.x, start1.y, end1.x, end1.y,
                out intersection, testBoundingBox) != IntersectionType.None;
        }

        public static bool SegmentToSegment(float start0x, float start0y, float end0x, float end0y, float start1x,
            float start1y, float end1x, float end1y, bool testBoundingBox = false)
        {
            Segment2 intersection;
            return SegmentToSegmentE(start0x, start0y, end0x, end0y, start1x, start1y, end1x, end1y,
                out intersection, testBoundingBox) != IntersectionType.None;
        }

        public static bool SegmentToSegment(Segment2 segment0, Segment2 segment1,
            out Segment2 intersection, bool testBoundingBox = false)
        {
            return SegmentToSegmentE(segment0.a, segment0.b, segment1.a, segment1.b, out intersection,
                testBoundingBox) != IntersectionType.None;
        }

        public static bool SegmentToSegment(Vertex2 start0, Vertex2 end0, Vertex2 start1, Vertex2 end1,
            out Segment2 intersection, bool testBoundingBox = false)
        {
            return SegmentToSegmentE(start0.x, start0.y, end0.x, end0.y, start1.x, start1.y, end1.x, end1.y,
                out intersection, testBoundingBox) != IntersectionType.None;
        }

        public static bool SegmentToSegment(float start0x, float start0y, float end0x, float end0y, float start1x,
            float start1y, float end1x, float end1y, out Segment2 intersection, bool testBoundingBox = false)
        {
            return SegmentToSegmentE(start0x, start0y, end0x, end0y, start1x, start1y, end1x, end1y,
                out intersection, testBoundingBox) != IntersectionType.None;
        }

        public static IntersectionType SegmentToSegmentE(Segment2 segment0, Segment2 segment1,
            bool testBoundingBox = false)
        {
            Segment2 intersection;
            return SegmentToSegmentE(segment0.a, segment0.b, segment1.a, segment1.b, out intersection,
                testBoundingBox);
        }

        public static IntersectionType SegmentToSegmentE(Vertex2 start0, Vertex2 end0, Vertex2 start1,
            Vertex2 end1, bool testBoundingBox = false)
        {
            Segment2 intersection;
            return SegmentToSegmentE(start0.x, start0.y, end0.x, end0.y, start1.x, start1.y, end1.x, end1.y,
                out intersection, testBoundingBox);
        }

        public static IntersectionType SegmentToSegmentE(float start0x, float start0y, float end0x, float end0y,
            float start1x, float start1y, float end1x, float end1y, bool testBoundingBox = false)
        {
            Segment2 intersection;
            return SegmentToSegmentE(start0x, start0y, end0x, end0y, start1x, start1y, end1x, end1y,
                out intersection, testBoundingBox);
        }

        public static IntersectionType SegmentToSegmentE(Segment2 segment0, Segment2 segment1,
            out Segment2 intersection, bool testBoundingBox = false)
        {
            return SegmentToSegmentE(segment0.a, segment0.b, segment1.a, segment1.b, out intersection,
                testBoundingBox);
        }

        public static IntersectionType SegmentToSegmentE(Vertex2 start0, Vertex2 end0, Vertex2 start1,
            Vertex2 end1, out Segment2 intersection, bool testBoundingBox = false)
        {
            return SegmentToSegmentE(start0.x, start0.y, end0.x, end0.y, start1.x, start1.y, end1.x, end1.y,
                out intersection, testBoundingBox);
        }

        #endregion SegmentToSegment overloads

        /// <summary>
        /// Based on "Faster Line Segment Intersection" by Franklin Antonio, Graphics Gems III
        /// </summary>
        public static IntersectionType SegmentToSegmentE(float start0x, float start0y, float end0x, float end0y,
            float start1x, float start1y, float end1x, float end1y, out Segment2 intersection,
            bool testBoundingBox = false)
        {
            intersection = new Segment2();
            float ax = end0x - start0x;
            float ay = end0y - start0y;
            float bx = start1x - end1x;
            float by = start1y - end1y;

            if (testBoundingBox)
            {
                // X bound box test
                float x1hi = end0x;
                float x1lo = start0x;
                if (ax < 0)
                {
                    x1lo = end0x;
                    x1hi = start0x;
                }
                if (bx > 0)
                {
                    if (x1hi < end1x || start1x < x1lo) return IntersectionType.None;
                }
                else
                {
                    if (x1hi < start1x || end1x < x1lo) return IntersectionType.None;
                }

                // Y bound box test
                float y1hi = end0y;
                float y1lo = start0y;
                if (ay < 0)
                {
                    y1lo = end0y;
                    y1hi = start0y;
                }
                if (by > 0)
                {
                    if (y1hi < end1y || start1y < y1lo) return IntersectionType.None;
                }
                else
                {
                    if (y1hi < start1y || end1y < y1lo) return IntersectionType.None;
                }
            }

            float cx = start0x - start1x;
            float cy = start0y - start1y;
            float denominator = ay*bx - ax*by; // Both denominator

            // Alpha tests
            float alphaNumerator = by*cx - bx*cy;
            if (denominator > 0)
            {
                if (alphaNumerator < 0 || alphaNumerator > denominator) return IntersectionType.None;
            }
            else
            {
                if (alphaNumerator > 0 || alphaNumerator < denominator) return IntersectionType.None;
            }
            // Beta tests
            float betaNumerator = ax*cy - ay*cx;
            if (denominator > 0)
            {
                if (betaNumerator < 0 || betaNumerator > denominator) return IntersectionType.None;
            }
            else
            {
                if (betaNumerator > 0 || betaNumerator < denominator) return IntersectionType.None;
            }

            // Compute intersection coordinates

            // Segments are parallel
            if (denominator == 0)
            {
                // Segments are noncollinear
                if (alphaNumerator != 0) return IntersectionType.None;

                // Make sure that start is before end on x axis
                if (start0x > end0x)
                {
                    PGUtils.Swap(ref start0x, ref end0x);
                    PGUtils.Swap(ref start0y, ref end0y);
                }
                if (start1x > end1x)
                {
                    PGUtils.Swap(ref start1x, ref end1x);
                    PGUtils.Swap(ref start1y, ref end1y);
                }

                float biggestStartX = start0x > start1x ? start0x : start1x;
                float smallestEndX = end0x < end1x ? end0x : end1x;

                // Segments are collinear but not intersecting
                if (biggestStartX > smallestEndX) return IntersectionType.None;

                // Make sure that start is before end on y axis
                // Remember swap event to prevent mirroring of intersection segment later
                bool swappedY = false;
                if (start0y > end0y)
                {
                    PGUtils.Swap(ref start0x, ref end0x);
                    PGUtils.Swap(ref start0y, ref end0y);
                    swappedY = true;
                }
                if (start1y > end1y)
                {
                    PGUtils.Swap(ref start1x, ref end1x);
                    PGUtils.Swap(ref start1y, ref end1y);
                    swappedY = true;
                }

                float biggestStartY = start0y > start1y ? start0y : start1y;
                float smallestEndY = end0y < end1y ? end0y : end1y;

                // Segments are collinear but not intersecting
                if (biggestStartY > smallestEndY) return IntersectionType.None;

                if (swappedY)
                {
                    intersection = new Segment2(biggestStartX, smallestEndY, smallestEndX, biggestStartY);
                }
                else
                {
                    intersection = new Segment2(biggestStartX, biggestStartY, smallestEndX, smallestEndY);
                }
                return IntersectionType.Segment;
            }

            float numerator = alphaNumerator*ax; // Numerator
            float x = start0x + numerator/denominator; // Intersection x

            numerator = alphaNumerator*ay;
            float y = start0y + numerator/denominator; // Intersection y

            intersection = new Segment2(x, y, x, y);
            return IntersectionType.Point;
        }

        #region IntSegmentToIntSegment overloads

        public static bool IntSegmentToIntSegment(IntSegment2 segment0, IntSegment2 segment1,
            bool testBoundingBox = false)
        {
            IntSegment2 intersection;
            return IntSegmentToIntSegmentE(segment0.a, segment0.b, segment1.a, segment1.b, out intersection,
                testBoundingBox) != IntersectionType.None;
        }

        public static bool IntSegmentToIntSegment(IntVertex2 start0, IntVertex2 end0, IntVertex2 start1, IntVertex2 end1,
            bool testBoundingBox = false)
        {
            IntSegment2 intersection;
            return IntSegmentToIntSegmentE(start0.x, start0.y, end0.x, end0.y, start1.x, start1.y, end1.x, end1.y,
                out intersection, testBoundingBox) != IntersectionType.None;
        }

        public static bool IntSegmentToIntSegment(int start0x, int start0y, int end0x, int end0y, int start1x,
            int start1y, int end1x, int end1y, bool testBoundingBox = false)
        {
            IntSegment2 intersection;
            return IntSegmentToIntSegmentE(start0x, start0y, end0x, end0y, start1x, start1y, end1x, end1y,
                out intersection, testBoundingBox) != IntersectionType.None;
        }

        public static bool IntSegmentToIntSegment(IntSegment2 segment0, IntSegment2 segment1,
            out IntSegment2 intersection, bool testBoundingBox = false)
        {
            return IntSegmentToIntSegmentE(segment0.a, segment0.b, segment1.a, segment1.b, out intersection,
                testBoundingBox) != IntersectionType.None;
        }

        public static bool IntSegmentToIntSegment(IntVertex2 start0, IntVertex2 end0, IntVertex2 start1, IntVertex2 end1,
            out IntSegment2 intersection, bool testBoundingBox = false)
        {
            return IntSegmentToIntSegmentE(start0.x, start0.y, end0.x, end0.y, start1.x, start1.y, end1.x, end1.y,
                out intersection, testBoundingBox) != IntersectionType.None;
        }

        public static bool IntSegmentToIntSegment(int start0x, int start0y, int end0x, int end0y, int start1x,
            int start1y, int end1x, int end1y, out IntSegment2 intersection, bool testBoundingBox = false)
        {
            return IntSegmentToIntSegmentE(start0x, start0y, end0x, end0y, start1x, start1y, end1x, end1y,
                out intersection, testBoundingBox) != IntersectionType.None;
        }

        public static IntersectionType IntSegmentToIntSegmentE(IntSegment2 segment0, IntSegment2 segment1,
            bool testBoundingBox = false)
        {
            IntSegment2 intersection;
            return IntSegmentToIntSegmentE(segment0.a, segment0.b, segment1.a, segment1.b, out intersection,
                testBoundingBox);
        }

        public static IntersectionType IntSegmentToIntSegmentE(IntVertex2 start0, IntVertex2 end0, IntVertex2 start1,
            IntVertex2 end1, bool testBoundingBox = false)
        {
            IntSegment2 intersection;
            return IntSegmentToIntSegmentE(start0.x, start0.y, end0.x, end0.y, start1.x, start1.y, end1.x, end1.y,
                out intersection, testBoundingBox);
        }

        public static IntersectionType IntSegmentToIntSegmentE(int start0x, int start0y, int end0x, int end0y,
            int start1x, int start1y, int end1x, int end1y, bool testBoundingBox = false)
        {
            IntSegment2 intersection;
            return IntSegmentToIntSegmentE(start0x, start0y, end0x, end0y, start1x, start1y, end1x, end1y,
                out intersection, testBoundingBox);
        }

        public static IntersectionType IntSegmentToIntSegmentE(IntSegment2 segment0, IntSegment2 segment1,
            out IntSegment2 intersection, bool testBoundingBox = false)
        {
            return IntSegmentToIntSegmentE(segment0.a, segment0.b, segment1.a, segment1.b, out intersection,
                testBoundingBox);
        }

        public static IntersectionType IntSegmentToIntSegmentE(IntVertex2 start0, IntVertex2 end0, IntVertex2 start1,
            IntVertex2 end1, out IntSegment2 intersection, bool testBoundingBox = false)
        {
            return IntSegmentToIntSegmentE(start0.x, start0.y, end0.x, end0.y, start1.x, start1.y, end1.x, end1.y,
                out intersection, testBoundingBox);
        }

        #endregion IntSegmentToIntSegment overloads

        /// <summary>
        /// Based on "Faster Line Segment Intersection" by Franklin Antonio, Graphics Gems III
        /// </summary>
        public static IntersectionType IntSegmentToIntSegmentE(int start0x, int start0y, int end0x, int end0y,
            int start1x, int start1y, int end1x, int end1y, out IntSegment2 intersection, bool testBoundingBox = false)
        {
            intersection = new IntSegment2();
            int ax = end0x - start0x;
            int ay = end0y - start0y;
            int bx = start1x - end1x;
            int by = start1y - end1y;

            if (testBoundingBox)
            {
                // X bound box test
                int x1hi = end0x;
                int x1lo = start0x;
                if (ax < 0)
                {
                    x1lo = end0x;
                    x1hi = start0x;
                }
                if (bx > 0)
                {
                    if (x1hi < end1x || start1x < x1lo) return IntersectionType.None;
                }
                else
                {
                    if (x1hi < start1x || end1x < x1lo) return IntersectionType.None;
                }

                // Y bound box test
                int y1hi = end0y;
                int y1lo = start0y;
                if (ay < 0)
                {
                    y1lo = end0y;
                    y1hi = start0y;
                }
                if (by > 0)
                {
                    if (y1hi < end1y || start1y < y1lo) return IntersectionType.None;
                }
                else
                {
                    if (y1hi < start1y || end1y < y1lo) return IntersectionType.None;
                }
            }

            int cx = start0x - start1x;
            int cy = start0y - start1y;
            int denominator = ay*bx - ax*by; // Both denominator

            // Alpha tests
            int alphaNumerator = by*cx - bx*cy;
            if (denominator > 0)
            {
                if (alphaNumerator < 0 || alphaNumerator > denominator) return IntersectionType.None;
            }
            else
            {
                if (alphaNumerator > 0 || alphaNumerator < denominator) return IntersectionType.None;
            }
            // Beta tests
            int betaNumerator = ax*cy - ay*cx;
            if (denominator > 0)
            {
                if (betaNumerator < 0 || betaNumerator > denominator) return IntersectionType.None;
            }
            else
            {
                if (betaNumerator > 0 || betaNumerator < denominator) return IntersectionType.None;
            }

            // Compute intersection coordinates

            // Segments are parallel
            if (denominator == 0)
            {
                // Segments are noncollinear
                if (alphaNumerator != 0) return IntersectionType.None;

                // Make sure that start is before end on x axis
                if (start0x > end0x)
                {
                    PGUtils.Swap(ref start0x, ref end0x);
                    PGUtils.Swap(ref start0y, ref end0y);
                }
                if (start1x > end1x)
                {
                    PGUtils.Swap(ref start1x, ref end1x);
                    PGUtils.Swap(ref start1y, ref end1y);
                }

                int biggestStartX = start0x > start1x ? start0x : start1x;
                int smallestEndX = end0x < end1x ? end0x : end1x;

                // Segments are collinear but not intersecting
                if (biggestStartX > smallestEndX) return IntersectionType.None;

                // Make sure that start is before end on y axis
                // Remember swap event to prevent mirroring of intersection segment later
                bool swappedY = false;
                if (start0y > end0y)
                {
                    PGUtils.Swap(ref start0x, ref end0x);
                    PGUtils.Swap(ref start0y, ref end0y);
                    swappedY = true;
                }
                if (start1y > end1y)
                {
                    PGUtils.Swap(ref start1x, ref end1x);
                    PGUtils.Swap(ref start1y, ref end1y);
                    swappedY = true;
                }

                int biggestStartY = start0y > start1y ? start0y : start1y;
                int smallestEndY = end0y < end1y ? end0y : end1y;

                // Segments are collinear but not intersecting
                if (biggestStartY > smallestEndY) return IntersectionType.None;

                if (swappedY)
                {
                    intersection = new IntSegment2(biggestStartX, smallestEndY, smallestEndX, biggestStartY);
                }
                else
                {
                    intersection = new IntSegment2(biggestStartX, biggestStartY, smallestEndX, smallestEndY);
                }
                return IntersectionType.Segment;
            }

            int numerator = alphaNumerator*ax; // Numerator
            int x = start0x + numerator/denominator; // Intersection x

            numerator = alphaNumerator*ay;
            int y = start0y + numerator/denominator; // Intersection y

            intersection = new IntSegment2(x, y, x, y);
            return IntersectionType.Point;
        }

        public static bool PolygonToPolygon(Polygon polygon1, Polygon polygon2)
        {
            return polygon1.segments.Any(s1 => polygon2.segments.Any(s2 => s1.Intersects(s2)));
        }

        public static bool PolygonToPolygon(Polygon polygon1, Polygon polygon2, out List<Segment2> intersections)
        {
            intersections = new List<Segment2>();
            foreach (var s1 in polygon1.segments)
            {
                foreach (var s2 in polygon2.segments)
                {
                    Segment2 intersection;
                    if (s1.Intersects(s2, out intersection))
                    {
                        intersections.Add(intersection);
                    }
                }
            }
            return intersections.Count > 0;
        }

        public static bool IntPolygonToIntPolygon(IntPolygon polygon1, IntPolygon polygon2)
        {
            return polygon1.segments.Any(s1 => polygon2.segments.Any(s2 => s1.Intersects(s2)));
        }

        public static bool IntPolygonToIntPolygon(IntPolygon polygon1, IntPolygon polygon2,
            out List<IntSegment2> intersections)
        {
            intersections = new List<IntSegment2>();
            foreach (var s1 in polygon1.segments)
            {
                foreach (var s2 in polygon2.segments)
                {
                    IntSegment2 intersection;
                    if (s1.Intersects(s2, out intersection))
                    {
                        intersections.Add(intersection);
                    }
                }
            }
            return intersections.Count > 0;
        }

        public static bool PolygonToSegment(Polygon polygon, Segment2 segment)
        {
            return polygon.segments.Any(s => s.Intersects(segment));
        }

        public static bool IntPolygonToIntSegment(IntPolygon polygon, IntSegment2 segment)
        {
            return polygon.segments.Any(s => s.Intersects(segment));
        }
    }
}