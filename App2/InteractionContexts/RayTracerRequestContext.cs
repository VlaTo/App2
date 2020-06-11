using Windows.Graphics.Imaging;
using LibraProgramming.Windows.Interaction;

namespace App2.InteractionContexts
{
    public sealed class RayTracerRequestContext : InteractionRequestContext
    {
        public SoftwareBitmap Bitmap
        {
            get;
        }

        public RayTracerRequestContext(SoftwareBitmap bitmap)
        {
            Bitmap = bitmap;
        }
    }
}