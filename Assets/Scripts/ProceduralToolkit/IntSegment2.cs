using System;
using UnityEngine;

namespace ProceduralToolkit
{
    [Serializable]
    public class IntSegment2
    {
        [SerializeField] public IntVertex2 a;
        [SerializeField] public IntVertex2 b;

        /// <summary>
        /// Reminder: may return float.NaN for vertical segments
        /// </summary>
        public float slope
        {
            get { return (float) (b.y - a.y)/(b.x - a.x); }
        }

        public IntVertex2 direction
        {
            get { return b - a; }
        }

        public IntVertex2 left
        {
            get { return PGUtils.Perp(b - a).normalized; }
        }

        public IntVertex2 right
        {
            get { return -PGUtils.Perp(b - a).normalized; }
        }

        public int length
        {
            get { return direction.magnitude; }
        }

        #region Constructors

        public IntSegment2()
        {
        }

        public IntSegment2(IntSegment2 segment)
        {
            a = segment.a;
            b = segment.b;
        }

        public IntSegment2(IntVertex2 a, IntVertex2 b)
        {
            this.a = a;
            this.b = b;
        }

        public IntSegment2(IntVertex2 b)
        {
            a = IntVertex2.zero;
            this.b = b;
        }

        public IntSegment2(int x0, int y0, int x1, int y1)
        {
            a = new IntVertex2(x0, y0);
            b = new IntVertex2(x1, y1);
        }

        #endregion Constructors

        public int CompareByLength(IntSegment2 segment)
        {
            if (length > segment.length) return 1;
            if (length == segment.length) return 0;
            return -1;
        }

        public bool Intersects(IntSegment2 segment, bool testBoundingBox = false)
        {
            return Intersection.IntSegmentToIntSegment(this, segment, testBoundingBox);
        }

        public bool Intersects(IntSegment2 segment, out IntSegment2 intersection, bool testBoundingBox = false)
        {
            return Intersection.IntSegmentToIntSegment(this, segment, out intersection, testBoundingBox);
        }

        #region Arithmetic operators

        public static IntSegment2 operator +(IntSegment2 segment, IntVertex2 vector)
        {
            return new IntSegment2(segment.a + vector, segment.b + vector);
        }

        public static IntSegment2 operator -(IntSegment2 segment, IntVertex2 vector)
        {
            return new IntSegment2(segment.a - vector, segment.b - vector);
        }

        #endregion Arithmetic operators

        public override string ToString()
        {
            return "(" + a + " " + b + ")";
        }
    }
}