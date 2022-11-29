
namespace ColorSeparatorApp.Samples
{
    internal class CurveSample
    {
        public string Path { get; private set; } 
        public string Name { get; private set; }

        public CurveSample(string path, string name)
        {
            Path = path;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
