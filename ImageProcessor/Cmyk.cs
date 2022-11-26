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

        public void CutValues()
        {
            C = CutValue(C);
            M = CutValue(M);
            Y = CutValue(Y);
            K = CutValue(K);
        }

        private float CutValue(float value)
        {
            return Math.Min(Math.Max(0, value), 1);
        }
    }
}
