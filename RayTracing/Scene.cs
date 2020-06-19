using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI;
using RayTracing.Extensions;

namespace RayTracing
{
    public class Scene
    {
        public Color AmbientColor
        {
            get;
            set;
        }

        public Camera Camera
        {
            get;
            set;
        }

        public List<Figure> Figures
        {
            get;
        }

        public Scene()
        {
            Figures = new List<Figure>();
        }

        public Figure Intersect(Ray ray, ref float distance)
        {
            var targetDistance = float.PositiveInfinity;
            Figure target = null;

            for (var index = 0; index < Figures.Count; index++)
            {
                var figure = Figures[index];

                if (figure.TryIntersect(ray, ref distance) && distance < targetDistance)
                {
                    targetDistance = distance;
                    target = figure;
                }
            }

            distance = targetDistance;

            return target;
        }

        public Vector3 ShadeBackground(Ray ray) => AmbientColor.ToVector3();
    }
}