using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private CancellationTokenSource cts;
        private Task traceTask;

        public IObservable<TraceProgressEventArgs> Trace => Observable.Defer(DoTrace);

        public bool IsTracing => TaskStatus.RanToCompletion == traceTask.Status;

        public SoftwareBitmap Bitmap
        {
            get;
            private set;
        }

        public Color AmbientColor
        {
            get;
            set;
        }

        public RayTracer(int bitmapWidth, int bitmapHeight)
        {
            this.bitmapWidth = bitmapWidth;
            this.bitmapHeight = bitmapHeight;

            traceTask = Task.CompletedTask;
            cts = default(CancellationTokenSource);
        }

        private IObservable<TraceProgressEventArgs> DoTrace()
        {
            Debug.WriteLine("Raytrace start");

            var subject = new Subject<TraceProgressEventArgs>();

            cts = new CancellationTokenSource();
            traceTask = TraceAsync(subject, cts.Token);

            return subject.AsObservable();
        }

        private Task TraceAsync(IObserver<TraceProgressEventArgs> observer, CancellationToken cancellationToken)
        {
            var bitmap = new WriteableBitmap(bitmapWidth, bitmapHeight);
            var pixelBuffer = bitmap.PixelBuffer.ToArray();
            
            var stride = bitmap.PixelWidth * 4;
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;

            SoftwareBitmap CreateBitmap() => SoftwareBitmap.CreateCopyFromBuffer(
                pixelBuffer.AsBuffer(),
                BitmapPixelFormat.Bgra8,
                width,
                height,
                BitmapAlphaMode.Premultiplied
            );

            var contexts = new List<TraceAreaContext>();
            var canvas = new Dimension(width, height);
            var cell = new Dimension(16);
            
            for (var y = 0; y < canvas.Height; y += cell.Width)
            {
                var h = Min.From(cell.Height, height - y);

                for (var x = 0; x < canvas.Width; x += cell.Height)
                {
                    var w = Min.From(cell.Width, width - x);
                    var area = new Area(x, y, w, h);

                    contexts.Add(new TraceAreaContext(area));
                }
            }

            return Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var center = new PointF(width / 2.0f, height / 2.0f);
                        var targets = contexts.ToArray();
                        var colors = new[] {Colors.Cyan, Colors.CadetBlue, Colors.DarkCyan};

                        for (var index = 0; index < targets.Length; index++)
                        {
                            var context = targets[index];
                            var color = colors[index % colors.Length];
                            var start = -center.X + context.TargetArea.X;
                            var origin = new Vector3(start, -center.Y + context.TargetArea.Y, -10.0f);

                            for (var line = context.TargetArea.Y; line < context.TargetArea.Bottom; line++)
                            {
                                var offset = stride * line;

                                for (var column = context.TargetArea.X; column < context.TargetArea.Right; column++)
                                {
                                    var position = offset + (column * 4);

                                    cancellationToken.ThrowIfCancellationRequested();

                                    TracePixel(ref pixelBuffer, position, ref origin, ref color);

                                    origin.X++;
                                }

                                origin.X = start;
                                origin.Y++;
                            }

                            var source = CreateBitmap();

                            observer.OnNext(new TraceProgressEventArgs(source));
                        }

                        Bitmap = CreateBitmap();
                    }
                    catch (Exception exception)
                    {
                        observer.OnError(exception);
                    }
                    finally
                    {
                        observer.OnCompleted();
                    }
                },
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Current
            );
        }

        private static void TracePixel(ref byte[] pixelBuffer, int position, ref Vector3 origin, ref Color color)
        {
            pixelBuffer[position + 0] = color.B; // B
            pixelBuffer[position + 1] = color.G; // G
            pixelBuffer[position + 2] = color.R; // R
            pixelBuffer[position + 3] = color.A; // A
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class TraceAreaContext
        {
            public Area TargetArea
            {
                get;
            }

            public TraceAreaContext(Area targetArea)
            {
                TargetArea = targetArea;
            }
        }
    }
}
