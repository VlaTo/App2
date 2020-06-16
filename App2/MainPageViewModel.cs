using App2.InteractionContexts;
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

        private void DoTraceCommand(object obj)
        {
            var disposable = rayTracer.Trace.Subscribe(
                e => RayTracerRequest.Raise(new BitmapRequestContext(e.Bitmap)),
                () => CanSave = true
            );
        }
    }
}