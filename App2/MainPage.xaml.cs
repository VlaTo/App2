using App2.InteractionContexts;
using LibraProgramming.Windows.Interaction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
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
            if (e.Context is BitmapRequestContext context)
            {
                var source = new SoftwareBitmapSource();

                await source.SetBitmapAsync(context.Bitmap);

                PreviewImage.Source = source;
            }
        }

        private async void DoSaveBitmapRequest(object sender, InteractionRequestedEventArgs e)
        {
            if (e.Context is BitmapRequestContext context)
            {
                var bitmapEncoders = BitmapEncoder.GetEncoderInformationEnumerator();
                var picker = CreateFileSavePicker(bitmapEncoders, $"RayTrace-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}");
                var file = await picker.PickSaveFileAsync();

                if (null != file)
                {
                    var extension = Path.GetExtension(file.Path);

                    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var encoder = await CreateBitmapEncoderByFileExtension(bitmapEncoders, extension, stream);

                        encoder.SetSoftwareBitmap(context.Bitmap);

                        await encoder.FlushAsync();
                    }
                }
            }
        }

        private static FileSavePicker CreateFileSavePicker(
            IReadOnlyList<BitmapCodecInformation> bitmapEncoders,
            string suggestedFileName)
        {
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = suggestedFileName
            };

            for (var index = 0; index < bitmapEncoders.Count; index++)
            {
                var encoder = bitmapEncoders[index];
                var fileExtensions = encoder.FileExtensions.ToList();

                picker.FileTypeChoices.Add(encoder.FriendlyName, fileExtensions);
            }

            return picker;
        }

        private static Task<BitmapEncoder> CreateBitmapEncoderByFileExtension(
            IReadOnlyList<BitmapCodecInformation> bitmapEncoders, 
            string extension, 
            IRandomAccessStream stream)
        {
            for (var index = 0; index < bitmapEncoders.Count; index++)
            {
                var codecInformation = bitmapEncoders[index];

                if (codecInformation.FileExtensions.Any(extension.Equals))
                {
                    return BitmapEncoder.CreateAsync(codecInformation.CodecId, stream).AsTask();
                }
            }

            return Task.FromResult<BitmapEncoder>(null);
        }
    }
}
