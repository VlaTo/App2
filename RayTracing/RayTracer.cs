using LibraProgramming.Windows.Core.Extensions;
using System;
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

        public IObservable<TraceProgress> Trace => Observable.Defer(DoTrace);

        public Scene Scene
        {
            get;
            set;
        }

        public RayTracer(int bitmapWidth, int bitmapHeight)
        {
            this.bitmapWidth = bitmapWidth;
            this.bitmapHeight = bitmapHeight;
        }

        private Task TraceAsync(IObserver<TraceProgress> observer, CancellationToken cancellationToken)
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

                observer.OnNext(new TraceProgress(source));
            }

            return Task.Run(() =>
            {
                try
                {
                    var currentChunkSize = 0;
                    var ray = new Ray(Vector3.Zero, Vector3.Zero);

                    for (var line = 0; line < height; line++)
                    {
                        var offset = stride * line;

                        for (var column = 0; column < width; column++)
                        {
                            var index = offset + (column * 4);

                            cancellationToken.ThrowIfCancellationRequested();

                            Camera(column, line, ref ray);

                            TracePixel(pixelBuffer, index, Scene.AmbientColor);

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

        private IObservable<TraceProgress> DoTrace()
        {
            Debug.WriteLine("Raytrace start");

            var subject = new Subject<TraceProgress>();

            TraceAsync(subject, CancellationToken.None).RunAndForget();

            return subject.AsObservable();
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
            {
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
