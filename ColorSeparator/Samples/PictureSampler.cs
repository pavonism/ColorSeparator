using ChartControl;
using FastBitmap;
using ImageProcessor;
using ImageProcessor.Colors;
using ImageProcessor.Interfaces;

namespace ColorSeparatorApp.Samples
{
    /// <summary>
    /// Przechowuje próbkę zdjęcia - jego pomniejszoną wersję w celu zwiększenia wydajności obliczeń
    /// </summary>
    internal class PictureSampler : PictureBox, ISampleProvider
    {
        #region Fields and Properties
        protected ImageMng? imageMng;
        /// <summary>
        /// Przetworzony (wyświetlany) obraz
        /// </summary>
        private DirectBitmap? directBitmapImage;

        /// <summary>
        /// Krzywa (kolor), który chce reprezentować Sampler przy wyświetleniu
        /// </summary>
        public CurveId? CurveId { get; private set; }
        /// <summary>
        /// Kolory próbki w reprezentacji CMY
        /// </summary>
        public Cmyk[,] CmyTable { get; private set; }
        /// <summary>
        /// Określa, czy próbka jest przetwarzana
        /// </summary>
        public bool Busy { get; set; }
        /// <summary>
        /// Próbka, która jest poddawana przetworzeniu. Stanowi pomniejszoną kopię <see cref="SourceImage"/>
        /// </summary>
        public DirectBitmap Sample { get; set; }

        private string imagePath;
        public string ImagePath
        {
            get => imagePath;
            set
            {
                SourceImage = Image.FromFile(value);
                imagePath = value;
            }
        }

        /// <summary>
        /// Obraz źródłowy
        /// </summary>
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
        #endregion

        #region Events and Handlers
        public event Action<ISampleProvider>? SampleChanged;

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            GenerateSample();
        }
        #endregion

        #region Initializing
        public PictureSampler(ImageMng? imageMng = null, CurveId? curveId = null)
        {
            CurveId = curveId;
            this.Dock = DockStyle.Fill;
            this.imageMng = imageMng;

            this.imageMng?.Subscribe(this, CurveId);
        }
        #endregion

        #region Rendering
        private void GenerateSample()
        {
            if (SourceImage == null)
                return;

            Sample?.Dispose();
            Sample = new(Width, Height);

            if (imageMng == null)
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

            if (imageMng != null)
                CmyTable = imageMng.CalculateCmyRepresentation(Sample);
            else
                PutImage(Sample);
            SampleChanged?.Invoke(this);
        }

        private void Fill()
        {
            using (var g = Graphics.FromImage(Sample.Bitmap))
            {
                g.Clear(Color.Black);
            }
        }

        /// <summary>
        /// Funkcja interfejsu, umożliwia <see cref="ImageMng"/> zwrócenie przetworzonego zdjęcia
        /// </summary>
        public void PutImage(DirectBitmap directBitmap)
        {
            directBitmapImage?.Dispose();
            directBitmapImage = directBitmap;

            Image = directBitmapImage.Bitmap;
        }
        #endregion
    }
}
