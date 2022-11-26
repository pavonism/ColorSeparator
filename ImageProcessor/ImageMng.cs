using ChartControl;

namespace ImageProcessor
{
    public class ImageMng
    {
        Charter colorCurves;
        Bitmap? image;
        float retraction = 1;
        Cmyk[,] cmykRepresentation;
        Bitmap magentaImgage;

        public ImageMng(Charter colorCurves)
        {
            this.colorCurves = colorCurves;
            this.colorCurves.CurveChanged += CurveChangedHandler;
            CMYKCurveGenerator.GenerateSample(colorCurves);
        }

        private void CurveChangedHandler(object obj)
        {
        }

        public Bitmap LoadImage(string path)
        {
            this.image = new Bitmap(Image.FromFile(path));
            this.cmykRepresentation = new Cmyk[this.image.Width, this.image.Height];

            this.colorCurves.Reset();
            CalculateCmykRepresentation();
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

        public Bitmap GenerateMagentaImage()
        {
            if (this.image == null)
                throw new NullReferenceException();

            this.magentaImgage = new Bitmap(this.image.Width, this.image.Height);

            for (int x = 0; x < this.image.Width; x++)
            {
                for (int y = 0; y < this.image.Height; y++)
                {
                    var color = MagentaToRgb(cmykRepresentation[x, y].M);
                    this.magentaImgage.SetPixel(x, y, color);
                }
            }

            return this.magentaImgage;
        }

        public Cmyk RgbToCmyk(Color color)
        {
            var cmy = ConvertToCmy(color);
            CmyToCmyk(cmy);
            return cmy;
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

        public void CmyToCmyk(Cmyk cmyk)
        {
            var kprime = cmyk.Min * retraction;
            cmyk.C = cmyk.C - kprime + colorCurves.GetCurveValueAt(CurveId.Cyan, kprime);
            cmyk.M = cmyk.M - kprime + colorCurves.GetCurveValueAt(CurveId.Magenta, kprime);
            cmyk.Y = cmyk.Y - kprime + colorCurves.GetCurveValueAt(CurveId.Yellow, kprime);
            cmyk.K = colorCurves.GetCurveValueAt(CurveId.Black, kprime);
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
                1,
                1,
                (int)((1 - yellowColor) * 255)
            );
        }

        public Color CyanToRgb(float cyanColor)
        {
            return Color.FromArgb
            (
                (int)((1 - cyanColor) * 255),
                1,
                1
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
