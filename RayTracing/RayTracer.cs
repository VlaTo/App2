using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using RayTracing.Extensions;

namespace RayTracing
{
    public sealed class TraceProgressEventArgs : EventArgs
    {
        public SoftwareBitmap Bitmap
        {
            get;
        }

        public TraceProgressEventArgs(SoftwareBitmap bitmap)
        {
            Bitmap = bitmap;
        }
    }

    public sealed class RayTracer
    {
        private readonly int bitmapWidth;
        private readonly int bitmapHeight;

        public event EventHandler<TraceProgressEventArgs> Progress;

        public IObservable<TraceProgressEventArgs> Trace => Observable.Defer(DoTrace);

        public Color AmbientColor
        {
            get;
            set;
        }

        public RayTracer(int bitmapWidth, int bitmapHeight)
        {
            this.bitmapWidth = bitmapWidth;
            this.bitmapHeight = bitmapHeight;
        }

        private Task TraceAsync(IObserver<TraceProgressEventArgs> observer, CancellationToken cancellationToken)
        {
            var bitmap = new WriteableBitmap(bitmapWidth, bitmapHeight);
            var pixelBuffer = bitmap.PixelBuffer.ToArray();
            
            var chunkSize = (bitmap.PixelWidth * bitmap.PixelHeight) / 100;
            
            var stride = bitmap.PixelWidth * 4;
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;

            void Report()
            {
                var source = SoftwareBitmap.CreateCopyFromBuffer(
                    pixelBuffer.AsBuffer(),
                    BitmapPixelFormat.Bgra8,
                    width,
                    height,
                    BitmapAlphaMode.Premultiplied
                );

                observer.OnNext(new TraceProgressEventArgs(source));
            }

            return Task.Run(() =>
            {
                try
                {
                    var currentChunkSize = 0;

                    for (var line = 0; line < height; line++)
                    {
                        var offset = stride * line;

                        for (var column = 0; column < width; column++)
                        {
                            var index = offset + (column * 4);

                            cancellationToken.ThrowIfCancellationRequested();

                            TracePixel(pixelBuffer, index);

                            if (chunkSize < currentChunkSize++)
                            {
                                currentChunkSize = 0;
                                Report();
                            }
                        }
                    }

                    Report();
                }
                catch(Exception exception)
                {
                    observer.OnError(exception);
                }
                finally
                {
                    pixelBuffer = null;
                    observer.OnCompleted();
                }
            }, cancellationToken);
        }

        private IObservable<TraceProgressEventArgs> DoTrace()
        {
            Debug.WriteLine("Raytrace start");

            var subject = new Subject<TraceProgressEventArgs>();

            TraceAsync(subject, CancellationToken.None).RunAndForget();

            return subject.AsObservable();
        }

        private void TracePixel(byte[] pixelBuffer, int position)
        {
            //pixelBuffer[position + 0] = color[0]; // B
            //pixelBuffer[position + 1] = color[1]; // G
            //pixelBuffer[position + 2] = color[2]; // R
            //pixelBuffer[position + 3] = 255; // A
            
            pixelBuffer[position + 0] = AmbientColor.B; // B
            pixelBuffer[position + 1] = AmbientColor.G; // G
            pixelBuffer[position + 2] = AmbientColor.R; // R
            pixelBuffer[position + 3] = AmbientColor.A; // A
        }

        private void Report(byte[] pixelBuffer, int width, int height)
        {
            var source = SoftwareBitmap.CreateCopyFromBuffer(
                pixelBuffer.AsBuffer(),
                BitmapPixelFormat.Bgra8,
                width,
                height,
                BitmapAlphaMode.Premultiplied
            );

            DoProgress(new TraceProgressEventArgs(source));
        }

        private void DoProgress(TraceProgressEventArgs e)
        {
            var handler = Progress;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }
    }
}
