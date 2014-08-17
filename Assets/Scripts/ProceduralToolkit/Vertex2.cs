using System;
using UnityEngine;

namespace ProceduralToolkit
{
    [Serializable]
    public class Vertex2 : IComparable<Vertex2>
    {
        [SerializeField] public float x;

        [SerializeField] public float y;

        public float this[int index]
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

        public static Vertex2 one
        {
            get { return new Vertex2(1, 1); }
        }

        public static Vertex2 zero
        {
            get { return new Vertex2(0, 0); }
        }

        public static Vertex2 right
        {
            get { return new Vertex2(1, 0); }
        }

        public static Vertex2 left
        {
            get { return new Vertex2(-1, 0); }
        }

        public static Vertex2 up
        {
            get { return new Vertex2(0, 1); }
        }

        public static Vertex2 down
        {
            get { return new Vertex2(0, -1); }
        }

        #endregion Static properties

        public float magnitude
        {
            get { return Mathf.Sqrt(sqrMagnitude); }
        }

        public float sqrMagnitude
        {
            get { return (x*x) + (y*y); }
        }

        public Vertex2 normalized
        {
            get
            {
                var vector = new Vertex2(this);
                vector.Normalize();
                return vector;
            }
        }

        #region Constructors

        public Vertex2()
        {
            x = 0;
            y = 0;
        }

        public Vertex2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vertex2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vertex2(Vertex2 v)
        {
            x = v.x;
            y = v.y;
        }

        public Vertex2(Vector2 v)
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

        public static float Dot(Vertex2 a, Vertex2 b)
        {
            return (a.x*b.x) + (a.y*b.y);
        }

        public static float Distance(Vertex2 a, Vertex2 b)
        {
            return (a - b).magnitude;
        }

        #region Implicit operators

        public static implicit operator Vertex2(Vector2 v)
        {
            return new Vertex2(v.x, v.y);
        }

        public static implicit operator Vertex2(Vector3 v)
        {
            return new Vertex2(v.x, v.y);
        }

        #endregion Implicit operators

        #region Explicit operators

        public static explicit operator Vector2(Vertex2 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static explicit operator Vector3(Vertex2 v)
        {
            return new Vector3(v.x, v.y);
        }

        public static explicit operator IntVertex2(Vertex2 v)
        {
            return new IntVertex2(v.x, v.y);
        }

        #endregion Explicit operators

        #region Arithmetic operators

        public static Vertex2 operator +(Vertex2 a, Vertex2 b)
        {
            return new Vertex2(a.x + b.x, a.y + b.y);
        }

        public static Vertex2 operator -(Vertex2 a, Vertex2 b)
        {
            return new Vertex2(a.x - b.x, a.y - b.y);
        }

        public static Vertex2 operator -(Vertex2 a)
        {
            return new Vertex2(-a.x, -a.y);
        }

        public static Vertex2 operator *(int d, Vertex2 a)
        {
            return new Vertex2(a.x*d, a.y*d);
        }

        public static Vertex2 operator *(float d, Vertex2 a)
        {
            return new Vertex2(a.x*d, a.y*d);
        }

        public static Vertex2 operator *(Vertex2 a, int d)
        {
            return new Vertex2(a.x*d, a.y*d);
        }

        public static Vertex2 operator *(Vertex2 a, float d)
        {
            return new Vertex2(a.x*d, a.y*d);
        }

        public static Vertex2 operator /(Vertex2 a, int d)
        {
            return new Vertex2(a.x/d, a.y/d);
        }

        public static Vertex2 operator /(Vertex2 a, float d)
        {
            return new Vertex2(a.x/d, a.y/d);
        }

        #endregion Arithmetic operators

        public static bool operator ==(Vertex2 a, Vertex2 b)
        {
            if (ReferenceEquals(a, b)) return true;
            if ((object) a == null || (object) b == null) return false;
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Vertex2 a, Vertex2 b)
        {
            return !(a == b);
        }

        public int CompareTo(Vertex2 other)
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