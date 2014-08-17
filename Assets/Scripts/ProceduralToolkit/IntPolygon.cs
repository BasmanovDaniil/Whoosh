using System.Collections.Generic;
using ProceduralToolkit.Generic;
using UnityEngine;

namespace ProceduralToolkit
{
    public class IntPolygon : CircularList<IntVertex2>
    {
        #region Static properties

        public static IntPolygon unitCircle
        {
            get { return Circle(1); }
        }

        public static IntPolygon unitRect
        {
            get
            {
                return new IntPolygon
                {
                    new IntVertex2(-1, -1),
                    new IntVertex2(-1, 1),
                    new IntVertex2(1, 1),
                    new IntVertex2(1, -1)
                };
            }
        }

        public static IntPolygon triangle
        {
            get { return Regular(3); }
        }

        public static IntPolygon square
        {
            get { return Regular(4, 1, Mathf.PI/4); }
        }

        public static IntPolygon pentagon
        {
            get { return Regular(5); }
        }

        public static IntPolygon hexagon
        {
            get { return Regular(6); }
        }

        public static IntPolygon heptagon
        {
            get { return Regular(7); }
        }

        public static IntPolygon octagon
        {
            get { return Regular(8); }
        }

        #endregion Static properties

        public int area
        {
            get { return Mathf.Abs(signedArea); }
        }

        public int signedArea
        {
            get
            {
                if (Count < 3) return 0;
                int a = 0;
                for (var i = 0; i < Count; i++)
                {
                    a += PGUtils.PerpDot(this[i - 1], this[i]);
                }
                return a/2;
            }
        }

        public int perimeter
        {
            get
            {
                int p = 0;
                for (var i = 0; i < Count; i++)
                {
                    p += IntVertex2.Distance(this[i - 1], this[i]);
                }
                return p;
            }
        }

        /// <summary>
        /// Centroid
        /// </summary>
        public IntVertex2 center
        {
            get
            {
                var sum = IntVertex2.zero;
                foreach (var vertex in this)
                {
                    sum += vertex;
                }
                return sum/Count;
            }
            set { Move(value - center); }
        }

        public bool clockwise
        {
            get { return orientation == Orientation.Clockwise; }
            set
            {
                if (Count < 3) return;
                var c = clockwise;
                if (value && !c) Reverse();
                if (!value && c) Reverse();
            }
        }

        public Orientation orientation
        {
            get
            {
                if (Count < 3) return Orientation.NonOrientable;
                var sa = signedArea;
                if (sa > 0) return Orientation.Clockwise;
                if (sa < 0) return Orientation.CounterClockwise;
                return Orientation.NonOrientable;
            }
        }

        public bool convex
        {
            get
            {
                if (Count < 3) return false;
                if (Count == 3) return true;

                var c = PGUtils.PerpDot(Edge(0), Edge(1)) >= 0;
                for (var i = 1; i < Count; i++)
                {
                    if (PGUtils.PerpDot(Edge(i), Edge(i + 1)) >= 0 && !c)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public CircularList<IntSegment2> segments
        {
            get
            {
                var s = new CircularList<IntSegment2>(Count);
                for (int i = 0; i < Count; i++)
                {
                    s.Add(new IntSegment2(this[i], this[i + 1]));
                }
                return s;
            }
        }

        public CircularList<Indexed<IntSegment2>> indexedSegments
        {
            get
            {
                var s = new CircularList<Indexed<IntSegment2>>(Count);
                for (int i = 0; i < Count; i++)
                {
                    s.Add(new Indexed<IntSegment2>(new IntSegment2(this[i], this[i + 1]), i));
                }
                return s;
            }
        }

        public AABB<IntVertex2> aabb
        {
            get
            {
                var min = new IntVertex2(this[0]);
                var max = new IntVertex2(this[0]);
                foreach (var vertex in this)
                {
                    if (vertex.x < min.x) min.x = vertex.x;
                    if (vertex.x > max.x) max.x = vertex.x;
                    if (vertex.y < min.y) min.y = vertex.y;
                    if (vertex.y > max.y) max.y = vertex.y;
                }
                return new AABB<IntVertex2>(min, max);
            }
        }

        #region Constructors

        public IntPolygon() : base()
        {
        }

        public IntPolygon(IEnumerable<IntVertex2> collection) : base(collection)
        {
        }

        public IntPolygon(int capacity) : base(capacity)
        {
        }

        public IntPolygon(Polygon polygon) : base(polygon.Count)
        {
            foreach (var vertex in polygon)
            {
                Add((IntVertex2) vertex);
            }
        }

        #endregion Constructors

        public static IntPolygon Regular(int sides, int radius = 1, float angle = Mathf.PI/2)
        {
            var vertices = new IntVertex2[sides];
            var step = Mathf.PI*2/sides;
            for (int i = 0; i < sides; i++)
            {
                vertices[i] = new IntVertex2((int) (radius*Mathf.Cos(angle)), (int) (radius*Mathf.Sin(angle)));
                angle += step;
            }
            return new IntPolygon(vertices);
        }

        public static IntPolygon Circle(int radius)
        {
            return Regular(Mathf.Max(32*radius, 8), radius, 0);
        }

        public static IntPolygon Circle(int radius, int points)
        {
            return Regular(points, radius, 0);
        }

        public IntVertex2 Edge(int index)
        {
            return this[index + 1] - this[index];
        }

        public IntSegment2 Segment(int index)
        {
            return new IntSegment2(this[index], this[index + 1]);
        }

        public void Extrude(int index, IntVertex2 offset)
        {
            Insert(index, this[index] + offset);
        }

        public void ExtrudeRange(int startIndex, int endIndex, IntVertex2 offset)
        {
            if (startIndex > endIndex)
            {
                PGUtils.Swap(ref startIndex, ref endIndex);
            }

            var startV = this[startIndex] + offset;
            var endV = this[endIndex] + offset;

            if ((endIndex - startIndex) > 1)
            {
                for (int i = startIndex + 1; i < endIndex; i++)
                {
                    this[i] += offset;
                }
            }

            Insert(startIndex + 1, startV);
            Insert(endIndex + 1, endV);
        }

        public void Move(IntVertex2 offset)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i] += offset;
            }
        }

