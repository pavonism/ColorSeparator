
using System.Drawing;

namespace ChartControl
{
    internal static class BezierDrawer
    {
        private const float DrawingStep = 0.001f;

        public static PointF[] CalculateCoefficients(ControlPoint[] points)
        {
            if (points.Length < 4)
                throw new ArgumentException();

            PointF[] coefficients = new PointF[points.Length];

            coefficients[0] = new PointF(points[0].X, points[0].Y);
            coefficients[1] = new PointF(3 * (points[1].X - points[0].X), 3 * (points[1].Y - points[0].Y));
            coefficients[2] = new PointF(3 * (points[2].X - 2 * points[1].X + points[0].X), 3 * (points[2].Y - 2 * points[1].Y + points[0].Y));
            coefficients[3] = new PointF(points[3].X - 3 * points[2].X + 3 * points[1].X - points[0].X, points[3].Y - 3 * points[2].Y + 3 * points[1].Y - points[0].Y);

            return coefficients;
        }

        public static void Draw(PointF[] Coefficients, Bitmap bitmap, Color color, int offset, float scale)
        {
            for (float t = 0; t <= 1; t += DrawingStep)
            {
                int x = (int)(scale * (Coefficients[0].X + t * (Coefficients[1].X + t * (Coefficients[2].X + (Coefficients[3].X * t))))) + offset;
                int y = (int)(scale * (Coefficients[0].Y + t * (Coefficients[1].Y + t * (Coefficients[2].Y + (Coefficients[3].Y * t))))) + offset;

                if (0 < x && x < bitmap.Width && 0 < y && y < bitmap.Height)
                    bitmap.SetPixel(x, bitmap.Width - y, color);
            }
        }

        public static float[] CalculateValues(PointF[] coefficients)
        {
            float[] res = new float[100];

            for (float t = 0; t <= 1; t += DrawingStep)
            {
                float x = coefficients[0].X + t * (coefficients[1].X + t * (coefficients[2].X + (coefficients[3].X * t)));
                float y = coefficients[0].Y + t * (coefficients[1].Y + t * (coefficients[2].Y + (coefficients[3].Y * t)));

                res[(int)(100 * x)] = Math.Min(Math.Max(0, y), 1f);
            }

            return res;
        }
    }
}
