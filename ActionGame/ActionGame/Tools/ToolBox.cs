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
        readonly Tool tool;

        public ToolBox(Tool toolInside, SoundEffect takeSound, Model model, PositionInTown position, Matrix world)
            : base(takeSound, model, position, world)
        {
            tool = toolInside;
        }

        public Tool Take()
        {
            base.Take();
            Destroy();
            return tool;
        }
    }
}
