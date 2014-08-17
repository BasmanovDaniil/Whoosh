namespace ProceduralToolkit
{
    public struct AABB<T>
    {
        public T min;
        public T max;

        public AABB(T min, T max)
        {
            this.min = min;
            this.max = max;
        }
    }
}