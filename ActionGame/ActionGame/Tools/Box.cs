using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Space;
using ActionGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Tools
{
    public abstract class Box : SpatialObject
    {
        public Box(Model model, PositionInTown position, Matrix world)
            : base(model, position, 0, world)
        { }

        public override void BecomeShot(int damage)
        {
            Destroy();
        }
    }
}
