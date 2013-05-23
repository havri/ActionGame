using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ActionGame.Space
{
    /// <summary>
    /// Interface for drawable objects of various types.
    /// </summary>
    public interface ITransformedDrawable
    {
        /// <summary>
        /// Draws the object on the screen.
        /// </summary>
        /// <param name="view">View transformation matrix</param>
        /// <param name="projection">Projection transformation matrix</param>
        /// <param name="world">World transformation matrix</param>
        void Draw(Matrix view, Matrix projection, Matrix world);
    }
}
