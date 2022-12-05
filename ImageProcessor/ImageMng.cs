using ChartControl;
using FastBitmap;
using ImageProcessor.Colors;
using ImageProcessor.Interfaces;

namespace ImageProcessor
{
    public class ImageMng
    {
        #region Fields and Properties
        private ISampleProvider? sampleProvider;
        private Dictionary<CurveId, ISampleProvider> cmykProviders = new Dictionary<CurveId, ISampleProvider>();

        private Charter colorCurves;

        public int RenderThreads { get; set; } = 50;
        #endregion Fields and Properties

        #region Events and Handlers
        private void CurveChangedHandler(CurveId? curveId)
        {
            if(curveId == null)
            {
                ReGenerateAll();
                return;
            }

            var curveValues = GetCurveValues();

            if(curveId != null && this.cmykProviders.TryGetValue(curveId.Value, out var provider) && provider.Sample != null)
            {
                RunGenerateImageAsync(provider, curveValues);
            }
        }
        
        private void SampleSizeChangedHandler(ISampleProvider sampleProvider)
        {
            RunGenerateImageAsync(sampleProvider, GetCurveValues());
        }
        #endregion Events and Handlers

        #region Initializing
        public void InitializeWithChart(Charter colorCurves)
        {
            this.colorCurves = colorCurves;
            this.colorCurves.CurveChanged += CurveChangedHandler;
            CMYKCurveGenerator.GenerateSample(this.colorCurves);
        }

        public void Subscribe(ISampleProvider sampleProvider, CurveId? curveId = null)
        {
            if (curveId == null)
                this.sampleProvider = sampleProvider;
            else
            {
                this.cmykProviders.Add(curveId.Value, sampleProvider);
            }

            sampleProvider.SampleChanged += SampleSizeChangedHandler;
        }

        private CurveValues GetCurveValues()
        {
            return new CurveValues
            (
                this.colorCurves.GetCurveValues(CurveId.Cyan),
                this.colorCurves.GetCurveValues(CurveId.Magenta),
                this.colorCurves.GetCurveValues(CurveId.Yellow),
                this.colorCurves.GetCurveValues(CurveId.Black)
            );
        }
        #endregion

        #region Generating Images
        public void SaveAll(Image image, string path, string extension)
        {
            var curveValues = GetCurveValues();
            var bitmap = new DirectBitmap(new Bitmap(image));

            var cmykTable = CmyTableToCmyk(CalculateCmyRepresentation(bitmap), curveValues);

            foreach (var pair in this.cmykProviders)
            {
                RunSaveImageAsync(pair.Value, cmykTable, path + pair.Key + extension);
            }
        }

        public void ReGenerateAll()
        {
            var curveValues = GetCurveValues();

            foreach (var pair in this.cmykProviders)
            {
                if(pair.Value.Sample != null)
                    RunGenerateImageAsync(pair.Value, curveValues);
            }
        }

        private Task GeneratePixelsAsync(int start, int step, DirectBitmap bitmap, Cmyk[,] cmykRepresentation, CurveId? curve = null)
        {
            return Task.Run(
                () =>
                {
                    int width = cmykRepresentation.GetLength(0);
                    int height = cmykRepresentation.GetLength(1);

                    for (int x = start; x < start + step && x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            
                            var color = curve.HasValue ? GetCmykColorInRgb(x, y, curve.Value, cmykRepresentation) : CmykToRgb(cmykRepresentation[x, y]);
                            bitmap.SetPixel(x, y, color);
                        }
                    }
                });
        }

        private void RunSaveImageAsync(ISampleProvider sampleProvider, Cmyk[,] cmykTable, string filename)
        {
            if (!sampleProvider.Busy)
            {
                sampleProvider.Busy = true;
                SaveImageAsync(sampleProvider, cmykTable, sampleProvider.CurveId, filename);
            }
        }

        public Task SaveImageAsync(ISampleProvider sampleProvider, Cmyk[,] cmykTable, CurveId? curveId, string filename)
        {
            return Task.Run(() => {
                var generatedImage = GenerateImage(cmykTable, curveId);
                generatedImage.Bitmap.Save(filename);
                sampleProvider.Busy = false;
            });
        }

        private void RunGenerateImageAsync(ISampleProvider sampleProvider, CurveValues curveValues)
        {
            if(!sampleProvider.Busy)
            {
                sampleProvider.Busy = true;
                GenerateImageAsync(sampleProvider, curveValues, sampleProvider.CurveId);
            }
        }

        public Task GenerateImageAsync(ISampleProvider sampleProvider, CurveValues curveValues, CurveId? curveId)
        {
            return Task.Run(() => { 
                var generatedImage = GenerateImage(CmyTableToCmyk(sampleProvider.CmyTable, curveValues), curveId);
                sampleProvider.PutImage(generatedImage);
                sampleProvider.Busy = false;
            });
        }

        public DirectBitmap GenerateImage(Cmyk[,] cmykRepresentation, CurveId? curveId)
        {
            int width = cmykRepresentation.GetLength(0);
            int height = cmykRepresentation.GetLength(1);
            DirectBitmap currentImage = new DirectBitmap(width, height);
            var rowsPerThread = (int)Math.Ceiling((float)width / this.RenderThreads);
            List<Task> tasks = new();

            for (int i = 0; i < this.RenderThreads; i++)
            {
                tasks.Add(GeneratePixelsAsync(i * rowsPerThread, rowsPerThread, currentImage, cmykRepresentation, curveId));
            }

            Task.WaitAll(tasks.ToArray());
            return currentImage;
        }
        #endregion Generating Images

        #region Color Convertion
        private Cmyk[,] CmyTableToCmyk(Cmyk[,] cmyRepresentation, CurveValues curveValues)
        {
            int width = cmyRepresentation.GetLength(0);
            int height = cmyRepresentation.GetLength(1);

            Cmyk[,] cmykRepresentation = new Cmyk[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cmykRepresentation[x, y] = CmyToCmyk(cmyRepresentation[x, y], curveValues);
                }
            }

            return cmykRepresentation;
        }

        public Cmyk[,] CalculateCmyRepresentation(DirectBitmap imageSample)
        {
            var cmykRepresentation = new Cmyk[imageSample.Width, imageSample.Height];

            for (int x = 0; x < imageSample.Width; x++)
            {
                for (int y = 0; y < imageSample.Height; y++)
                {
                    cmykRepresentation[x, y] = RgbToCmy(imageSample.GetPixel(x, y));
                }
            }

            return cmykRepresentation;
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

        public Cmyk RgbToCmyk(Color color, CurveValues curveValues)
        {
            var cmy = RgbToCmy(color);
            return CmyToCmyk(cmy, curveValues);
        }

        public Cmyk RgbToCmy(Color color)
        {
            return new Cmyk()
            {
                C = 1 - (float)color.R / 255,
                M = 1 - (float)color.G / 255,
                Y = 1 - (float)color.B / 255,
            };
        }

        public Cmyk CmyToCmyk(Cmyk cmyk, CurveValues curveValues)
        {
            var kprime = cmyk.Min;
            var res = new Cmyk()
            {
                C = cmyk.C - kprime + curveValues.GetCurveValue(CurveId.Cyan, kprime),
                M = cmyk.M - kprime + curveValues.GetCurveValue(CurveId.Magenta, kprime),
                Y = cmyk.Y - kprime + curveValues.GetCurveValue(CurveId.Yellow, kprime),
                K = curveValues.GetCurveValue(CurveId.Black, kprime)
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
