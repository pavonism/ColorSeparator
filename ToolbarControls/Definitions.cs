using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolbarControls
{
    public static class ControlConstants
    {
        public const int MinimumControlSize = 32;
        public const int ToolbarWidth = 6 * MinimumControlSize;
        public const int SliderWidth = 3 * MinimumControlSize;
        public const int LabelWidth = 2 * MinimumControlSize;
    }

    internal static class ControlGlyphs
    {
        public const string Play = "\u25B6";
        public const string Pause = "\u23F8";
    }
}
