using System.Numerics;

namespace RayTracing
{
    public sealed class SurfaceInfo
    {
        public SurfaceData Data
        {
            get;
            set;
        }

        public Vector3 Normal
        {
            get;
            set;
        }
    }
}