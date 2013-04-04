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
    public class ToolBox : Box
    {
        /// <summary>
        /// Tool in the box.
        /// </summary>
        private readonly Tool tool;
        SoundEffect takeSound;

        public ToolBox(Tool toolInside, SoundEffect takeSound, Model model, PositionInTown position, Matrix world)
            : base(model, position, world)
        {
            tool = toolInside;
            this.takeSound = takeSound;
        }

        public Tool Take()
        {
            takeSound.Play();
            Destroy();
            return tool;
        }
    }
}
