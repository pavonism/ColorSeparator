
namespace ToolbarControls
{
    public static class ControlConstants
    {
        public const int MinimumControlSize = 32;
        public const int ToolbarWidth = 7 * MinimumControlSize;
        public const int SliderWidth = 3 * MinimumControlSize;
        public const int LabelWidth = 2 * MinimumControlSize;
    }

    public static class Resources
    {
        public static Color ThemeColor = Color.FromArgb(255, 0, 120, 215);
    }

    internal static class ControlGlyphs
    {
        public const string Play = "\u25B6";
        public const string Pause = "\u23F8";
    }
}
