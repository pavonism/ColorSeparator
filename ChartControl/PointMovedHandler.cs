
namespace ChartControl
{
    public delegate void PointMovedEventHandler(PointMovedEventArguments eventArguments);
    public class PointMovedEventArguments
    {
        public PointMovedEventArguments(PointF from, PointF to)
        {
            From = from;
            To = to;
        }

        public PointF From { get; }
        public PointF To { get; }
    }
}
