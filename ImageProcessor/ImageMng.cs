using ChartControl;
using FastBitmap;
using ImageProcessor.Colors;
using ImageProcessor.Interfaces;

namespace ImageProcessor
{
    public class ImageMng
    {
        #region Fields and Properties
        private ISampleProvider sampleProvider;
        private Charter colorCurves;
        private DirectBitmap? image;
        private Cmyk[,] cmykRepresentation;
        private float retraction;
        private bool isGenerating;
        private bool[] imageIsGenerating = new bool[4];

        public int RenderThreads { get; set; } = 50;
        public float Retraction
        {
            get => this.retraction;
            set
            {
                this.retraction = value;
                RunRegenerateImageAsync();
            }
        }
        #endregion Fields and Properties

        #region Events
        public event Action<CurveId?, Cmyk[,]>? ParametersChanged;

        private void SendParametersChanged(CurveId? curve = null)
        {
            if (this.image == null)
                return;

            Cmyk[,] currentCmykRepresentation = (Cmyk[,])this.cmykRepresentation.Clone();
            this.ParametersChanged?.Invoke(curve, currentCmykRepresentation);
        }
        #endregion Events

        #region Event Handlers
        private void SampleSizeChanged(Bitmap sample)
        {
            InitializeSamples(sample);
            SendParametersChanged();
        }

        private void SampleChangedHandler(Bitmap sample)
        {
            this.colorCurves.Reset();
            CMYKCurveGenerator.GenerateSample(colorCurves);
            InitializeSamples(sample);
        }

        private void CurveChangedHandler(CurveId? obj)
        {
            RunRegenerateImageAsync(obj);
        }
        #endregion Event Handlers

        #region Initializing
        public ImageMng(ISampleProvider sampleProvider, Charter colorCurves)
        {
            this.sampleProvider = sampleProvider;
            this.colorCurves = colorCurves;
            this.colorCurves.CurveChanged += CurveChangedHandler;
            this.sampleProvider.SampleChanged += SampleChangedHandler;
            this.sampleProvider.SampleSizeChanged += SampleSizeChanged;
        }

        private void InitializeSamples(Bitmap sample)
        {
            this.image?.Dispose();
            this.image = new(sample);
            this.cmykRepresentation = new Cmyk[this.image.Width, this.image.Height];

            CalculateCmykRepresentation();
            GenerateImage();
        }
        #endregion

        #region Generating Images
        private void RunRegenerateImageAsync(CurveId? curve = null)
        {
            if (!isGenerating)
            {
                isGenerating = true;
                ReGenerateImageAsync(curve);
            }
        }

        private Task ReGenerateImageAsync(CurveId? curve)
        {
            return Task.Run(() =>
            {
                CalculateCmykRepresentation();
                SendParametersChanged(curve);
                GenerateImage();
                isGenerating = false;
            });
        }

        private Task GeneratePixelsAsync(int start, int step, DirectBitmap bitmap, Cmyk[,] cmykRepresentation, CurveId? curve = null)
        {
            return Task.Run(
                () =>
                {
                    for (int x = start; x < start + step && x < bitmap.Width; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            
                            var color = curve.HasValue ? GetCmykColorInRgb(x, y, curve.Value, cmykRepresentation) : CmykToRgb(cmykRepresentation[x, y]);
                            bitmap.SetPixel(x, y, color);
                        }
                    }
                });
        }

        public Task GenerateImageAsync()
        {
            return Task.Run(() => GenerateImage());
        }

        public void GenerateImage()
        {
            if (this.image == null)
                return;

            DirectBitmap currentImage = new DirectBitmap(this.image.Width, this.image.Height);
            var rowsPerThread = (int)Math.Ceiling((float)this.image.Width / this.RenderThreads);
            List<Task> tasks = new();

            for (int i = 0; i < this.RenderThreads; i++)
            {
                tasks.Add(GeneratePixelsAsync(i * rowsPerThread, rowsPerThread, currentImage, this.cmykRepresentation));
            }

            Task.WaitAll(tasks.ToArray());
            this.sampleProvider.PutImage(currentImage);
        }

        public void RunGenerateSeparateImageAsync(ISampleViewer sampleViewer, CurveId curve, Cmyk[,]? cmykRepresentation)
        {
            if (!imageIsGenerating[(int)curve])
            {
                imageIsGenerating[(int)curve] = true;
                GenerateSeparateImageAsync(sampleViewer, curve, cmykRepresentation ?? this.cmykRepresentation);
            }
        }

        public Task GenerateSeparateImageAsync(ISampleViewer sampleViewer, CurveId curve, Cmyk[,] cmykRepresentation)
        {
            return Task.Run(() =>
            {
                GenerateSeparateImage(sampleViewer, curve, cmykRepresentation);
                imageIsGenerating[(int)curve] = false;
            });
        }

        public void GenerateSeparateImage(ISampleViewer sampleViewer, CurveId curve, Cmyk[,] cmykRepresentation)
        {
            if (this.image == null)
                return;

            var newImage = new DirectBitmap(this.image.Width, this.image.Height);
            var rowsPerThread = (int)Math.Ceiling((float)newImage.Width / this.RenderThreads);
            List<Task> tasks = new();

            for (int i = 0; i < this.RenderThreads; i++)
            {
                tasks.Add(GeneratePixelsAsync(i * rowsPerThread, rowsPerThread, newImage, cmykRepresentation, curve));
            }

            Task.WaitAll(tasks.ToArray());
            sampleViewer.PutImage(newImage);
        }
        #endregion Generating Images

        #region Color Convertion
        private void CalculateCmykRepresentation()
        {
            if (this.image == null)
                return;

            this.cmykRepresentation = new Cmyk[this.image.Width, this.image.Height];

            for (int x = 0; x < this.image.Width; x++)
            {
                for (int y = 0; y < this.image.Height; y++)
                {
                    this.cmykRepresentation[x, y] = RgbToCmyk(this.image.GetPixel(x, y));
                }
            }
        }

        private Color GetCmykColorInRgb(int x, int y, CurveId curve, Cmyk[,] cmykRepresentation)
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
            var kprime = cmyk.Min * Retraction;
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

        public Color CmykToRgb(Cmyk cmyk)
        {
            return Color.FromArgb
            (
                (int)((1 - cmyk.C) * (1 - cmyk.K) * 255),
                (int)((1 - cmyk.M) * (1 - cmyk.K) * 255),
                (int)((1 - cmyk.Y) * (1 - cmyk.K) * 255)
            );
        }
        #endregion Color Convertion
    }

}
