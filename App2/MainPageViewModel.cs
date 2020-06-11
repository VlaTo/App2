using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Windows.UI;
using App2.InteractionContexts;
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

            rayTracer.AmbientColor = Colors.Beige;

            RayTracerRequest = new InteractionRequest<RayTracerRequestContext>();
            SaveCommand = new DelegateCommand(DoSaveCommand, _ => CanSave);
            TraceCommand = new DelegateCommand(DoTraceCommand);

            rayTracer.Progress += DoRayTracerProgress;

            /*TraceProgressObservable = Observable.FromEventPattern<TraceProgressEventArgs>(
                handler => rayTracer.Progress += handler,
                handler => rayTracer.Progress -= handler
            );*/
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

        private void DoRayTracerProgress(object sender, TraceProgressEventArgs e)
        {
            var content = new RayTracerRequestContext(e.Bitmap);
            RayTracerRequest.Raise(content, () => { });
        }
    }
}