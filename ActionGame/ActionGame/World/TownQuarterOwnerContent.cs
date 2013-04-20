using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.World
{
    public struct TownQuarterOwnerContent
    {
        public Model FlagModel { get; set; }
        public Model AllyHumanModel { get; set; }
        public Texture2D RoadSignTexture { get; set; }
        public Texture2D ColorTexture { get; set; }
        public Texture2D PointerTexture { get; set; }
        public System.Drawing.Color DrawingColor { get; set; }
    }
}