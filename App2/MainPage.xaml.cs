using App2.InteractionContexts;
using LibraProgramming.Windows.Interaction;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace App2
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void DoRayTracerRequest(object sender, InteractionRequestedEventArgs e)
        {
            switch (e.Context)
            {
                case RayTracerRequestContext tracerRequestContext:
                {
                    await DoUpdateSourceImageAsync(tracerRequestContext, e.Callback);
                    break;
                }
            }
        }

        private async Task DoUpdateSourceImageAsync(RayTracerRequestContext context, Action callback)
        {
            var source = new SoftwareBitmapSource();

            await source.SetBitmapAsync(context.Bitmap);

            PreviewImage.Source = source;

            callback.Invoke();
        }
    }
}
