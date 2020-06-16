using System.Numerics;

namespace RayTracing
{
    public sealed class Camera
    {
        public Vector3 Origin
        {
            get;
        }

        public Vector3 Direction
        {
            get;
        }

        public Vector3 Gravity
        {
            get;
        }

        public Camera(Vector3 origin, Vector3 direction, Vector3 gravity)
        {
            Origin = origin;
            Direction = direction;
            Gravity = gravity;
        }
    }
}