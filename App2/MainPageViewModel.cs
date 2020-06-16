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

namespace App2
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly RayTracer rayTracer;
        private CancellationTokenSource cancellationTokenSource;
        private bool canSave;

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

        public InteractionRequest<RayTracerRequestContext> RayTracerRequest
        {
            get;
        }

        public MainPageViewModel(RayTracer rayTracer)
        {
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

        private void DoSaveCommand(object parameter)
        {
            ;
        }

        private void DoTraceCommand(object obj)
        {
            Debug.WriteLine("DoTraceCommand start");

            var disposable = rayTracer.Trace.Subscribe(e =>
            {
                Debug.WriteLine("DoTraceCommand onNext");

                var content = new RayTracerRequestContext(e.Bitmap);

                RayTracerRequest.Raise(content, () =>
                {
                    Debug.WriteLine("Raytrace callback");
                });
            });

            Debug.WriteLine("DoTraceCommand done");

            /*cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await rayTracer.TraceAsync(cancellationTokenSource.Token);
            }
            finally
            {
                cancellationTokenSource = null;
            }*/
        }

        private void DoRayTracerProgress(object sender, TraceProgress e)
        {
            var content = new RayTracerRequestContext(e.Bitmap);
            RayTracerRequest.Raise(content, () => { });
        }
    }
}