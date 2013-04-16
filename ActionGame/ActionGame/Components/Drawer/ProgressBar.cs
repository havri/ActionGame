using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Components
{
    public class ProgressBar
    {
        float val;
        public float Value
        {
            get
            {
                return val;
            }
            set
            {
                if (val == value)
                    return;
                val = value;
            }
        }
        readonly Texture2D texture;

        public ProgressBar(Texture2D texture)
        {
            this.texture = texture;
            val = 0;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle targetRectangle)
        {
            targetRectangle.Width = (int)(val * targetRectangle.Width);
            spriteBatch.Draw(texture, targetRectangle, Color.White);
        }
    }
}
