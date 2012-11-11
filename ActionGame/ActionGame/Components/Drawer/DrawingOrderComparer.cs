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
    class DrawingOrderComparer : IComparer<DrawedSpatialObject>
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
        public int Compare(DrawedSpatialObject x, DrawedSpatialObject y)
        {
            if (LengthFromCamera(y) < LengthFromCamera(x))
                return -1;
            else if (LengthFromCamera(x) < LengthFromCamera(y))
                return 1;
            else
                return 0;
        }

        float LengthFromCamera(DrawedSpatialObject dObj)
        {
            Vector2[] cornersOrig = new Vector2[] {
                dObj.Object.UpperLeftCorner,
                dObj.Object.UpperRightCorner,
                dObj.Object.LowerLeftCorner,
                dObj.Object.LowerRightCorner    
            };

            Vector2[] cornersReal = new Vector2[4];
            Matrix quarterTransformMatrix = dObj.TransformMatrix;

            Vector2.TransformNormal(cornersOrig,ref quarterTransformMatrix, cornersReal);

            float minLength = cornersReal.Min( corner => (corner - camera.Position.XZToVector2()).Length() );
            return minLength;
        }
    }
}
