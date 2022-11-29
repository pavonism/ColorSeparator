﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorSeparator
{

    public static class FormConstants
    {
        public const int MinimumHeight = 500;
        public const int MinimumWidth = 500;
        public const int InitialWidth = 900;
        public const int InitialHeight = 750;

        public const int MainFormColumnCount = 2;
        public const int ChartSize = 500;
        public const int ChartMargin = 10;
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
    }

    public static class Labels
    {
        public const string Cyan = "Cyan";
        public const string Megenta = "Megenta";
        public const string Yellow = "Yellow";
        public const string Black = "Black";
        public const string ShowAllCurves = "Show all curves";
    }
}
