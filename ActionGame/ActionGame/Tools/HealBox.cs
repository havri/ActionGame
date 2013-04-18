using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ActionGame.World;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Tools
{
    class HealBox : Box
    {
        public HealBox(Model model, PositionInTown position, Matrix world)
            : base(model, position, world)
        {
            
        }
    }
}
