using ChartControl;
using FastBitmap;
using ImageProcessor;
using ImageProcessor.Colors;
using ImageProcessor.Interfaces;
using System.IO;

namespace ColorSeparatorApp.Components
{
    internal class PictureSampler : SampleViewer, ISampleProvider
    {
        public CurveId? CurveId { get; private set; }
        protected bool isGeneratingView;
        protected ImageMng? imageMng;

        private string imagePath;
        public string ImagePath
        {
            get => this.imagePath;
            set
            {
                SourceImage = Image.FromFile(value);
                this.imagePath = value;
            }
        }
        public DirectBitmap Sample { get; set; }

        public event Action<ISampleProvider>? SampleChanged;

        public PictureSampler(ImageMng? imageMng = null, CurveId? curveId = null)
        {
            this.CurveId = curveId;
            this.Dock = DockStyle.Fill;
            this.imageMng = imageMng;
            
            this.imageMng?.Subscribe(this, this.CurveId);
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

            if (this.imageMng == null)
                Fill();

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

            if (this.imageMng != null)
                CmyTable = this.imageMng.CalculateCmyRepresentation(this.Sample);
            else
                PutImage(this.Sample);
            SampleChanged?.Invoke(this);
        }

        private void Fill()
        {
            using(var g = Graphics.FromImage(Sample.Bitmap)) 
            {
                g.Clear(Color.Black);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            GenerateSample();
        }
    }
}
