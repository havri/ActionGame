using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Extensions;
using ActionGame.People;
using ActionGame.QSP;
using Microsoft.Xna.Framework;
using System.Drawing;

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

        public bool HasInside(Vector2 point)
        {
            Triangle myUp = new Triangle(UpperLeftCorner, UpperRightCorner, LowerRightCorner);
            Triangle myDown = new Triangle(UpperLeftCorner, LowerLeftCorner, LowerRightCorner);
            return myUp.HasInside(point) || myDown.HasInside(point);
        }

        public bool IsInCollisionWith(Quadrangle obj)
        {
            Triangle myUp = new Triangle(UpperLeftCorner, UpperRightCorner, LowerRightCorner);
            Triangle myDown = new Triangle(UpperLeftCorner, LowerLeftCorner, LowerRightCorner);
            Triangle itsUp = new Triangle(obj.UpperLeftCorner, obj.UpperRightCorner, obj.LowerRightCorner);
            Triangle itsDown = new Triangle(obj.UpperLeftCorner, obj.LowerLeftCorner, obj.LowerRightCorner);

            return myUp.IsInCollisionWith(itsUp) || myUp.IsInCollisionWith(itsDown) || myDown.IsInCollisionWith(itsUp) || myDown.IsInCollisionWith(itsDown);

            return ZapCollisionDetect(obj);
            
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

        public virtual void BecomeShot(int damage, Human by)
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

        private bool ZapCollisionDetect(Quadrangle theObject)
        {
            List<Vector2> aRectangleAxis = new List<Vector2>();
            aRectangleAxis.Add(UpperRightCorner - UpperLeftCorner);
            aRectangleAxis.Add(UpperRightCorner - LowerRightCorner);
            aRectangleAxis.Add(theObject.UpperLeftCorner - theObject.LowerLeftCorner);
            aRectangleAxis.Add(theObject.UpperLeftCorner - theObject.UpperRightCorner);

            foreach (Vector2 aAxis in aRectangleAxis)
            {
                if (!ZapIsAxisCollision(theObject, aAxis))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ZapIsAxisCollision(Quadrangle theObject, Vector2 aAxis)
        {
            int[] aRectangleAScalars = new int[]
            {
                ZapGenerateScalar(theObject.UpperLeftCorner, aAxis),
                ZapGenerateScalar(theObject.UpperRightCorner, aAxis),
                ZapGenerateScalar(theObject.LowerLeftCorner, aAxis),
                ZapGenerateScalar(theObject.LowerRightCorner, aAxis)
            };

            int[] aRectangleBScalars = new int[]
            {
                ZapGenerateScalar(UpperLeftCorner, aAxis),
                ZapGenerateScalar(UpperRightCorner, aAxis),
                ZapGenerateScalar(LowerLeftCorner, aAxis),
                ZapGenerateScalar(LowerRightCorner, aAxis)
            };

            int aRectangleAMinimum = aRectangleAScalars.Min();
            int aRectangleAMaximum = aRectangleAScalars.Max();
            int aRectangleBMinimum = aRectangleBScalars.Min();
            int aRectangleBMaximum = aRectangleBScalars.Max();

            if (aRectangleBMinimum <= aRectangleAMaximum && aRectangleBMaximum >= aRectangleAMaximum)
            {
                return true;
            }
            else if (aRectangleAMinimum <= aRectangleBMaximum && aRectangleAMaximum >= aRectangleBMaximum)
            {
                return true;
            }

            return false;
        }



        private int ZapGenerateScalar(Vector2 theRectangleCorner, Vector2 theAxis)
        {
            float aNumerator = (theRectangleCorner.X * theAxis.X) + (theRectangleCorner.Y * theAxis.Y);
            float aDenominator = (theAxis.X * theAxis.X) + (theAxis.Y * theAxis.Y);
            float aDivisionResult = aNumerator / aDenominator;
            Vector2 aCornerProjected = new Vector2(aDivisionResult * theAxis.X, aDivisionResult * theAxis.Y);

            float aScalar = (theAxis.X * aCornerProjected.X) + (theAxis.Y * aCornerProjected.Y);
            return (int)aScalar;
        }
    }
}
