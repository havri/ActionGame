using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ActionGame.Space
{
    struct Line2
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

        public Line2(Vector2 a, Vector2 b)
        {
            this.a = a;
            this.b = b;
        }


        /// <summary>
        /// From http://devmag.org.za/2009/04/17/basic-collision-detection-in-2d-part-2/
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool IsCrossing(Line2 line)
        {
            float denom = ((line.B.Y - B.Y) * (line.A.X - A.X)) - ((line.B.X - B.X) * (line.A.Y - A.Y));
            if (denom == 0)
            {
                return false;
            }
            else
            {
                float ua = (((line.B.X - B.X) * (A.Y - B.Y)) - ((line.B.Y - B.Y) * (A.X - B.X))) / denom;
                float ub = (((line.A.X - A.X) * (A.Y - B.Y)) - ((line.A.Y - A.Y) * (A.X - B.X))) / denom;
                if ((ua < 0) || (ua > 1) || (ub < 0) || (ub > 1))
                {
                    return false;
                }
            }
            return true;
            //return A + ua * (line.A – A)
        }
    }
}
