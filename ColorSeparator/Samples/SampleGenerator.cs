using ColorSeparator;

namespace ColorSeparatorApp.Samples
{
    internal class SampleGenerator
    {

        public static CurveSample[] GenerateAllSamples()
        {
            string[] files = Directory.GetFiles(Resources.Curves, $"*.{Resources.CurveExtension}");
            CurveSample[] samples = new CurveSample[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                samples[i] = new CurveSample(files[i], Path.GetFileNameWithoutExtension(files[i]));
            }

            return samples;
        }
    }
}
