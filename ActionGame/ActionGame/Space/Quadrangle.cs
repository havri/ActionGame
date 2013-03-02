using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Extensions;
using ActionGame.QSP;
using Microsoft.Xna.Framework;

namespace ActionGame.Space
{
    public class Quadrangle
    {
        readonly Vector2 upperLeftCorner;
        readonly Vector2 upperRightCorner;
        readonly Vector2 lowerLeftCorner;
        readonly Vector2 lowerRightCorner;
        readonly ISet<GridField> spacePartitioningFields;

        public Quadrangle(Vector2 UpperLeftCorner, Vector2 UpperRightCorner, Vector2 LowerLeftCorner, Vector2 LowerRightCorner)
        {
            upperLeftCorner = UpperLeftCorner;
            upperRightCorner = UpperRightCorner;
            lowerLeftCorner = LowerLeftCorner;
            lowerRightCorner = LowerRightCorner;
            spacePartitioningFields = new HashSet<GridField>();
        }

        protected Quadrangle()
        {
            upperLeftCorner = Vector2.Zero;
            upperRightCorner = Vector2.Zero;
            lowerLeftCorner = Vector2.Zero;
            lowerRightCorner = Vector2.Zero;
            spacePartitioningFields = new HashSet<GridField>();
        }

        public bool IsInCollisionWith(Quadrangle obj)
        {
            Line leftAxis = Line.FromTwoPoints(UpperLeftCorner, LowerLeftCorner);
            Line rightAxis = Line.FromTwoPoints(UpperRightCorner, LowerRightCorner);
            Line upperAxis = Line.FromTwoPoints(UpperLeftCorner, UpperRightCorner);
            Line lowerAxis = Line.FromTwoPoints(LowerLeftCorner, LowerRightCorner);

            return
                ( !leftAxis.HasOnLeftSide(obj) && !rightAxis.HasOnRightSide(obj) && !upperAxis.HasOnLeftSide(obj) && !lowerAxis.HasOnRightSide(obj) ) // classic
                || (!leftAxis.HasOnLeftSide(obj) && !rightAxis.HasOnRightSide(obj) && !upperAxis.HasOnRightSide(obj) && !lowerAxis.HasOnLeftSide(obj)) // horizontal switched
                || (!leftAxis.HasOnRightSide(obj) && !rightAxis.HasOnLeftSide(obj) && !upperAxis.HasOnLeftSide(obj) && !lowerAxis.HasOnRightSide(obj)) // vertical switched
                || (!leftAxis.HasOnRightSide(obj) && !rightAxis.HasOnLeftSide(obj) && !upperAxis.HasOnRightSide(obj) && !lowerAxis.HasOnLeftSide(obj)) // both switched
                ;
        }

        public void CheckHits()
        {
            foreach (GridField field in SpacePartitioningFields)
            {
                foreach(Quadrangle q in field.GetCollisions(this))
                {
                    Hit(q);
                }
            }
        }

        public virtual void Hit(Quadrangle something)
        { }

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

        public ISet<GridField> SpacePartitioningFields
        {
            get
            {
                return spacePartitioningFields;
            }
        }
    }
}
