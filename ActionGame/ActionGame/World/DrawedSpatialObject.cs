using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Space;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;

namespace ActionGame.World
{
    struct DrawedObject
    {
        readonly IDrawableObject @object;
        readonly float azimuthDelta;
        readonly Vector2 positionDelta;
        readonly Matrix transformMatrix;

        public DrawedObject(IDrawableObject @object, float azimuthDelta, Vector2 positionDelta)
        {
            this.@object = @object;
            this.azimuthDelta = azimuthDelta;
            this.positionDelta = positionDelta;
            transformMatrix = Matrix.CreateRotationY(-azimuthDelta) * Matrix.CreateTranslation(positionDelta.ToVector3(0)); //azimuth is clockwise
        }

        public IDrawableObject Object { get { return @object; } }
        public float AzimuthDelta { get { return azimuthDelta; } }
        public Vector2 PositionDelta { get { return positionDelta; } }
        public Matrix TransformMatrix
        {
            get
            {
                return transformMatrix;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is DrawedObject)
            {
                ((DrawedObject)obj).Object.Equals(this.Object);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Object.GetHashCode();
        }

        public static bool operator ==(DrawedObject a, DrawedObject b)
        {
            return a.Object == b.Object;
        }
        public static bool operator !=(DrawedObject a, DrawedObject b)
        {
            return a.Object != b.Object;
        }
    }
}
