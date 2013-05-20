using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.World;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;

namespace ActionGame.Space
{
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

        public void MoveTo(PositionInTown newPosition, double azimuth)
        {
            position = newPosition;
            this.azimuth = azimuth;
        }

        public void MoveTo(Vector2 newPositionInQuarter, double azimuth)
        {
            position.PositionInQuarter = newPositionInQuarter;
            this.azimuth = azimuth;
        }

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
