using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ActionGame.Space
{
    struct LineSegment
    {
        public Vector2 A
        {
            get { return a; }
            set { a = value; }
        }
        Vector2 a;
        public Vector2 B
        {
            get { return b; }
            set { b = value; }
        }
        Vector2 b;

        public LineSegment(Vector2 a, Vector2 b)
        {
            this.a = a;
            this.b = b;
        }

        public bool IsCrossing(LineSegment lineSegment)
        { 
            Vector2 i;
            return IsCrossing(lineSegment, out i);
        }

        /// <summary>
        /// From http://thirdpartyninjas.com/blog/2010/02/23/line-segment-intersection-update/
        /// </summary>
        /// <param name="lineSegment"></param>
        /// <returns></returns>
        public bool IsCrossing(LineSegment lineSegment, out Vector2 intersection)
        {
            intersection = Vector2.Zero;
            bool coincident, crossing;
            const float epsilon = 0.00001f;

            float ua = (lineSegment.B.X - lineSegment.A.X) * (A.Y - lineSegment.A.Y) - (lineSegment.B.Y - lineSegment.A.Y) * (A.X - lineSegment.A.X);
            float ub = (B.X - A.X) * (A.Y - lineSegment.A.Y) - (B.Y - A.Y) * (A.X - lineSegment.A.X);
            float denominator = (lineSegment.B.Y - lineSegment.A.Y) * (B.X - A.X) - (lineSegment.B.X - lineSegment.A.X) * (B.Y - A.Y);

            crossing = coincident = false;

            if (Math.Abs(denominator) <= epsilon)
            {
                if (Math.Abs(ua) <= 0.00001f && Math.Abs(ub) <= 0.00001f)
                {
                    crossing = coincident = true;
                    intersection = (A + B) / 2;
                }
            }
            else
            {
                ua /= denominator;
                ub /= denominator;

                if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
                {
                    crossing = true;
                    intersection.X = A.X + ua * (B.X - A.X);
                    intersection.Y = A.Y + ua * (B.Y - A.Y);
                }
            }

            return crossing;
        }
    }
}
