using ImageProcessor;

namespace ColorSeparatorApp
{
    internal class SampleViewer : PictureBox, ISampleViewer
    {
        private DirectBitmap? directBitmapImage;

        public void PutImage(DirectBitmap directBitmap)
        {
            this.directBitmapImage?.Dispose();
            this.directBitmapImage = directBitmap;

            this.Image = this.directBitmapImage.Bitmap;
        }

    }
}
