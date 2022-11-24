
namespace ChartControl
{
    public class Curve
    {
        public ControlPoint[] ControlPoints { get; private set; } = new ControlPoint[Constants.ControlPointsCount];
        public PointF[] Coefficients { get; private set; } = new PointF[Constants.ControlPointsCount];
        public Color Color { get; private set; }
        public bool ShowControlPoints { get; set; } = false;
        public bool Visible { get; set; } = true;

        public Curve(ControlPoint[] controlPoints, Color color)
        {
            ControlPoints = controlPoints;
            Color = color;
            CalculateCoefficients();
        }

        public Curve(PointF from, PointF to, Color color)
        {
            var dx = (to.X - from.X) / (ControlPoints.Length - 1);
            var dy = (to.Y - from.Y) / (ControlPoints.Length - 1);

            for (int i = 0; i < ControlPoints.Length; i++)
            {
                ControlPoints[i] = new ControlPoint(from.X + i * dx, from.Y + i * dy, i == 0 || i == ControlPoints.Length - 1);
                ControlPoints[i].PointMoved += PointMovedHandler;
            }

            Color = color;
            CalculateCoefficients();
        }

        private void PointMovedHandler(PointMovedEventArguments eventArguments)
        {
            CalculateCoefficients();
        }

        private void CalculateCoefficients()
        {
            Coefficients = BezierDrawer.CalculateCoefficients(ControlPoints);

        }

        internal void Render(Bitmap bitmap, float scale, int offset)
        {
            foreach (var point in ControlPoints)
            {
                point.Scale = scale;
            }

            if(Visible)
                BezierDrawer.Draw(Coefficients, bitmap, Color, offset, scale);
            
            if(ShowControlPoints)
                foreach (var point in ControlPoints)
                {
                    point.Render(bitmap, offset, scale);
                }
        }

        internal bool TrySelectPoint(int x, int y, int chartSize, out ControlPoint? point)
        {
            if(ShowControlPoints)
                foreach (var controlPoint in ControlPoints)
                {
                    if(controlPoint.HitTest(x, y, chartSize))
                    {
                        point = controlPoint;
                        return true;
                    }
                }

            point = null;
            return false;
        }
    }
}
