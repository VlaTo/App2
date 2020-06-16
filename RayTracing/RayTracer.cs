using LibraProgramming.Windows.Core.Extensions;
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

namespace RayTracing
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TraceProgress
    {
        public SoftwareBitmap Bitmap
        {
            get;
        }

        public TraceProgress(SoftwareBitmap bitmap)
        {
            Bitmap = bitmap;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class RayTracer
    {
        internal readonly Vector3 Eye = Vector3.Zero;
        internal readonly Vector3 EyeDirection = Vector3.UnitZ;
        internal readonly Vector3 Vx = Vector3.UnitX;
        internal readonly Vector3 Vy = Vector3.UnitY;
        internal const float Threshold = 0.01f;

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
        private void Camera(int column, int line, ref Ray ray)
        {
            ray.Origin = Eye;
            ray.Direction = Vector3.Normalize(EyeDirection + Vx * column + Vy * line);
        }

        private Vector3 TraceRay(Medium medium, float weight, Ray ray)
        {
            var distance = float.PositiveInfinity;
            var figure = Scene.Intersect(ray, distance);
            var color = Vector3.Zero;

            if (null != figure)
            {
                color = Shade(medium, weight, ray.Point(distance), ray.Direction, figure);

                if (medium.Betta > Threshold)
                {
                    color *= (float) Math.Exp(-distance * medium.Betta);
                }
                else
                {
                    color = Scene.ShadeBackground(ray);
                }
            }

            return color;
        }

        private Vector3 Shade(Medium medium, float weight, Vector3 point, Vector3 view, Figure figure)
        {
            var ray = new Ray(point, Vector3.Zero);
            var texture = figure.FindTexture(point);

            if (null != texture)
            public TraceAreaContext(Area targetArea)
            {
                TargetArea = targetArea;
            }
                var vn = view & texture.N;
            }
            
            throw new NotImplementedException();
        }

        private static void TracePixel(byte[] pixels, int position, Color color)
        {
            pixels[position + 0] = color.B; // B
            pixels[position + 1] = color.G; // G
            pixels[position + 2] = color.R; // R
            pixels[position + 3] = color.A; // A
        }
    }
}
