using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.People
{
    public class Opponent : Human
    {
        public Opponent(ActionGame game)
            : base(game, null, new PositionInTown(null, Vector2.Zero), 0, Matrix.Identity)
        { }

        public void Load(Model model, PositionInTown position, double azimuth, Matrix worldTransform)
        {
            base.Load(model, position, 0, azimuth, worldTransform);
        }
    }
}
