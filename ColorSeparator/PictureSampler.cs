using ImageProcessor;

namespace ColorSeparatorApp
{
    internal class PictureSampler : SampleViewer, ISampleProvider
    {
        public Bitmap Sample { get; set; }

        public event Action<Bitmap>? SampleChanged;
        public event Action<Bitmap>? SampleSizeChanged;

        public PictureSampler()
        {
            Sample = new(this.Width, this.Height);
        }

        private Image? sourceImage;
        public Image? SourceImage
        {
            get => this.sourceImage;
            set
            {
                this.sourceImage = value;

                GenerateSample();
                this.SampleChanged?.Invoke(this.Sample);
            }
        }

        private void GenerateSample()
        {
            if (SourceImage == null)
                return;

            this.Sample.Dispose();
            this.Sample = new(this.Width, this.Height);

            if(SourceImage.Width < SourceImage.Height)
            {
                float scale = (float)this.Height / SourceImage.Height;
                var bitmapWidth = (int)(scale * SourceImage.Width);

                var margin = (this.Width - bitmapWidth) / 2;

                using(var g = Graphics.FromImage(this.Sample))
                {
                    g.DrawImage(SourceImage, margin, 0, bitmapWidth, this.Height);
                }
            } 
            else
            {
                float scale = (float)this.Width / SourceImage.Width;
                var bitmapHeight = (int)(scale * SourceImage.Height);

                var margin = (this.Height - bitmapHeight) / 2;

                using (var g = Graphics.FromImage(this.Sample))
                {
                    g.DrawImage(SourceImage, 0, margin, this.Width, bitmapHeight);
                }
            }

        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            GenerateSample();
            this.SampleSizeChanged?.Invoke(this.Sample);
        }

        public void LoadImage(string path)
        {
            SourceImage = Image.FromFile(path);
        }
    }
}
