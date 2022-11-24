
namespace ChartControl
{
    public class ControlPoint
    {
        public float Scale { get; set; }
        public float X { get; private set; }
        public float Y { get; private set; }

        internal bool edgeControlPoint;

        public event PointMovedEventHandler? PointMoved;

        public ControlPoint(float x, float y, bool edgeControlPoint = false)
        {
            X = x;
            Y = y;

            this.edgeControlPoint = edgeControlPoint;
        }

        public void MoveTo(float x, float y)
        {
            var oldX = X; 
            var oldY = Y;
            if(!edgeControlPoint)
            X = x / Scale; 
            
            Y = y / Scale;

            var eventArguments = new PointMovedEventArguments
            (
                new PointF(oldX, oldY),
                new PointF(X, Y)
            );

            PointMoved?.Invoke(eventArguments);
        }

        internal void Render(Bitmap bitmap, int offset, float scale)
        {
            Scale = scale;

            using(var g = Graphics.FromImage(bitmap)) 
            {
                g.DrawEllipse
                (
                    Constants.ControlPointPen, 
                    X * scale - Constants.ControlPointRadius + offset, 
                    bitmap.Height - Y * scale - Constants.ControlPointRadius - offset, 
                    2*Constants.ControlPointRadius, 
                    2*Constants.ControlPointRadius
                );
            }
        }

        internal bool HitTest(int x, int y, int chartSize)
        {
            return Math.Pow(Math.Abs(X * Scale - x), 2) + Math.Pow(Math.Abs(chartSize - Y * Scale - y), 2) <= Math.Pow(Constants.ControlPointRadius, 2);
        }
    }
}
