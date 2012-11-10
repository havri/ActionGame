using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ActionGame
{
    struct DrawedTownQuarter
    {
        public TownQuarter TownQuarter { get; set; }
        public TownQuarterInterfacePosition JoiningInterfacePosition {get; set;}
        public Vector2 Delta { get; set; }
    }
}
