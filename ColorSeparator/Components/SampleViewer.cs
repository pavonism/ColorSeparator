using FastBitmap;
using ImageProcessor.Interfaces;

namespace ColorSeparatorApp.Components
{
    internal class SampleViewer : PictureBox, ISampleViewer
    {
        private DirectBitmap? directBitmapImage;

        public void PutImage(DirectBitmap directBitmap)
        {
            directBitmapImage?.Dispose();
            directBitmapImage = directBitmap;

            Image = directBitmapImage.Bitmap;
        }
    }
}
