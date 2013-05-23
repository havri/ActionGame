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

        public static Quadrangle CreateBand(Vector2 start, float azimuth, float width, float length)
        {
            float halfWidth = width * 0.5f;
            Vector2 left = start.Go(halfWidth, azimuth - MathHelper.PiOver2);
            Vector2 right = start.Go(halfWidth, azimuth + MathHelper.PiOver2);
            return new Quadrangle(right, left, right.Go(length, azimuth), left.Go(length, azimuth));
        }

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
        }

        public void CheckHits(bool gameLogicOnly, GameTime gameTime)
        {
            foreach (GridField field in SpacePartitioningFields)
            {
                foreach(Quadrangle q in field.GetCollisions(this))
                {
                    Hit(q, gameLogicOnly, gameTime);
                }
            }
        }

        public virtual void Hit(Quadrangle something, bool gameLogicOnly, GameTime gameTime)
        { }

        public virtual void BecomeShoot(GameTime gameTime, int damage, Human by)
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
