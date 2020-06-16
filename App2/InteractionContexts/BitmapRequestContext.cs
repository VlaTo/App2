using Windows.Graphics.Imaging;
using LibraProgramming.Windows.Interaction;

namespace App2.InteractionContexts
{
    public sealed class BitmapRequestContext : InteractionRequestContext
    {
        public SoftwareBitmap Bitmap
        {
            get;
        }

        public BitmapRequestContext(SoftwareBitmap bitmap)
        {
            Bitmap = bitmap;
        }
    }
}