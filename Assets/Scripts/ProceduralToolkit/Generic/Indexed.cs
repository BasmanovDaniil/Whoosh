namespace ProceduralToolkit.Generic
{
    public class Indexed<T>
    {
        public T value;
        public int index;

        public Indexed(T value, int index)
        {
            this.value = value;
            this.index = index;
        }

        //public static bool operator ==(IndexedVector<T> left, IndexedVector<T> right)
        //{
        //    return left.vector.Equals(right.vector) && (left.index == right.index);
        //}

        //public static bool operator !=(IndexedVector<T> left, IndexedVector<T> right)
        //{
        //    return !(left == right);
        //}

        public static bool operator ==(Indexed<T> left, Indexed<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Indexed<T> left, Indexed<T> right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode() ^ index.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Indexed<T>))
                return false;

            return Equals((Indexed<T>) obj);
        }

        public bool Equals(Indexed<T> other)
        {
            return (value.Equals(other.value)) && (index == other.index);
        }
    }
}