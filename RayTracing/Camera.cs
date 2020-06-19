using System.Numerics;

namespace RayTracing
{
    public sealed class Camera
    {
        public static readonly Vector3 Up = Vector3.UnitY;

        public Vector3 Origin
        {
            get;
        }

        public Vector3 Direction
        {
            get;
        }

        public Vector3 Vx
        {
            get;
        }

        public Vector3 Vy
        {
            get;
        }

        private Camera(Vector3 origin, Vector3 direction, Vector3 vx, Vector3 vy)
        {
            Origin = origin;
            Direction = direction;
            Vx = vx;
            Vy = vy;
        }

        public static Camera Set(Vector3 origin, Vector3 direction, float rotation)
        {
            var vx = Vector3.Normalize(Vector3.Cross(Up, direction));
            var vy = Vector3.Normalize(Vector3.Cross(direction, vx));
            return new Camera(origin, direction, vx, vy);
        }
    }
}