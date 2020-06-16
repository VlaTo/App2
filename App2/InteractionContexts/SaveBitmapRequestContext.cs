using Windows.Graphics.Imaging;
using LibraProgramming.Windows.Interaction;

namespace App2.InteractionContexts
{
    public sealed class SaveBitmapRequestContext : InteractionRequestContext
    {
        public SoftwareBitmap Bitmap
        {
            get;
        }

        public SaveBitmapRequestContext(SoftwareBitmap bitmap)
        {
            Bitmap = bitmap;
        }
    }
}