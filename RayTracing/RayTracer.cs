using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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
        internal readonly Medium Air = new Medium(1.0f, 0.0f);
        internal const float Threshold = 0.01f;

        private readonly int bitmapWidth;
        private readonly int bitmapHeight;
        private CancellationTokenSource cts;
        private Task traceTask;

        public IObservable<TraceProgress> Trace => Observable.Defer(DoTrace);

        public bool IsTracing => TaskStatus.RanToCompletion == traceTask.Status;

        public SoftwareBitmap Bitmap
        {
            get;
            private set;
        }

        public Scene Scene
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

        private IObservable<TraceProgress> DoTrace()
        {
            Debug.WriteLine("Raytrace start");

            var subject = new Subject<TraceProgress>();

            cts = new CancellationTokenSource();
            traceTask = TraceAsync(subject, cts.Token);

            return subject.AsObservable();
        }

        private Task TraceAsync(IObserver<TraceProgress> observer, CancellationToken cancellationToken)
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

            var areas = new List<Area>();
            var canvas = new Dimension(width, height);
            var cell = new Dimension(16);
            
            for (var y = 0; y < canvas.Height; y += cell.Width)
            {
                var h = Min.From(cell.Height, height - y);

                for (var x = 0; x < canvas.Width; x += cell.Height)
                {
                    var w = Min.From(cell.Width, width - x);
                    var area = new Area(x, y, w, h);

                    areas.Add(area);
                }
            }

            return Task.Factory.StartNew(() =>
                {
                    //Vector3 Eye = Vector3.Zero;
                    //Vector3 EyeDirection = Vector3.UnitZ;
                    //Vector3 Vx = Vector3.UnitX;
                    //Vector3 Vy = Vector3.UnitY;

                    try
                    {

                        var center = new PointF(width / 2.0f, height / 2.0f);
                        //var targets = areas.ToArray();
                        var ray = new Ray(Vector3.Zero, Vector3.Zero);
                        var context = new TraceContext(Scene.AmbientColor.ToVector3());

                        for (var index = 0; index < areas.Count; index++)
                        {
                            var area = areas[index];
                            //var color = colors[index % colors.Length];
                            var start = -center.X + area.X;
                            var origin = new PointF(start, center.Y - area.Y);

                            for (var line = area.Y; line < area.Bottom; line++)
                            {
                                var offset = stride * line;

                                for (var column = area.X; column < area.Right; column++)
                                {
                                    var position = offset + (column * 4);

                                    ray.Move(Scene.Camera, ref origin);

                                    var color = TraceRay(context, Air, 1.0f, ray);

                                    PutPixel(ref pixelBuffer, position, ref color);

                                    origin.X++;
                                }

                                origin.X = start;
                                origin.Y--;
                            }

                            cancellationToken.ThrowIfCancellationRequested();

                            var source = CreateBitmap();

                            observer.OnNext(new TraceProgress(source));
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

        private static void PutPixel(ref byte[] pixelBuffer, int position, ref Color color)
        {
            pixelBuffer[position + 0] = color.B; // B
            pixelBuffer[position + 1] = color.G; // G
            pixelBuffer[position + 2] = color.R; // R
            pixelBuffer[position + 3] = color.A; // A
        }

        private Color TraceRay(TraceContext context, Medium medium, float weight, Ray ray)
        {
            var distance = float.PositiveInfinity;
            var figure = Scene.Intersect(ray, ref distance);
            Vector3 color;

            if (null != figure)
            {
                color = Shade(context, medium, weight, ray.Point(distance), ray.Direction, figure);

                if (medium.Betta > Threshold)
                {
                    color *= (float) Math.Exp(-distance * medium.Betta);
                }
            }
            else
            {
                color = Scene.ShadeBackground(ray);
            }

            return color.ToColor();
        }

        private Vector3 Shade(TraceContext context, Medium medium, float weight, Vector3 point, Vector3 view, Figure figure)
        {
            var ray = new Ray(point, Vector3.Zero);
            var info = figure.FindTexture(ref point);
            var entering = 1;

            if (null == info)
            {
                return context.AmbientColor;
            }

            var vn = Vector3.Dot(view, info.Normal);

            if (0.0f < vn)
            {
                info.Normal = -info.Normal;
                vn = -vn;
                entering = 0;
            }

            ray.Origin = point;

            var color = context.AmbientColor * info.Data.Color * info.Data.Ka;

            //

            return color;
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

        /// <summary>
        /// 
        /// </summary>
        private sealed class TraceContext
        {
            public Vector3 AmbientColor
            {
                get;
            }

            public TraceContext(Vector3 ambientColor)
            {
                AmbientColor = ambientColor;
            }
        }
    }
}
