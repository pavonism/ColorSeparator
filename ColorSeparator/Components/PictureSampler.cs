using ChartControl;
using FastBitmap;
using ImageProcessor;
using ImageProcessor.Colors;
using ImageProcessor.Interfaces;

namespace ColorSeparatorApp.Components
{
    internal class PictureSampler : SampleViewer, ISampleProvider
    {
        public CurveId? CurveId { get; private set; }
        protected bool isGeneratingView;
        protected ImageMng imageMng;

        public DirectBitmap Sample { get; set; }

        public event Action<ISampleProvider>? SampleChanged;

        public PictureSampler(ImageMng imageMng, CurveId? curveId = null)
        {
            this.CurveId = curveId;
            this.Dock = DockStyle.Fill;
            this.imageMng = imageMng;
            
            this.imageMng.Subscribe(this, this.CurveId);
        }

        private Image? sourceImage;
        public Image? SourceImage
        {
            get => sourceImage;
            set
            {
                sourceImage = value;
                GenerateSample();
            }
        }

        public Cmyk[,] CmyTable { get; private set; }
        public bool Busy { get; set; }

        private void GenerateSample()
        {
            if (SourceImage == null)
                return;

            Sample?.Dispose();
            Sample = new(Width, Height);

            if (SourceImage.Width < SourceImage.Height)
            {
                float scale = (float)Height / SourceImage.Height;
                var bitmapWidth = (int)(scale * SourceImage.Width);

                var margin = (Width - bitmapWidth) / 2;

                using (var g = Graphics.FromImage(Sample.Bitmap))
                {
                    g.DrawImage(SourceImage, margin, 0, bitmapWidth, Height);
                }
            }
            else
            {
                float scale = (float)Width / SourceImage.Width;
                var bitmapHeight = (int)(scale * SourceImage.Height);

                var margin = (Height - bitmapHeight) / 2;

                using (var g = Graphics.FromImage(Sample.Bitmap))
                {
                    g.DrawImage(SourceImage, 0, margin, Width, bitmapHeight);
                }
            }

            CmyTable = this.imageMng.CalculateCmyRepresentation(this.Sample);
            SampleChanged?.Invoke(this);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            GenerateSample();
        }

        public void LoadImage(string path)
        {
            SourceImage = Image.FromFile(path);
        }
    }
}
