using ImageProcessor.Interfaces;

namespace ImageProcessor
{
    public class ImageGenerator
    {
        private const int x = 500;
        private const int y = 500;

        private List<ISampleProvider> sampleProviders = new();
        private bool isBusy;

        private bool enabled = false;
        public bool Enabled
        {
            get => this.enabled;
            set
            {
                if(this.enabled != value)
                {
                    this.enabled = value;
                    UpdateProviders();
                }
            }
        }

        private int circlesCount = 12;
        public int CirclesCount
        {
            get => this.circlesCount;
            set
            {
                if (value != this.circlesCount)
                {
                    this.circlesCount = value;
                    UpdateProviders();
                }
            }
        }

        private float sParameter;
        public float SParameter
        {
            get => this.sParameter;
            set
            {
                if(value != this.sParameter)
                {
                    this.sParameter = value;
                    UpdateProviders();
                }
            }
        }

        public void UpdateProviders()
        {
            if (!Enabled || isBusy)
                return;

            isBusy = true;
            RunGenerateImage();
        }

        public void Subscribe(params ISampleProvider[] sampleProvider)
        {
            this.sampleProviders.AddRange(sampleProvider);
        }

        public Task RunGenerateImage()
        {
            return Task.Run(GenerateImage);
        }

        public void GenerateImage()
        {
            Bitmap image = new Bitmap(x, y);
            var center = new Point(x / 2, y / 2);

            int R = Math.Min(x, y) / 4;
            int r = (int)(2 * Math.PI * R / circlesCount);

            using (var g = Graphics.FromImage(image))
            {
                for (int i = 0; i < circlesCount; i++)
                {
                    int X = (int)(center.X + R * Math.Cos(i * 2 * Math.PI / circlesCount));
                    int Y = (int)(center.Y + R * Math.Sin(i * 2 * Math.PI / circlesCount));

                    g.DrawEllipse(Pens.Pink, X - r / 2, Y - r / 2, r, r);
                    g.FillEllipse(new SolidBrush(ColorFromHSV(360 / circlesCount * i, SParameter, 1)), X - r / 2, Y - r / 2, r, r);
                }
            }

            foreach (var provider in sampleProviders)
            {
                provider.SourceImage = image;
            }
            isBusy = false;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
    }
}
