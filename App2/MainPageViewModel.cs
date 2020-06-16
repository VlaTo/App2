using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Windows.Storage.Pickers;
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

        public InteractionRequest<SaveBitmapRequestContext> SaveBitmapRequest
        {
            get;
        }

        public MainPageViewModel(RayTracer rayTracer)
        {
            this.rayTracer = rayTracer;

            rayTracer.AmbientColor = Colors.DarkCyan;

            RayTracerRequest = new InteractionRequest<RayTracerRequestContext>();
            SaveBitmapRequest = new InteractionRequest<SaveBitmapRequestContext>();

            SaveCommand = new DelegateCommand(DoSaveCommand);
            TraceCommand = new DelegateCommand(DoTraceCommand);
        }

        private void DoSaveCommand(object parameter)
        {
            SaveBitmapRequest.Raise(new SaveBitmapRequestContext(rayTracer.Bitmap));
        }

        private void DoTraceCommand(object obj)
        {
            var disposable = rayTracer.Trace.Subscribe(
                e => RayTracerRequest.Raise(new RayTracerRequestContext(e.Bitmap)),
                () => CanSave = true
            );
        }
    }
}