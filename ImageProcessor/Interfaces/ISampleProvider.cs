﻿using ChartControl;
using FastBitmap;
using ImageProcessor.Colors;

namespace ImageProcessor.Interfaces
{
    public interface ISampleProvider : ISampleViewer
    {
        public Image SourceImage { get; set; }

        CurveId? CurveId { get; }
        Cmyk[,] CmyTable { get; }
        bool Busy { get; set; }

        event Action<ISampleProvider> SampleChanged;
        DirectBitmap Sample { get; }
    }
}