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
    /// Box containg a specific tool instance.
    /// </summary>
    public class ToolBox : Box
    {
        /// <summary>
        /// Tool in the box.
        /// </summary>
        readonly Tool tool;
        /// <summary>
        /// Creates a new toolbox
        /// </summary>
        /// <param name="toolInside">The tool instance inside</param>
        /// <param name="game">The game</param>
        /// <param name="takeSound">The taking sound</param>
        /// <param name="model">Model of the box</param>
        /// <param name="position">Position of the box</param>
        /// <param name="world">World transformation matrix</param>
        public ToolBox(Tool toolInside, ActionGame game, SoundEffect takeSound, Model model, PositionInTown position, Matrix world)
            : base(game, takeSound, model, position, world)
        {
            tool = toolInside;
        }
        /// <summary>
        /// Performs taking of this box. It destroys it and return the tool to the taker.
        /// </summary>
        /// <returns>The tool what was inside</returns>
        public Tool Take()
        {
            base.Take();
            Destroy();
            return tool;
        }
    }
}
