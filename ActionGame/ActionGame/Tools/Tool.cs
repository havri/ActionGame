﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Tools
{
    public abstract class Tool
    {
        protected Human handler;
        private readonly Texture2D icon;
        public Texture2D Icon
        {
            get { return icon; }
        } 


        public Tool(Texture2D icon)
        {
            this.icon = icon;
        }

        public Tool(Texture2D icon, Human handler)
            : this(icon)
        {
            this.handler = handler;
        }

        public abstract void DoAction(GameTime gameTime, PositionInTown position, float azimuth);

        public abstract string ToolBarText { get; }
    }
}
