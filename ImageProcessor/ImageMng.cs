using ChartControl;

namespace ImageProcessor
{
    public class ImageMng
    {
        Charter colorCurves;
        Bitmap? image;
        float retraction = 1;
        Cmyk[,] cmykRepresentation;
        Bitmap currentImage;
        CurveId currentColor = CurveId.Cyan;

        public event Action? ImageChanged;

        public ImageMng(Charter colorCurves)
        {
            this.colorCurves = colorCurves;
            this.colorCurves.CurveChanged += CurveChangedHandler;
            CMYKCurveGenerator.GenerateSample(colorCurves);
        }

        private void CurveChangedHandler(object obj)
        {
            CalculateCmykRepresentation();
            GenerateSeparateImage(this.currentColor);
            this.ImageChanged?.Invoke();
        }

        public Bitmap LoadImage(string path)
        {
            this.image = new Bitmap(Image.FromFile(path));
            this.cmykRepresentation = new Cmyk[this.image.Width, this.image.Height];

            this.colorCurves.Reset();
            CMYKCurveGenerator.GenerateSample(colorCurves);
            CalculateCmykRepresentation();
            this.currentImage = new Bitmap(this.image.Width, this.image.Height);
            return this.image;
        }

        private void CalculateCmykRepresentation()
        {
            if (this.image == null)
                throw new NullReferenceException();

            for (int x = 0; x < this.image.Width; x++)
            {
                for (int y = 0; y < this.image.Height; y++)
                {
                    this.cmykRepresentation[x, y] = RgbToCmyk(this.image.GetPixel(x, y));
                }
            }
        }

        private Color GetCmykColorInRgb(int x, int y, CurveId curve)
        {
            switch (curve)
            {
                case CurveId.Cyan:
                    return CyanToRgb(cmykRepresentation[x, y].C);
                case CurveId.Magenta:
                    return MagentaToRgb(cmykRepresentation[x, y].M);
                case CurveId.Yellow:
                    return YellowToRgb(cmykRepresentation[x, y].Y);
                case CurveId.Black:
                    return BlackToRgb(cmykRepresentation[x, y].K);
            }

            return Color.Empty;
        }

        public Bitmap? GenerateSeparateImage(CurveId curve)
        {
            if (this.image == null)
                return null;

            this.currentColor = curve;

            for (int x = 0; x < this.image.Width; x++)
            {
                for (int y = 0; y < this.image.Height; y++)
                {
                    var color = GetCmykColorInRgb(x, y, curve);
                    this.currentImage.SetPixel(x, y, color);
                }
            }

            return this.currentImage;
        }

        public Cmyk RgbToCmyk(Color color)
        {
            var cmy = ConvertToCmy(color);
            return CmyToCmyk(cmy);
        }

        public Cmyk ConvertToCmy(Color color)
        {
            return new Cmyk()
            {
                C = 1 - (float)color.R / 255,
                M = 1 - (float)color.G / 255,
                Y = 1 - (float)color.B / 255,
            };
        }

        public Cmyk CmyToCmyk(Cmyk cmyk)
        {
            var kprime = cmyk.Min * retraction;
            var res = new Cmyk()
            {
                C = cmyk.C - kprime + colorCurves.GetCurveValueAt(CurveId.Cyan, kprime),
                M = cmyk.M - kprime + colorCurves.GetCurveValueAt(CurveId.Magenta, kprime),
                Y = cmyk.Y - kprime + colorCurves.GetCurveValueAt(CurveId.Yellow, kprime),
                K = colorCurves.GetCurveValueAt(CurveId.Black, kprime)
            };

            res.CutValues();
            return res;
        }

        public Color MagentaToRgb(float magentaColor)
        {
            return Color.FromArgb
            (
                255,
                (int)((1 - magentaColor) * 255),
                255
            );
        }

        public Color YellowToRgb(float yellowColor)
        {
            return Color.FromArgb
            (
                255,
                255,
                (int)((1 - yellowColor) * 255)
            );
        }

        public Color CyanToRgb(float cyanColor)
        {
            return Color.FromArgb
            (
                (int)((1 - cyanColor) * 255),
                255,
                255
            );
        }

        public Color BlackToRgb(float blackColor)
        {
            return Color.FromArgb
            (
                (int)((1 - blackColor) * 255),
                (int)((1 - blackColor) * 255),
                (int)((1 - blackColor) * 255)
            );
        }
    }
}
