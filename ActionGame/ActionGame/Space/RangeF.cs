using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionGame.Space
{
    struct RangeF
    {
        float begin;
        float end;

        public RangeF(float begin, float end)
        {
            this.begin = begin;
            this.end = end;
        }

        public float Begin
        {
            get { return begin; }
            set { begin = value; }
        }

        public float End
        {
            get { return end; }
            set { end = value; }
        }

        public float Length
        {
            get { return Math.Abs(end - begin); }
        }
    }
}

