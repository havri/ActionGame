using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.Space;
using ActionGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Tools
{
    public abstract class Box : SpatialObject
    {
        readonly ActionGame game;
        readonly SoundEffect takeSound;

        public Box(ActionGame game, SoundEffect takeSound, Model model, PositionInTown position, Matrix world)
            : base(model, position, 0, world)
        {
            this.takeSound = takeSound;
            this.game = game;
        }

        public override void BecomeShoot(GameTime gameTime, int damage, Human by)
        {
            Destroy();
        }

        protected void Take()
        {
            game.SoundPlayer.PlaySound(takeSound, Position);
        }
    }
}
