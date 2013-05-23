using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ActionGame.Tools;
using Microsoft.Xna.Framework.Input;

namespace ActionGame
{
    /// <summary>
    /// Simple structure carrying game setting values.
    /// </summary>
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

        public Keys Forward { get; set; }
        public Keys Backward { get; set; }
        public Keys StepLeft { get; set; }
        public Keys StepRight { get; set; }
        public Keys TurnLeft { get; set; }
        public Keys TurnRight { get; set; }
        public Keys TurnUp { get; set; }
        public Keys TurnDown { get; set; }
        public Keys RunWalkSwitch { get; set; }
        public Keys ShowQuarterMap { get; set; }
        public Keys ShowTownMap { get; set; }
        public Keys CameraSwitch { get; set; }
    }
}
