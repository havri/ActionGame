using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ActionGame.World;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Tools
{
    public class ToolBox : Box
    {
        /// <summary>
        /// Tool in the box.
        /// </summary>
        private readonly Tool tool;

        public ToolBox(Tool toolInside, Model model, PositionInTown position, Matrix world)
            : base(model, position, world)
        {
            tool = toolInside;
        }

        public Tool Take()
        {
            Destroy();
            return tool;
        }
    }
}
