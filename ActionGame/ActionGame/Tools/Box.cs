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
        readonly SoundEffect takeSound;

        public Box(SoundEffect takeSound, Model model, PositionInTown position, Matrix world)
            : base(model, position, 0, world)
        {
            this.takeSound = takeSound;
        }

        public override void BecomeShot(GameTime gameTime, int damage, Human by)
        {
            Destroy();
        }

        protected void Take()
        {
            takeSound.Play();
        }
    }
}
