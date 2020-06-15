using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml.Controls;
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
            var memory = new Memory<byte>(bitmap.PixelBuffer.ToArray());
            
            var chunkSize = (bitmap.PixelWidth * bitmap.PixelHeight) / 100;
            
            var stride = bitmap.PixelWidth * 4;
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;

            void Report()
            {
                var source = SoftwareBitmap.CreateCopyFromBuffer(
                    memory.ToArray().AsBuffer(),
                    BitmapPixelFormat.Bgra8,
                    width,
                    height,
                    BitmapAlphaMode.Premultiplied
                );

                observer.OnNext(new TraceProgressEventArgs(source));
            }

            var left = 100;
            var top = 100;
            var right = 200;
            var bottom = 200;

            var hw = width / 2.0f;
            var hh = height / 2.0f;
            //var origin = new Vector3(-hw, -hh, -10.0f);

            var col = GetPartitionsCount(width, 16);
            var rows = GetPartitionsCount(height, 16);
            var areas = new Queue<Rect>();

            for (int row = 0; row < height; row += 16)
            {
                var h = Math.Min(16.0d, height - row);

                for (int column = 0; column < width; column += 16)
                {
                    var w = Math.Min(16.0d, width - column);
                    var area = new Rect(column, row, w, h);

                    areas.Enqueue(area);
                }
            }

            return Task.Run(() =>
            {
                try
                {
                    var currentChunkSize = 0;
                    var colors = new[] {Colors.DarkCyan, Colors.CadetBlue, Colors.Aqua};
                    var pos = 0;

                    while (0 < areas.Count)
                    {
                        var area = areas.Dequeue();
                        var origin = new Vector3(-hw + (float) area.X, -hh + (float) area.Y, -10.0f);


                        for (var line = (int) area.Top; line < (int) area.Bottom; line++)
                        {
                            var offset = stride * line;

                            for (var column = (int) area.Left; column < (int) area.Right; column++)
                            {
                                var index = offset + (column * 4);

                                cancellationToken.ThrowIfCancellationRequested();

                                TracePixel(memory.Span, index, ref origin, ref colors[pos]);

                                if (chunkSize < currentChunkSize++)
                                {
                                    currentChunkSize = 0;
                                    Report();
                                }
                            }

                            origin.X = -hw;
                            origin.Y += 1.0f;
                        }

                        pos = (pos + 1) % colors.Length;
                    }

                    Report();
                }
                catch(Exception exception)
                {
                    observer.OnError(exception);
                }
                finally
                {
                    memory = null;
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

        private void TracePixel(Span<byte> pixelBuffer, int position, ref Vector3 origin, ref Color color)
        {
            pixelBuffer[position + 0] = color.B; // B
            pixelBuffer[position + 1] = color.G; // G
            pixelBuffer[position + 2] = color.R; // R
            pixelBuffer[position + 3] = color.A; // A
        }

        private int GetPartitionsCount(int value, int size)
        {
            var reminder = 0 < (value % size) ? 1 : 0;
            return (value / size) + reminder;
        }

        /*private void Report(byte[] pixelBuffer, int width, int height)
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
        }*/
    }
}
