using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ActionGame.Space
{
    public struct Line
    {
        private float yIntersect;
        private float xIntersect;
        public float XIntersect
        {
            get
            {
                return xIntersect;
            }
            set
            {
                xIntersect = value;
            }
        }
        public float YIntersect
        {
            get
            {
                return yIntersect;
            }
            set
            {
                yIntersect = value;
            }
        }

        public Line(float xIntersect, float yIntersect)
        {
            this.xIntersect = xIntersect;
            this.yIntersect = yIntersect;
        }

        public static Line FromTwoPoints(Vector2 a, Vector2 b)
        {
            Vector2 diff = b - a;
            float y;
            float x;

            if (diff.X == 0)
            {
                x = a.X;
                y = float.PositiveInfinity;
            }
            else if (diff.Y == 0)
            {
                x = float.PositiveInfinity;
                y = a.Y;
            }
            else
            {
                x = ((diff.X / (-diff.Y)) * a.Y) + a.X;
                y = (((-diff.Y) / diff.X) * b.X) + b.Y;
            }

            return new Line(x, y);
        }



        public bool IsCrossing(Quadrangle quadrangle)
        {
            return !HasOnLeftSide(quadrangle) && !HasOnRightSide(quadrangle);
        }

        public bool HasOnLeftSide(Vector2 point)
        {
            if (XIntersect == float.PositiveInfinity)
            {
                return point.Y < YIntersect;
            }
            if (YIntersect == float.PositiveInfinity)
            {
                return point.X < XIntersect;
            }

            float nx = (YIntersect / XIntersect) * point.Y;
            float dx = XIntersect - nx;

            return (dx > point.X);
        }

        public bool HasOnLeftSide(Quadrangle quadrangle)
        {
            return HasOnLeftSide(quadrangle.UpperLeftCorner)
                && HasOnLeftSide(quadrangle.UpperRightCorner)
                && HasOnLeftSide(quadrangle.LowerLeftCorner)
                && HasOnLeftSide(quadrangle.LowerRightCorner);
        }

        public bool HasOnRightSide(Quadrangle quadrangle)
        {
            return !HasOnLeftSide(quadrangle.UpperLeftCorner)
                && !HasOnLeftSide(quadrangle.UpperRightCorner)
                && !HasOnLeftSide(quadrangle.LowerLeftCorner)
                && !HasOnLeftSide(quadrangle.LowerRightCorner);
        }

        public override bool Equals(object obj)
        {
            if (obj is Line)
            { 
                return this == (Line)obj;
            }
            return base.Equals(obj);
        }

        public static bool operator== (Line l1, Line l2)
        {
            return l1.xIntersect == l2.xIntersect && l1.yIntersect == l2.yIntersect;
        }
        public static bool operator !=(Line l1, Line l2)
        {
            return l1.xIntersect != l2.xIntersect || l1.yIntersect != l2.yIntersect;
        }
    }
}
