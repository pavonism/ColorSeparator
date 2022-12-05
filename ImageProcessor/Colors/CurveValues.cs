using ChartControl;

namespace ImageProcessor.Colors
{
    public class CurveValues
    {
        public float[] CyanValues { get; private set; }
        public float[] MagentaValues { get; private set; }
        public float[] YellowValues { get; private set; }
        public float[] BlackValues { get; private set; }

        public CurveValues(float[] cyanValues, float[] magentaValues, float[] yellowValues, float[] blackValues)
        {
            CyanValues = cyanValues;
            MagentaValues = magentaValues;
            YellowValues = yellowValues;
            BlackValues = blackValues;
        }

        public float GetCurveValue(CurveId curve, float k)
        {
            var indx = (int)Math.Min(100 * k, 99);

            switch (curve)
            {
                case CurveId.Cyan:
                    return CyanValues[indx];
                case CurveId.Magenta:
                    return MagentaValues[indx];
                case CurveId.Yellow:
                    return YellowValues[indx];
                case CurveId.Black:
                    return BlackValues[indx];
            }

            return 0f;
        }
    }
}
