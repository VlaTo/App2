using System.Numerics;

namespace RayTracing
{
    public sealed class Ray
    {
        public Vector3 Origin
        {
            get;
            set;
        }

        public Vector3 Direction
        {
            get;
            set;
        }

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Vector3 Point(float distance) => Origin + Direction * distance;
    }
}