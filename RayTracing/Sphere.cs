using System;
using System.Numerics;

namespace RayTracing
{
    public sealed class Sphere : Figure
    {
        private float radius2;

        public Vector3 Center
        {
            get;
        }

        public float Radius
        {
            get;
        }

        public Sphere(Vector3 center, float radius)
        {
            radius2 = radius * radius;

            Center = center;
            Radius = radius;
        }

        public override Vector3 FindNormal(Vector3 point)
        {
            throw new System.NotImplementedException();
        }

        public override int Intersect(Ray ray, float distance)
        {
            var location = Center - ray.Origin;
            var l20c = Vector3.Distance(location, location);
            var tca = Vector3.Distance(location, ray.Direction);
            var t2hc = radius2 - l20c + tca * tca;

            if (0.0f >= t2hc)
            {
                return 0;
            }

            t2hc = (float) Math.Sqrt(t2hc);

            throw new NotImplementedException();
        }

    }
}