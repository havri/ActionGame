using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ActionGame
{
    /// <summary>
    /// Class for sorting objects for drawing.
    /// </summary>
    class DrawingOrderComparer : IComparer<SpatialObject>
    {
        Camera camera;

        public DrawingOrderComparer(Camera camera)
        {
            this.camera = camera;
        }


        /// <summary>
        /// Decides whether the first object is in front of the second or not.
        /// </summary>
        /// <param name="rx">First object</param>
        /// <param name="ry">Second object</param>
        /// <returns>Returns 1 if the firt object is in front of the second one.</returns>
        public int Compare(SpatialObject x, SpatialObject y)
        {
            Vector2 diffx = x.Position.XZToVector2() - camera.Position.XZToVector2();
            Vector2 diffy = y.Position.XZToVector2() - camera.Position.XZToVector2();
            if (diffx.Length() < diffy.Length())
                return 1;
            else if (diffx.Length() > diffy.Length())
                return -1;
            else
                return 0;
        }
    }
}
