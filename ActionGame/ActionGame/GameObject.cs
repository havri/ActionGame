using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ActionGame
{
    class GameObject : Quadrangle
    {
        protected Vector2 position;
        double azimuth_;
        protected Vector2 size;

        public GameObject(Vector2 position, double azimuth, Vector2 size)
        {
            load(position, azimuth, size);
        }

        protected void load(Vector2 position, double azimuth, Vector2 size)
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
                azimuth_ = value;
                while (azimuth_ > MathHelper.TwoPi)
                    azimuth_ -= MathHelper.TwoPi;
                while (azimuth_ < 0.00)
                    azimuth_ += MathHelper.TwoPi;
            }
        }

        public Vector2 Pivot
        {
            get { return position + (size * 0.5f); }
        }

        public override Vector2 UpperLeftCorner
        {
            get { return position.Rotate(azimuth, Pivot); }
        }

        public override Vector2 UpperRightCorner
        {
            get { return (position + new Vector2(size.X, 0)).Rotate(azimuth, Pivot); }
        }

        public override Vector2 LowerLeftCorner
        {
            get { return (position + new Vector2(0, size.Y)).Rotate(azimuth, Pivot); }
        }

        public override Vector2 LowerRightCorner
        {
            get { return (position + size).Rotate(azimuth, Pivot); }
        }

    }
}
