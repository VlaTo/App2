using System.Numerics;

namespace RayTracing.Extensions
{
    public static class RayExtensions
    {
        public static void Move(this Ray ray, Camera camera, ref PointF point)
        {
            ray.Origin = camera.Origin;
            ray.Direction = Vector3.Normalize(camera.Direction + camera.Vx * point.X + camera.Vy * point.Y);
        }
    }
}