using System.Numerics;

namespace RayTracing
{
    public abstract class Figure
    {
        public SurfaceInfo Material
        {
            get;
        }

        public virtual SurfaceInfo FindTexture(Vector3 point)
        {
            var clone = Material.Clone();
            
            clone.Normal = FindNormal(point);

            return clone;
        }

        public abstract Vector3 FindNormal(Vector3 point);

        public abstract int Intersect(Ray ray, float distance);
    }
}