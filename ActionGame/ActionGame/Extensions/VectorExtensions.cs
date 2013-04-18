using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ActionGame.Space;
using Microsoft.Xna.Framework;

namespace ActionGame.Extensions
{
    static class VectorExtensions
    {
        public static Vector2 Rotate(this Vector2 vector, double radians, Vector2 pivot)
        {
            Vector2 byOrigin = vector - pivot;
            Vector2 rotatedVector = Vector2.Transform(byOrigin, Matrix.CreateRotationZ((float)radians));
            return rotatedVector + pivot;
        }

        public static Vector2 XZToVector2(this Vector3 vector)
        {
            return new Vector2(vector.X, vector.Z);
        }

        public static Vector2 Go(this Vector2 vector, float length, double azimuth)
        {
            vector.X += (float)Math.Sin(azimuth) * length;
            vector.Y -= (float)Math.Cos(azimuth) * length;
            return vector;
        }

        public static Vector3 ToVector3(this Vector2 vector, float y)
        {
            return new Vector3(vector.X, y, vector.Y);
        }

        public static PointF ToPointF(this Vector2 vector)
        {
            return new PointF(vector.X, vector.Y);
        }
        /// <summary>
        /// Computes angle between this vector and Y axis. Angle is clockwise.
        /// </summary>
        /// <param name="vector">This vector</param>
        /// <returns>Angle in radians. Value between 0 - 2PI.</returns>
        public static float GetAngle(this Vector2 vector)
        {
            float atan = (float)Math.Atan2(vector.Y, vector.X);
            atan = (MathHelper.TwoPi + atan) % MathHelper.TwoPi;
            return atan;
        }

        public static Vector2 ToVector2(this Microsoft.Xna.Framework.Point point)
        {
            return new Vector2(point.X, point.Y);
        }
    }
}
