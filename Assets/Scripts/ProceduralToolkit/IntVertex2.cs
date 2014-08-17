using System;
using UnityEngine;

namespace ProceduralToolkit
{
    [Serializable]
    public class IntVertex2 : IComparable<IntVertex2>
    {
        [SerializeField] public int x;

        [SerializeField] public int y;

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                }
                throw new IndexOutOfRangeException("Invalid index!");
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid index!");
                }
            }
        }

        #region Static properties

        public static IntVertex2 one
        {
            get { return new IntVertex2(1, 1); }
        }

        public static IntVertex2 zero
        {
            get { return new IntVertex2(0, 0); }
        }

        public static IntVertex2 right
        {
            get { return new IntVertex2(1, 0); }
        }

        public static IntVertex2 left
        {
            get { return new IntVertex2(-1, 0); }
        }

        public static IntVertex2 up
        {
            get { return new IntVertex2(0, 1); }
        }

        public static IntVertex2 down
        {
            get { return new IntVertex2(0, -1); }
        }

        #endregion Static properties

        public int magnitude
        {
            get { return (int) Mathf.Sqrt(sqrMagnitude); }
        }

        public int sqrMagnitude
        {
            get { return (x*x) + (y*y); }
        }

        public IntVertex2 normalized
        {
            get
            {
                var vector = new IntVertex2(this);
                vector.Normalize();
                return vector;
            }
        }

        #region Constructors

        public IntVertex2()
        {
            x = 0;
            y = 0;
        }

        public IntVertex2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public IntVertex2(float x, float y)
        {
            this.x = (int) x;
            this.y = (int) y;
        }

        public IntVertex2(IntVertex2 v)
        {
            x = v.x;
            y = v.y;
        }

        #endregion Constructors

        public void Normalize()
        {
            var n = 1/magnitude;
            x *= n;
            y *= n;
        }

        public void Swap()
        {
            PGUtils.Swap(ref x, ref y);
        }

        public static int Dot(IntVertex2 a, IntVertex2 b)
        {
            return (a.x*b.x) + (a.y*b.y);
        }

        public static int Distance(IntVertex2 a, IntVertex2 b)
        {
            return (a - b).magnitude;
        }

        #region Implicit operators

        public static implicit operator IntVertex2(Vector2 v)
        {
            return new IntVertex2(v.x, v.y);
        }

        public static implicit operator IntVertex2(Vector3 v)
        {
            return new IntVertex2(v.x, v.y);
        }

        #endregion Implicit operators

        #region Explicit operators

        public static explicit operator Vector2(IntVertex2 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static explicit operator Vector3(IntVertex2 v)
        {
            return new Vector3(v.x, v.y);
        }

        public static explicit operator Vertex2(IntVertex2 v)
        {
            return new Vertex2(v.x, v.y);
        }

        #endregion Explicit operators

        #region Arithmetic operators

        public static IntVertex2 operator +(IntVertex2 a, IntVertex2 b)
        {
            return new IntVertex2(a.x + b.x, a.y + b.y);
        }

        public static IntVertex2 operator -(IntVertex2 a, IntVertex2 b)
        {
            return new IntVertex2(a.x - b.x, a.y - b.y);
        }

        public static IntVertex2 operator -(IntVertex2 a)
        {
            return new IntVertex2(-a.x, -a.y);
        }

        public static IntVertex2 operator *(int d, IntVertex2 a)
        {
            return new IntVertex2(a.x*d, a.y*d);
        }

        public static IntVertex2 operator *(float d, IntVertex2 a)
        {
            return new IntVertex2(a.x*d, a.y*d);
        }

        public static IntVertex2 operator *(IntVertex2 a, int d)
        {
            return new IntVertex2(a.x*d, a.y*d);
        }

        public static IntVertex2 operator *(IntVertex2 a, float d)
        {
            return new IntVertex2(a.x*d, a.y*d);
        }

        public static IntVertex2 operator /(IntVertex2 a, int d)
        {
            return new IntVertex2(a.x/d, a.y/d);
        }

        public static IntVertex2 operator /(IntVertex2 a, float d)
        {
            return new IntVertex2(a.x/d, a.y/d);
        }

        #endregion Arithmetic operators

        public static bool operator ==(IntVertex2 a, IntVertex2 b)
        {
            if (ReferenceEquals(a, b)) return true;
            if ((object) a == null || (object) b == null) return false;
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(IntVertex2 a, IntVertex2 b)
        {
            return !(a == b);
        }

        public int CompareTo(IntVertex2 other)
        {
            if (other == null) return 1;
            return x == other.x ? y.CompareTo(other.y) : x.CompareTo(other.x);
        }

        public override string ToString()
        {
            return "(" + x + " " + y + ")";
        }
    }
}