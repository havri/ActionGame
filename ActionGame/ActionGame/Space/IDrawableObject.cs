using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ActionGame.Space
{
    public interface IDrawableObject
    {
        void Draw(Matrix view, Matrix projection, Matrix world);
    }
}
