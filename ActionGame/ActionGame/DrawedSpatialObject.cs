using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Space;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;

namespace ActionGame
{
    struct DrawedSpatialObject
    {
        SpatialObject @object;
        float azimuthDelta;
        Vector2 positionDelta;
        Matrix transformMatrix;

        public DrawedSpatialObject(SpatialObject @object, float azimuthDelta, Vector2 positionDelta)
        {
            this.@object = @object;
            this.azimuthDelta = azimuthDelta;
            this.positionDelta = positionDelta;
            transformMatrix = Matrix.CreateRotationY(-azimuthDelta) * Matrix.CreateTranslation(positionDelta.ToVector3(0)); //azimuth is clockwise
        }

        public SpatialObject Object { get { return @object; } }
        public float AzimuthDelta { get { return azimuthDelta; } }
        public Vector2 PositionDelta { get { return positionDelta; } }
        public Matrix TransformMatrix
        {
            get
            {
                return transformMatrix;
            }
        }

    }
}
