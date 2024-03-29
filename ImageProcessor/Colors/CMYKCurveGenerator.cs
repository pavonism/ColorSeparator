﻿using ChartControl;

namespace ImageProcessor
{
    public class CMYKCurveGenerator
    {
        public static void GenerateSample(Charter charter)
        {
            charter.Reset();
            PointF startPoint = new(0, 0);
            PointF endPoint = new(1, 1);

            Color[] colors = new[] { Color.Cyan, Color.Magenta, Color.Yellow, Color.Black };

            for (int i = 0; i < colors.Length; i++)
            {
                charter.AddCurve(new Curve((CurveId)i, startPoint, endPoint, colors[i]));
            }

            charter.Refresh();
        }
    }
}
