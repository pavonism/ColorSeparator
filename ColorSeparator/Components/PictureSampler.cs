using ImageProcessor.Interfaces;

namespace ColorSeparatorApp.Components
{
    internal class PictureSampler : SampleViewer, ISampleProvider
    {
        public Bitmap Sample { get; set; }

        public event Action<Bitmap>? SampleChanged;
        public event Action<Bitmap>? SampleSizeChanged;

        public PictureSampler()
        {
            Sample = new(Width, Height);
        }

        private Image? sourceImage;
        public Image? SourceImage
        {
            get => sourceImage;
            set
            {
                sourceImage = value;

                GenerateSample();
                SampleChanged?.Invoke(Sample);
            }
        }

        private void GenerateSample()
        {
            if (SourceImage == null)
                return;

            Sample.Dispose();
            Sample = new(Width, Height);

            if (SourceImage.Width < SourceImage.Height)
            {
                float scale = (float)Height / SourceImage.Height;
                var bitmapWidth = (int)(scale * SourceImage.Width);

                var margin = (Width - bitmapWidth) / 2;

                using (var g = Graphics.FromImage(Sample))
                {
                    g.DrawImage(SourceImage, margin, 0, bitmapWidth, Height);
                }
            }
            else
            {
                float scale = (float)Width / SourceImage.Width;
                var bitmapHeight = (int)(scale * SourceImage.Height);

                var margin = (Height - bitmapHeight) / 2;

                using (var g = Graphics.FromImage(Sample))
                {
                    g.DrawImage(SourceImage, 0, margin, Width, bitmapHeight);
                }
            }

        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            GenerateSample();
            SampleSizeChanged?.Invoke(Sample);
        }

        public void LoadImage(string path)
        {
            SourceImage = Image.FromFile(path);
        }
    }
}
