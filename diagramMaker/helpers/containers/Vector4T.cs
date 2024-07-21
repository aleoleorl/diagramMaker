namespace diagramMaker.helpers.containers
{
    public struct Vector4<T>
    {
        public T x;
        public T y;
        public T w;
        public T h;
        public Vector4(T x, T y, T w, T h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
    }
}