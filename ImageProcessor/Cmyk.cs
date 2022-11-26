using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{
    public struct Cmyk
    {
        public float C { get; set; }
        public float M {get; set; }
        public float Y {get; set; }
        public float K { get; set; }

        public float Min => Math.Min(Math.Min(C, M), Y);
    }
}
