using System;
using System.Diagnostics;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Windows.UI;
using App2.InteractionContexts;
using LibraProgramming.Windows.Core;
using LibraProgramming.Windows.Interaction;
using RayTracing;
using System;
using System.Reactive.Linq;
using Windows.UI;

namespace App2
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly RayTracer rayTracer;
        private bool canSave;
        private double bitmapWidth;
        private double bitmapHeight;

        public bool CanSave
        {
            get => canSave;
            set => SetValue(ref canSave, value);
        }

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
            this.rayTracer = rayTracer;

            rayTracer.Scene = new Scene
            {
                AmbientColor = Colors.DarkCyan,
                Camera = new Camera(
                    new Vector3(0.0f, 0.0f, -15.0f),
                    new Vector3(0.0f, 0.0f, 1.0f),
                    new Vector3(0.0f, 1.0f, 0.0f)
                ),

            };

            rayTracer.Scene.Add(new Sphere(new Vector3(0.0f, 0.0f, 0.0f), 2.0f));

            RayTracerRequest = new InteractionRequest<RayTracerRequestContext>();
            SaveCommand = new DelegateCommand(DoSaveCommand, _ => CanSave);
            TraceCommand = new DelegateCommand(DoTraceCommand);
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

            rayTracer.AmbientColor = Colors.DarkCyan;

            RayTracerRequest = new InteractionRequest<BitmapRequestContext>();
            SaveBitmapRequest = new InteractionRequest<BitmapRequestContext>();

            SaveCommand = new DelegateCommand(DoSaveCommand);
            TraceCommand = new DelegateCommand(DoTraceCommand);
        }

        private void DoSaveCommand(object parameter)
        {
            SaveBitmapRequest.Raise(new BitmapRequestContext(rayTracer.Bitmap));
        }

        private void DoRayTracerProgress(object sender, TraceProgress e)
        {
            var disposable = rayTracer.Trace.Subscribe(
                e => RayTracerRequest.Raise(new BitmapRequestContext(e.Bitmap)),
                () => CanSave = true
            );
        }
    }
}