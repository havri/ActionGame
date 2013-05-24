using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Tools
{
    /// <summary>
    /// This is a abstraction for tool holded by a human.
    /// </summary>
    public abstract class Tool
    {
        Human holder;
        readonly ActionGame game;

        protected ActionGame Game
        {
            get { return game; }
        } 

        /// <summary>
        /// Gets or sets the holder of this tool
        /// </summary>
        public Human Holder
        {
            get { return holder; }
            set { holder = value; }
        }

        private readonly Texture2D icon;
        /// <summary>
        /// Gets the icon for the toolbar.
        /// </summary>
        public Texture2D Icon
        {
            get { return icon; }
        } 

        /// <summary>
        /// Creates a new tool held by nobody.
        /// </summary>
        /// <param name="icon">Used icon</param>
        public Tool(Texture2D icon)
        {
            this.icon = icon;
        }
        /// <summary>
        /// Creates a new tool.
        /// </summary>
        /// <param name="icon">Used icon for toolbar</param>
        /// <param name="handler">The human that is holding this toolBarText</param>
        /// <param name="game">The game</param>
        public Tool(Texture2D icon, Human handler, ActionGame game)
            : this(icon)
        {
            this.holder = handler;
            this.game = game;
        }

        /// <summary>
        /// Performs the specific tool action if it is overriden.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="position">The position of the tool at the action moment</param>
        /// <param name="azimuth">The action direction</param>
        public abstract void DoAction(GameTime gameTime, PositionInTown position, float azimuth);
        /// <summary>
        /// Gets text for the toolbar.
        /// </summary>
        public abstract string ToolBarText { get; }
        /// <summary>
        /// Gets indicator wheter this tool is usable.
        /// </summary>
        public abstract bool Usable { get; }
    }
}
