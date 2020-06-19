using System.Numerics;

namespace RayTracing
{
    public sealed class SurfaceData
    {
        public float Ka
        {
            get;
            set;
        }

        public float Kd
        {
            get;
            set;
        }

        public float Ks
        {
            get;
            set;
        }

        public float Kr
        {
            get;
            set;
        }

        public float Kt
        {
            get;
            set;
        }

        public Vector3 Color
        {
            get;
            set;
        }

        /*public Vector3 Normal
        {
            get;
            set;
        }*/

        public Medium Medium
        {
            get;
            set;
        }

        public int P
        {
            get;
            set;
        }
    }
}