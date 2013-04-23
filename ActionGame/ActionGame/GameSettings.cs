using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ActionGame.Tools;

namespace ActionGame
{
    public struct GameSettings
    {
        public int TownQuarterCount { get; set; }
        public int AmmoBoxCount { get; set; }
        public int HealBoxCount { get; set; }
        public Size ScreenSize { get; set; }
        public bool Fullscreen { get; set; }
        public int MouseXSensitivity { get; set; }
        public int MouseYSensitivity { get; set; }
        public bool MouseIgnoresWindow { get; set; }
        public bool MouseXInvert { get; set; }
        public bool MouseYInvert { get; set; }
        public string GunSetFilename { get; set; }
    }
}
