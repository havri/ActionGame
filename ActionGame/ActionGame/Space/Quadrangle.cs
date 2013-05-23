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
    /// <summary>
    /// Base quadrangle projection of every object in the game. It defined by four corners.
    /// </summary>
    public class Quadrangle
    {
        readonly Vector2 upperLeftCorner;
        readonly Vector2 upperRightCorner;
        readonly Vector2 lowerLeftCorner;
        readonly Vector2 lowerRightCorner;
        /// <summary>
        /// Set of the partitioning fields this quadrangle belongs to.
        /// </summary>
        readonly ISet<GridField> spacePartitioningFields;
        /// <summary>
        /// Creates quadrangle forming a band from a specified point.
        /// </summary>
        /// <param name="start">The start point</param>
        /// <param name="azimuth">Direction azimuth</param>
        /// <param name="width">The band width</param>
        /// <param name="length">The band length</param>
        /// <returns></returns>
        public static Quadrangle CreateBand(Vector2 start, float azimuth, float width, float length)
        {
            float halfWidth = width * 0.5f;
            Vector2 left = start.Go(halfWidth, azimuth - MathHelper.PiOver2);
            Vector2 right = start.Go(halfWidth, azimuth + MathHelper.PiOver2);
            return new Quadrangle(right, left, right.Go(length, azimuth), left.Go(length, azimuth));
        }
        /// <summary>
        /// Creates a new quadrangle from the four corners
        /// </summary>
        /// <param name="UpperLeftCorner">Upper left corner</param>
        /// <param name="UpperRightCorner">Upper right corner</param>
        /// <param name="LowerLeftCorner">Lower left corner</param>
        /// <param name="LowerRightCorner">Lower right corner</param>
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
        /// <summary>
        /// Says whether this quadrangle has the point inside or not.
        /// </summary>
        /// <param name="point">The tested point</param>
        /// <returns>True if the point is inside the quadrangle</returns>
        public bool HasInside(Vector2 point)
        {
            Triangle myUp = new Triangle(UpperLeftCorner, UpperRightCorner, LowerRightCorner);
            Triangle myDown = new Triangle(UpperLeftCorner, LowerLeftCorner, LowerRightCorner);
            return myUp.HasInside(point) || myDown.HasInside(point);
        }
        /// <summary>
        /// Says whether this quadrangle is in collision with the given one.
        /// </summary>
        /// <param name="obj">The given quadrangle</param>
        /// <returns>True if they collide</returns>
        public bool IsInCollisionWith(Quadrangle obj)
        {
            Triangle myUp = new Triangle(UpperLeftCorner, UpperRightCorner, LowerRightCorner);
            Triangle myDown = new Triangle(UpperLeftCorner, LowerLeftCorner, LowerRightCorner);
            Triangle itsUp = new Triangle(obj.UpperLeftCorner, obj.UpperRightCorner, obj.LowerRightCorner);
            Triangle itsDown = new Triangle(obj.UpperLeftCorner, obj.LowerLeftCorner, obj.LowerRightCorner);

            return myUp.IsInCollisionWith(itsUp) || myUp.IsInCollisionWith(itsDown) || myDown.IsInCollisionWith(itsUp) || myDown.IsInCollisionWith(itsDown);
        }
        /// <summary>
        /// Check all the collision of itself and calls the Hit method.
        /// </summary>
        /// <param name="gameLogicOnly">Simplyfied logic indicator</param>
        /// <param name="gameTime">Game time</param>
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

        /// <summary>
        /// Solves collision with something if it is overriden.
        /// </summary>
        /// <param name="something">The hit object</param>
        /// <param name="gameLogicOnly">The simplyfied logic indicator</param>
        /// <param name="gameTime">Game time</param>
        public virtual void Hit(Quadrangle something, bool gameLogicOnly, GameTime gameTime)
        { }
        /// <summary>
        /// Solves collision with bullet if it is overriden.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="damage">The shoot damage</param>
        /// <param name="by">The shooter</param>
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
        /// <summary>
        /// Gets set of the partitioning fields this quadrangle belongs to.
        /// </summary>
        public ISet<GridField> SpacePartitioningFields
        {
            get
            {
                return spacePartitioningFields;
            }
        }
    }
}
