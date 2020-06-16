using System;

namespace RayTracing
{
    public struct Area
    {
        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

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

        public int Right
        {
            get;
        }

        public int Bottom
        {
            get;
        }

        public Area(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Right = x + width;
            Bottom = y + height;
        }

        /*public Area(PointF point1, PointF point2)
        {
            X = MathF.Min(point1.X, point2.X);
            Y = MathF.Min(point1.Y, point2.Y);
            Width = MathF.Max(point1.X, point2.X) - X;
            Height = MathF.Max(point1.Y, point2.Y) - Y;
        }

        public Area(PointF origin, SizeF size)
        {
            X = origin.X;
            Y = origin.Y;
            Width = size.Width;
            Height = size.Height;
        }*/
    }
}