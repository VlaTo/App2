using System;
using System.Numerics;
using Windows.UI;
using RayTracing;
using RayTracing.Extensions;

namespace App2
{
    public sealed class ViewModelLocator
    {
        private readonly Lazy<RayTracer> rayTracer;

        public MainPageViewModel MainPageModel => new MainPageViewModel(rayTracer.Value)
        {
            BitmapWidth = 640,
            BitmapHeight = 480
        };

        public ViewModelLocator()
        {
            rayTracer = new Lazy<RayTracer>(() =>
            {
                var material = new SurfaceData
                {
                    Color = Colors.Red.ToVector3(),
                    Medium = new Medium(0.67f, 0.0f),
                    Ka = 0.2f,
                    Kd = 0.5f,
                    Ks = 0.6f,
                    Kr = 0.0f,
                    Kt = 0.0f,
                    P = 30
                };

                var tracer = new RayTracer(640, 480)
                {
                    Scene = new Scene
                    {
                        AmbientColor = Colors.DarkCyan,
                        Camera = Camera.Set(
                            new Vector3(0.0f, 0.0f, -15.0f),
                            new Vector3(0.0f, 0.0f, 1.0f),
                            0.0f
                        ),
                        Figures =
                        {
                            new Sphere(new Vector3(0.0f, 0.0f, 0.0f), 8.0f, material)
                        }
                    }
                };

                return tracer;
            });
        }
    }
}