using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ActionGame.World;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace ActionGame.Tools
{
    /// <summary>
    /// Box with first-aid for the human.
    /// </summary>
    public class HealBox : Box
    {
        readonly int healPercentage;

        /// <summary>
        /// Creates a new heal box.
        /// </summary>
        /// <param name="healPercentage">Percentage that will be added to the human's health status if he takes it.</param>
        /// <param name="game">The game</param>
        /// <param name="takeSound">The take sound effect</param>
        /// <param name="model">Model of the box</param>
        /// <param name="position">Position</param>
        /// <param name="world">World transformation matrix</param>
        public HealBox(int healPercentage, ActionGame game, SoundEffect takeSound, Model model, PositionInTown position, Matrix world)
            : base(game, takeSound, model, position, world)
        {
            this.healPercentage = healPercentage;
        }

        /// <summary>
        /// Performs the taking of the box - destroys the box and returns the heal percentage.
        /// </summary>
        /// <returns>The healing percentage</returns>
        public int Take()
        {
            base.Take();
            Destroy();
            return healPercentage;
        }
    }
}
