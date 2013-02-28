using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ActionGame
{
    public struct GameSettings
    {
        public int TownQuarterCount { get; set; }
        public int AmmoBoxCount { get; set; }
        public int HealBoxCount { get; set; }
        public Size ScreenSize { get; set; }
        public bool Fullscreen { get; set; }
    }
}
