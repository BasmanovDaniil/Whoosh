using System;
using UnityEngine;

namespace ProceduralToolkit
{
    [Serializable]
    public class Segment2
    {
        [SerializeField] public Vertex2 a;
        [SerializeField] public Vertex2 b;

        /// <summary>
        /// Reminder: may return float.NaN for vertical segments
        /// </summary>
        public float slope
        {
            get { return (b.y - a.y)/(b.x - a.x); }
        }

        public Vertex2 direction
        {
            get { return b - a; }
        }

        public Vertex2 left
        {
            get { return PGUtils.Perp(b - a).normalized; }
        }

        public Vertex2 right
        {
            get { return -PGUtils.Perp(b - a).normalized; }
        }

        public float length
        {
            get { return direction.magnitude; }
        }

        #region Constructors

        public Segment2()
        {
        }

        public Segment2(Segment2 segment)
        {
            a = segment.a;
            b = segment.b;
        }

        public Segment2(Vertex2 a, Vertex2 b)
        {
            this.a = a;
            this.b = b;
        }

        public Segment2(Vertex2 b)
        {
            a = Vertex2.zero;
            this.b = b;
        }

        public Segment2(float x0, float y0, float x1, float y1)
        {
            a = new Vertex2(x0, y0);
            b = new Vertex2(x1, y1);
        }

        #endregion Constructors

        public int CompareByLength(Segment2 segment)
        {
            if (length > segment.length) return 1;
            if (length == segment.length) return 0;
            return -1;
        }

        public bool Intersects(Segment2 segment, bool testBoundingBox = false)
        {
            return Intersection.SegmentToSegment(this, segment, testBoundingBox);
        }

        public bool Intersects(Segment2 segment, out Segment2 intersection, bool testBoundingBox = false)
        {
            return Intersection.SegmentToSegment(this, segment, out intersection, testBoundingBox);
        }

        #region Arithmetic operators

        public static Segment2 operator +(Segment2 segment, Vertex2 vector)
        {
            return new Segment2(segment.a + vector, segment.b + vector);
        }

        public static Segment2 operator -(Segment2 segment, Vertex2 vector)
        {
            return new Segment2(segment.a - vector, segment.b - vector);
        }

        #endregion Arithmetic operators

        public override string ToString()
        {
            return "(" + a + " " + b + ")";
        }
    }
}