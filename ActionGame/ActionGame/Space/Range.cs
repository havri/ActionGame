using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionGame
{
    struct Range
    {
        int begin;
        int end;

        public Range(int begin, int end)
        {
            this.begin = begin;
            this.end = end;
        }

        public int Begin
        { 
            get { return begin; }
            set { begin = value; }
        }

        public int End
        {
            get { return end; }
            set { end = value; }
        }

        public int Length
        {
            get { return Math.Abs(end - begin); }
        }
    }
}
