namespace ImageProcessor.Interfaces
{
    public interface ISampleProvider : ISampleViewer
    {
        event Action<Bitmap> SampleChanged;
        event Action<Bitmap> SampleSizeChanged;
        Bitmap Sample { get; }
    }
}