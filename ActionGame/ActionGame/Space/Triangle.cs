using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ActionGame.Space
{
    struct Triangle
    {
        public Vector2 A { get { return a; } set { a = value; } }
        Vector2 a;
        public Vector2 B { get { return b; } set { b = value; } }
        Vector2 b;
        public Vector2 C { get { return c; } set { c = value; } }
        Vector2 c;

        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        /// <summary>
        /// Says whether the point is inside of the triangle. Taken from http://www.gamespp.com/algorithms/CollisionDetectionTutorial.html
        /// </summary>
        /// <param name="point">Vertex</param>
        /// <returns>True if the vertex is inside.</returns>
        public bool HasInside(Vector2 point)
        {
            const double epsilon = 0.01;
            double dAngle;
            Vector2 vec0 = Vector2.Normalize(point - a);
            Vector2 vec1 = Vector2.Normalize(point - b);
            Vector2 vec2 = Vector2.Normalize(point - c);
            dAngle =
                Math.Acos(Vector2.Dot(vec0, vec1)) +
                Math.Acos(Vector2.Dot(vec1, vec2)) +
                Math.Acos(Vector2.Dot(vec2, vec0));


            if (Math.Abs(dAngle - 2 * Math.PI) < epsilon)
                return true;
            else
                return false;
        }

        public bool IsInCollisionWith(Triangle triangle)
        {
            Line2 ab1 = new Line2(A, B);
            Line2 ac1 = new Line2(A, C);
            Line2 bc1 = new Line2(B, C);

            Line2 ab2 = new Line2(triangle.A, triangle.B);
            Line2 ac2 = new Line2(triangle.A, triangle.C);
            Line2 bc2 = new Line2(triangle.B, triangle.C);

            return ab1.IsCrossing(ab2)
                || ab1.IsCrossing(ac2)
                || ab1.IsCrossing(bc2)
                || ac1.IsCrossing(ab2)
                || ac1.IsCrossing(ac2)
                || ac1.IsCrossing(bc2)
                || bc1.IsCrossing(ab2)
                || bc1.IsCrossing(ac2)
                || bc1.IsCrossing(bc2)
                || HasInside(triangle.A)
                || triangle.HasInside(A);
        }
    }
}
