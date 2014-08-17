using System.Collections.Generic;
using ProceduralToolkit.Generic;
using UnityEngine;

namespace ProceduralToolkit
{
    public class Polygon : CircularList<Vertex2>
    {
        #region Static properties

        public static Polygon unitCircle
        {
            get { return Circle(1); }
        }

        public static Polygon unitRect
        {
            get
            {
                return new Polygon
                {
                    new Vertex2(-0.5f, -0.5f),
                    new Vertex2(-0.5f, 0.5f),
                    new Vertex2(0.5f, 0.5f),
                    new Vertex2(0.5f, -0.5f)
                };
            }
        }

        public static Polygon triangle
        {
            get { return Regular(3); }
        }

        public static Polygon square
        {
            get { return Regular(4, 1, Mathf.PI/4); }
        }

        public static Polygon pentagon
        {
            get { return Regular(5); }
        }

        public static Polygon hexagon
        {
            get { return Regular(6); }
        }

        public static Polygon heptagon
        {
            get { return Regular(7); }
        }

        public static Polygon octagon
        {
            get { return Regular(8); }
        }

        #endregion Static properties

        public float area
        {
            get { return Mathf.Abs(signedArea); }
        }

        public float signedArea
        {
            get
            {
                if (Count < 3) return 0f;
                float a = 0;
                for (var i = 0; i < Count; i++)
                {
                    a += PGUtils.PerpDot(this[i - 1], this[i]);
                }
                return a/2;
            }
        }

        public float perimeter
        {
            get
            {
                float p = 0;
                for (var i = 0; i < Count; i++)
                {
                    p += Vertex2.Distance(this[i - 1], this[i]);
                }
                return p;
            }
        }

        /// <summary>
        /// Centroid
        /// </summary>
        public Vertex2 center
        {
            get
            {
                var sum = Vertex2.zero;
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

        public CircularList<Segment2> segments
        {
            get
            {
                var s = new CircularList<Segment2>(Count);
                for (int i = 0; i < Count; i++)
                {
                    s.Add(new Segment2(this[i], this[i + 1]));
                }
                return s;
            }
        }

        public CircularList<Indexed<Segment2>> indexedSegments
        {
            get
            {
                var s = new CircularList<Indexed<Segment2>>(Count);
                for (int i = 0; i < Count; i++)
                {
                    s.Add(new Indexed<Segment2>(new Segment2(this[i], this[i + 1]), i));
                }
                return s;
            }
        }

        public AABB<Vertex2> aabb
        {
            get
            {
                var min = new Vertex2(this[0]);
                var max = new Vertex2(this[0]);
                foreach (var vertex in this)
                {
                    if (vertex.x < min.x) min.x = vertex.x;
                    if (vertex.x > max.x) max.x = vertex.x;
                    if (vertex.y < min.y) min.y = vertex.y;
                    if (vertex.y > max.y) max.y = vertex.y;
                }
                return new AABB<Vertex2>(min, max);
            }
        }

        #region Constructors

        public Polygon() : base()
        {
        }

        public Polygon(IEnumerable<Vertex2> collection) : base(collection)
        {
        }

        public Polygon(int capacity) : base(capacity)
        {
        }

        public Polygon(IntPolygon polygon) : base(polygon.Count)
        {
            foreach (var vertex in polygon)
            {
                Add((Vertex2) vertex);
            }
        }

        #endregion Constructors

        public static Polygon Regular(int sides, float radius = 1f, float angle = Mathf.PI/2)
        {
            var vertices = new Vertex2[sides];
            var step = Mathf.PI*2/sides;
            for (int i = 0; i < sides; i++)
            {
                vertices[i] = new Vertex2(radius*Mathf.Cos(angle), radius*Mathf.Sin(angle));
                angle += step;
            }
            return new Polygon(vertices);
        }

        public static Polygon Circle(float radius)
        {
            return Regular(Mathf.Max((int) (32*radius), 8), radius, 0);
        }

        public static Polygon Circle(float radius, int points)
        {
            return Regular(points, radius, 0);
        }

        public Vertex2 Edge(int index)
        {
            return this[index + 1] - this[index];
        }

        public Segment2 Segment(int index)
        {
            return new Segment2(this[index], this[index + 1]);
        }

        public void Extrude(int index, Vertex2 offset)
        {
            Insert(index, this[index] + offset);
        }

        public void ExtrudeRange(int startIndex, int endIndex, Vertex2 offset)
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

        public void Move(Vertex2 offset)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i] += offset;
            }
        }

        public bool Contains(Vertex2 point, out int wn)
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
            var hashSet = new HashSet<Vertex2>(this);
            Clear();
            AddRange(hashSet);
        }

        public void RemoveCollinearVertices()
        {
            var newPolygon = RemoveCollinearVertices(this);
            Clear();
            AddRange(newPolygon);
        }

        public static Polygon RemoveCollinearVertices(Polygon polygon)
        {
            if (polygon.Count <= 3)
            {
                return polygon;
            }

            var newPolygon = new Polygon();
            for (int i = 0; i < polygon.Count; i++)
            {
                if (PGUtils.LocatePointOnLine(polygon[i - 1], polygon[i + 1], polygon[i]) != 0)
                {
                    newPolygon.Add(polygon[i]);
                }
            }
            return newPolygon;
        }

        public bool Intersects(Polygon polygon)
        {
            return Intersection.PolygonToPolygon(this, polygon);
        }

        public bool Intersects(Polygon polygon, out List<Segment2> intersections)
        {
            return Intersection.PolygonToPolygon(this, polygon, out intersections);
        }

        public bool Intersects(Segment2 segment)
        {
            return Intersection.PolygonToSegment(this, segment);
        }

        #region Explicit operators

        public static explicit operator IntPolygon(Polygon polygon)
        {
            return new IntPolygon(polygon);
        }

        public static explicit operator Vertex2[](Polygon polygon)
        {
            return polygon.ToArray();
        }

        #endregion Explicit operators

        #region Arithmetic operators

        public static Polygon operator +(Polygon polygon, Vertex2 vector)
        {
            var newPolygon = new Polygon(polygon);
            newPolygon.Move(vector);
            return newPolygon;
        }

        public static Polygon operator +(Vertex2 vector, Polygon polygon)
        {
            var newPolygon = new Polygon(polygon);
            newPolygon.Move(vector);
            return newPolygon;
        }

        public static Polygon operator -(Polygon polygon, Vertex2 vector)
        {
            var newPolygon = new Polygon(polygon);
            newPolygon.Move(-vector);
            return newPolygon;
        }

        public static Polygon operator *(Polygon polygon, float x)
        {
            var newPolygon = new Polygon(polygon);
            var c = newPolygon.center;
            for (int i = 0; i < newPolygon.Count; i++)
            {
                newPolygon[i] *= x;
            }
            newPolygon.center = c;
            return newPolygon;
        }

        public static Polygon operator *(Polygon polygon, Vertex2 vector)
        {
            var newPolygon = new Polygon(polygon);
            var c = newPolygon.center;
            for (int i = 0; i < newPolygon.Count; i++)
            {
                newPolygon[i] = new Vertex2(newPolygon[i].x*vector.x, newPolygon[i].y*vector.y);
            }
            newPolygon.center = c;
            return newPolygon;
        }

        #endregion Arithmetic operators
    }
}