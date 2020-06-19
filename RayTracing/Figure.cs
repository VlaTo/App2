using System.Numerics;

namespace RayTracing
{
    public abstract class Figure
    {
        public SurfaceData Material
        {
            get;
            set;
        }

        public virtual SurfaceInfo FindTexture(ref Vector3 point)
        {
            return new SurfaceInfo
            {
                Data = Material,
                Normal = FindNormal(ref point)
            };
        }

        public abstract Vector3 FindNormal(ref Vector3 point);

        public abstract bool TryIntersect(Ray ray, ref float distance);
    }
}