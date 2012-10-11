using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame
{
    public class Human : SpatialObject
    {
        public static readonly float StepLength = 0.05f;
        public static readonly double RotateAngle = MathHelper.Pi / 50f;
        public static readonly float ThirdHeadHorizontalDistance = 1.5f;
        public static readonly float ThirdHeadVerticalDistance = 0.1f;
        public static readonly float LookingAtDistance = 10;
        public static readonly float LookingAtHeightStep = 0.3f;

        protected int health;
        protected float lookingAtHeight;

        public Human(Model model, Vector3 position, double azimuth, Matrix worldTransform)
            : base(model, position, azimuth, worldTransform)
        {
            health = 100;
        }

        protected void Go(bool forward)
        {
            position = position.Go(StepLength * (forward ? 1 : -1), azimuth);
        }

        protected void Step(bool toLeft)
        {
            position = position.Go(StepLength, azimuth + (toLeft ? -MathHelper.PiOver2 : MathHelper.PiOver2));
        }

        protected void Rotate(bool toLeft)
        {
            azimuth += (toLeft ? -1 : 1) * RotateAngle;
        }

        public Vector3 FirstHeadPosition
        {
            get
            { 
                Vector2 ret = Pivot.Go(2*size.Z, azimuth);
                return ret.ToVector3(size.Y);
            }
        }

        public Vector3 ThirdHeadPosition
        {
            get
            {
                Vector2 ret = Pivot.Go(-(size.Z + ThirdHeadHorizontalDistance), azimuth);
                return ret.ToVector3(size.Y + ThirdHeadVerticalDistance);
            }
        }

        public Vector3 LookingAt
        {
            get
            {
                Vector2 ret = Pivot.Go((size.Z + LookingAtDistance), azimuth);
                return ret.ToVector3(lookingAtHeight * LookingAtHeightStep);
            }
        }

    }
}
