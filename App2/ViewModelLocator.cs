using System;
using RayTracing;

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
            rayTracer = new Lazy<RayTracer>(() => new RayTracer(640, 480));
        }
    }
}