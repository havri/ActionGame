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
    public class HealBox : Box
    {
        readonly int healPercentage;

        public HealBox(int healPercentage, SoundEffect takeSound, Model model, PositionInTown position, Matrix world)
            : base(takeSound, model, position, world)
        {
            this.healPercentage = healPercentage;
        }

        public int Take()
        {
            base.Take();
            Destroy();
            return healPercentage;
        }
    }
}
