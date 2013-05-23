using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.World;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;

namespace ActionGame.Space
{
    /// <summary>
    /// The base of almost all objects in the game. It carries information about position and size and provides projection into a quadrangle.
    /// </summary>
    public class GameObject : Quadrangle
    {
        PositionInTown position;
        double azimuth_;
        Vector2 size;
        protected Vector2 Size
        {
            get { return size; }
            set
            {
                size = value;
            }
        }
        /// <summary>
        /// Creates a new game object
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="azimuth">Azimuth</param>
        /// <param name="size">Size (width, length)</param>
        public GameObject(PositionInTown position, double azimuth, Vector2 size)
        {
            Load(position, azimuth, size);
        }

        protected void Load(PositionInTown position, double azimuth, Vector2 size)
        {
            this.position = position;
            this.azimuth = azimuth;
            this.size = size;
        }

        protected double azimuth
        {
            get { return azimuth_; }
            set
            {
                azimuth_ = ( value + MathHelper.TwoPi ) % MathHelper.TwoPi;
            }
        }

        public double Azimuth
        { get { return azimuth_; } }

        public PositionInTown Pivot
        {
            get
            {
                return new PositionInTown(position.Quarter, position.PositionInQuarter + (size * 0.5f));
            }
        }

        public override Vector2 UpperLeftCorner
        {
            get { return position.PositionInQuarter.Rotate(azimuth, Pivot.PositionInQuarter); }
        }

        public override Vector2 UpperRightCorner
        {
            get { return (position.PositionInQuarter + new Vector2(size.X, 0)).Rotate(azimuth, Pivot.PositionInQuarter); }
        }

        public override Vector2 LowerLeftCorner
        {
            get { return (position.PositionInQuarter + new Vector2(0, size.Y)).Rotate(azimuth, Pivot.PositionInQuarter); }
        }

        public override Vector2 LowerRightCorner
        {
            get { return (position.PositionInQuarter + size).Rotate(azimuth, Pivot.PositionInQuarter); }
        }
        /// <summary>
        /// Chages the objects position and azimuth.
        /// </summary>
        /// <param name="newPosition">The new position</param>
        /// <param name="azimuth">The new azimuth</param>
        public void MoveTo(PositionInTown newPosition, double azimuth)
        {
            position = newPosition;
            this.azimuth = azimuth;
        }
        /// <summary>
        /// Chages the objects position inside the same quarter and azimuth.
        /// </summary>
        /// <param name="newPositionInQuarter">The new position</param>
        /// <param name="azimuth">The new azimuth</param>
        public void MoveTo(Vector2 newPositionInQuarter, double azimuth)
        {
            position.PositionInQuarter = newPositionInQuarter;
            this.azimuth = azimuth;
        }
        /// <summary>
        /// Gets the object's  position.
        /// </summary>
        public PositionInTown Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
    }
}
