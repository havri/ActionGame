using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
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

        public abstract void DoAction();
    }
}
