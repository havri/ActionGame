using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Components
{
    /// <summary>
    /// Class for handling drawn progess bars.
    /// </summary>
    public class ProgressBar
    {
        float val;
        /// <summary>
        /// Gets or sets the current value. Value between 0.0f and 1.0f.
        /// </summary>
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

        /// <summary>
        /// Creates a new progress bar.
        /// </summary>
        /// <param name="texture"></param>
        public ProgressBar(Texture2D texture)
        {
            this.texture = texture;
            val = 0;
        }

        /// <summary>
        /// Draws the progress bar into a specified rectangle.
        /// </summary>
        /// <param name="spriteBatch">Sprite branch for drawing</param>
        /// <param name="targetRectangle">Target rectanhle for the whole 100% progress bar</param>
        public void Draw(SpriteBatch spriteBatch, Rectangle targetRectangle)
        {
            targetRectangle.Width = (int)(val * targetRectangle.Width);
            spriteBatch.Draw(texture, targetRectangle, Color.White);
        }
    }
}
