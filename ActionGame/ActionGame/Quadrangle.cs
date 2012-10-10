using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ActionGame
{
    class Quadrangle
    {
        Vector2 upperLeftCorner;
        Vector2 upperRightCorner;
        Vector2 lowerLeftCorner;
        Vector2 lowerRightCorner;

        public Quadrangle(Vector2 UpperLeftCorner, Vector2 UpperRightCorner, Vector2 LowerLeftCorner, Vector2 LowerRightCorner)
        {
            upperLeftCorner = UpperLeftCorner;
            upperRightCorner = UpperRightCorner;
            lowerLeftCorner = LowerLeftCorner;
            lowerRightCorner = LowerRightCorner;
        }

        protected Quadrangle()
        {
            upperLeftCorner = Vector2.Zero;
            upperRightCorner = Vector2.Zero;
            lowerLeftCorner = Vector2.Zero;
            lowerRightCorner = Vector2.Zero;
        }

        public virtual Vector2 UpperLeftCorner
        {
            get
            {
                return upperLeftCorner;
            }
        }

        public virtual Vector2 UpperRightCorner
        {
            get
            {
                return upperRightCorner;
            }
        }

        public virtual Vector2 LowerLeftCorner
        {
            get
            {
                return lowerLeftCorner;
            }
        }

        public virtual Vector2 LowerRightCorner
        {
            get
            {
                return lowerRightCorner;
            }
        }
    }
}
