namespace RayTracing
{
    public struct Dimension
    {
        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public Dimension(int value)
        {
            Width = value;
            Height = value;
        }

        public Dimension(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}