        public bool Contains(IntVertex2 point, out int wn)
        {
            wn = 0;
            for (int i = 0; i < Count; i++)
            {
                if (this[i].y <= point.y)
                {
                    // start y <= P.y
                    if (this[i + 1].y > point.y) // an upward crossing
                        if (PGUtils.LocatePointOnLine(this[i], this[i + 1], point) > 0) // P left of edge
                            ++wn; // have a valid up intersect
                }
                else
                {
                    // start y > P.y (no test needed)
                    if (this[i + 1].y <= point.y) // a downward crossing
                        if (PGUtils.LocatePointOnLine(this[i], this[i + 1], point) < 0) // P right of edge
                            --wn; // have a valid down intersect
                }
            }
            return wn != 0;
        }

        public void RemoveDuplicateVertices()
        {
            var hashSet = new HashSet<IntVertex2>(this);
            Clear();
            AddRange(hashSet);
        }

        public void RemoveCollinearVertices()
        {
            var newPolygon = RemoveCollinearVertices(this);
            Clear();
            AddRange(newPolygon);
        }

        public static IntPolygon RemoveCollinearVertices(IntPolygon polygon)
        {
            if (polygon.Count <= 3)
            {
                return polygon;
            }

            var newPolygon = new IntPolygon();
            for (int i = 0; i < polygon.Count; i++)
            {
                if (PGUtils.LocatePointOnLine(polygon[i - 1], polygon[i + 1], polygon[i]) != 0)
                {
                    newPolygon.Add(polygon[i]);
                }
            }
            return newPolygon;
        }

        public bool Intersects(IntPolygon polygon)
        {
            return Intersection.IntPolygonToIntPolygon(this, polygon);
        }

        public bool Intersects(IntPolygon polygon, out List<IntSegment2> intersections)
        {
            return Intersection.IntPolygonToIntPolygon(this, polygon, out intersections);
        }

        public bool Intersects(IntSegment2 segment)
        {
            return Intersection.IntPolygonToIntSegment(this, segment);
        }

        #region Explicit operators

        public static explicit operator Polygon(IntPolygon polygon)
        {
            return new Polygon(polygon);
        }

        public static explicit operator IntVertex2[](IntPolygon polygon)
        {
            return polygon.ToArray();
        }

        #endregion Explicit operators

        #region Arithmetic operators

        public static IntPolygon operator +(IntPolygon polygon, IntVertex2 vector)
        {
            var newPolygon = new IntPolygon(polygon);
            newPolygon.Move(vector);
            return newPolygon;
        }

        public static IntPolygon operator +(IntVertex2 vector, IntPolygon polygon)
        {
            var newPolygon = new IntPolygon(polygon);
            newPolygon.Move(vector);
            return newPolygon;
        }

        public static IntPolygon operator -(IntPolygon polygon, IntVertex2 vector)
        {
            var newPolygon = new IntPolygon(polygon);
            newPolygon.Move(-vector);
            return newPolygon;
        }

        public static IntPolygon operator *(IntPolygon polygon, int x)
        {
            var newPolygon = new IntPolygon(polygon);
            var c = newPolygon.center;
            for (int i = 0; i < newPolygon.Count; i++)
            {
                newPolygon[i] *= x;
            }
            newPolygon.center = c;
            return newPolygon;
        }

        public static IntPolygon operator *(IntPolygon polygon, IntVertex2 vector)
        {
            var newPolygon = new IntPolygon(polygon);
            var c = newPolygon.center;
            for (int i = 0; i < newPolygon.Count; i++)
            {
                newPolygon[i] = new IntVertex2(newPolygon[i].x*vector.x, newPolygon[i].y*vector.y);
            }
            newPolygon.center = c;
            return newPolygon;
        }

        #endregion Arithmetic operators
    }
}