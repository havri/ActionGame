using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;

namespace ActionGame.Space
{
    public class Quadrangle
    {
        readonly Vector2 upperLeftCorner;
        readonly Vector2 upperRightCorner;
        readonly Vector2 lowerLeftCorner;
        readonly Vector2 lowerRightCorner;

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

        public bool IsInCollisionWith(Quadrangle obj)
        {
            Line leftAxis = Line.FromTwoPoints(UpperLeftCorner, LowerLeftCorner);
            Line rightAxis = Line.FromTwoPoints(UpperRightCorner, LowerRightCorner);
            Line upperAxis = Line.FromTwoPoints(UpperLeftCorner, UpperRightCorner);
            Line lowerAxis = Line.FromTwoPoints(LowerLeftCorner, LowerRightCorner);

            return leftAxis.IsCrossing(obj)
                || upperAxis.IsCrossing(obj)
                || lowerAxis.IsCrossing(obj)
                || rightAxis.IsCrossing(obj)
                || (!leftAxis.HasOnLeftSide(obj.UpperLeftCorner) && rightAxis.HasOnLeftSide(obj.UpperLeftCorner) && !upperAxis.HasOnLeftSide(obj.UpperLeftCorner) && lowerAxis.HasOnLeftSide(obj.UpperLeftCorner)) // inside normal
                || (leftAxis.HasOnLeftSide(obj.UpperLeftCorner) && !rightAxis.HasOnLeftSide(obj.UpperLeftCorner) && upperAxis.HasOnLeftSide(obj.UpperLeftCorner) && !lowerAxis.HasOnLeftSide(obj.UpperLeftCorner)) // inside up-side-down
                ;
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
