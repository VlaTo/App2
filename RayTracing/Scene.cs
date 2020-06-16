using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI;

namespace RayTracing
{
    public class Scene : IEnumerable<Figure>
    {
        private readonly List<Figure> figures;

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

        public Scene()
        {
            figures = new List<Figure>();
        }

        public void Add(Figure figure)
        {
            if (null == figure)
            {
                throw new ArgumentNullException(nameof(figure));
            }

            figures.Add(figure);
        }

        public IEnumerator<Figure> GetEnumerator() => figures.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Figure Intersect(Ray ray, float distance)
        {
            throw new NotImplementedException();
        }

        public Vector3 ShadeBackground(Ray ray) => AmbientColor.ToVector3();
    }
}