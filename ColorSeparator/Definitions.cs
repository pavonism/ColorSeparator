
namespace ColorSeparator
{

    public static class FormConstants
    {
        public const int MinimumHeight = 500;
        public const int MinimumWidth = 500;
        public const int InitialWidth = 900;
        public const int InitialHeight = 800;

        public const int MainFormColumnCount = 2;
        public const int ChartSize = 500;
        public const int ChartMargin = 10;
        public const int MaxCirclesCount = 24;
        public const int MinCirclesCount = 8;
    }

    public static class Resources
    {
        public const string ProgramTitle = "ColorSeperator";

        public const string Curves = @"..\..\..\..\Assets\Curves";
        public const string CurveExtension = "json";
    }
    public static class Glyphs
    {
        public const string File = "\U0001F4C2";
        public const string Save = "\U0001F4BE";
        public const string Image = "\U0001F5BC";
        public const string Chart = "\U0001F4C8";
        public const string Reset = "\U0001F504";
        public const string Generate = "\U00002B55";
    }

    public static class Labels
    {
        public const string ImageSection = "Image section";
        public const string Cyan = "Cyan";
        public const string Megenta = "Megenta";
        public const string Yellow = "Yellow";
        public const string Black = "Black";
        public const string ShowAllCurves = "Show all curves";
        public const string CirclesOption = "Generate HSV Circles";
        public const string SParamSlider = "S";
        public const string CirclesCount = "Count";
    }

    public static class Hints 
    {
        public const string OpenImage = "Otwórz zdjęcie z pliku";
        public const string SaveCurves = "Zapisz krzywe do pliku";
        public const string LoadCurves = "Wczytaj krzywe z pliku";
        public const string ResetChart = "Ustaw punkty na wykresie w pozycji początkowej";
        public const string ShowAllCurves = "Wyświetl na wykresie wszystkie krzywe";
        public const string SaveImages = "Zapisz obrazy CMYK";
        public const string ApplyCurve = "Ustaw krzywą Gray Ramp";
        public const string GenerateHSV = "Wygeneruj obraz HSV";
    }
}
