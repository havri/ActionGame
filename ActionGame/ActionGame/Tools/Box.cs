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
    /// <summary>
    /// An abstraction for box in the city. Box can be generally taken by a human. What is in there decide the concrete implementations.
    /// </summary>
    public abstract class Box : SpatialObject
    {
        readonly ActionGame game;
        readonly SoundEffect takeSound;

        /// <summary>
        /// Creates a new Box
        /// </summary>
        /// <param name="game">The game</param>
        /// <param name="takeSound">Sound that will be played if the box is taken</param>
        /// <param name="model"></param>
        /// <param name="position"></param>
        /// <param name="world"></param>
        public Box(ActionGame game, SoundEffect takeSound, Model model, PositionInTown position, Matrix world)
            : base(model, position, 0, world)
        {
            this.takeSound = takeSound;
            this.game = game;
        }

        /// <summary>
        /// Solves the collision with a bullet. The bullet will destroy this box.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="damage">The bullet's damage</param>
        /// <param name="by">The shooter</param>
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
