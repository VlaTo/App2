using System;
using System.Numerics;
using Windows.Graphics.Printing3D;
using Windows.UI.Xaml.Controls;

namespace RayTracing
{
    public sealed class Sphere : Figure
    {
        internal const float Threshold = 0.001f;

        private float radius2;

        public Vector3 Center
        {
            get;
        }

        public float Radius
        {
            get;
        }

        public Sphere(Vector3 center, float radius, SurfaceData material)
        {
            radius2 = radius * radius;

            Center = center;
            Radius = radius;
            Material = material;
        }

        public override Vector3 FindNormal(ref Vector3 point) => (point - Center) / Radius;

        public override bool TryIntersect(Ray ray, ref float distance)
        {
            var location = Center - ray.Origin;
            var l20c = Vector3.Dot(location, location);
            var tca = Vector3.Dot(location, ray.Direction);
            var t2hc = radius2 - l20c + tca * tca;
            var t2 = 0.0f;

            if (0.0f >= t2hc)
            {
                return false;
            }

            t2hc = (float) Math.Sqrt(t2hc);

            if (t2hc > tca)
            {
                distance = tca + t2hc;
                t2 = tca - t2hc;
            }
            else
            {
                distance = tca - t2hc;
                t2 = tca + t2hc;
            }

            if (Threshold > MathF.Abs(distance))
            {
                distance = t2;
            }

            return Threshold < distance;
        }
    }
}