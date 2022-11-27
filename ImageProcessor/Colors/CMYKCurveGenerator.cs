using ChartControl;

namespace ImageProcessor.Colors
{
    public class CMYKCurveGenerator
    {
        public static void GenerateSample(Charter charter)
        {
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

    public enum CurveId
    {
        Cyan,
        Magenta,
        Yellow,
        Black,
    }
}
