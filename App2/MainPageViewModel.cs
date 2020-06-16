using App2.InteractionContexts;
using LibraProgramming.Windows.Core;
using LibraProgramming.Windows.Interaction;
using RayTracing;
using System;
using System.Reactive.Linq;

namespace App2
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly RayTracer rayTracer;
        private double bitmapWidth;
        private double bitmapHeight;

        public DelegateCommand SaveCommand
        {
            get;
        }

        public DelegateCommand TraceCommand
        {
            get;
        }

        public InteractionRequest<BitmapRequestContext> RayTracerRequest
        {
            get;
        }

        public InteractionRequest<BitmapRequestContext> SaveBitmapRequest
        {
            get;
        }

        public double BitmapWidth
        {
            get => bitmapWidth;
            set => SetValue(ref bitmapWidth, value);
        }

        public double BitmapHeight
        {
            get => bitmapHeight;
            set => SetValue(ref bitmapHeight, value);
        }

        public MainPageViewModel(RayTracer rayTracer)
        {
            this.rayTracer = rayTracer;

            /*rayTracer.Scene = new Scene
            {
                AmbientColor = Colors.DarkCyan,
                Camera = new Camera(
                    new Vector3(0.0f, 0.0f, -15.0f),
                    new Vector3(0.0f, 0.0f, 1.0f),
                    new Vector3(0.0f, 1.0f, 0.0f)
                ),

            };

            rayTracer.Scene.Add(new Sphere(new Vector3(0.0f, 0.0f, 0.0f), 2.0f));*/

            RayTracerRequest = new InteractionRequest<BitmapRequestContext>();
            SaveBitmapRequest = new InteractionRequest<BitmapRequestContext>();
            SaveCommand = new DelegateCommand(DoSaveCommand);
            TraceCommand = new DelegateCommand(DoTraceCommand);
        }

        private void DoSaveCommand(object parameter)
        {
            SaveBitmapRequest.Raise(new BitmapRequestContext(rayTracer.Bitmap));
        }

        private void DoTraceCommand(object parameter)
        {
            var disposable = rayTracer.Trace.Subscribe(
                progress => RayTracerRequest.Raise(new BitmapRequestContext(progress.Bitmap))
            );
        }
    }
}