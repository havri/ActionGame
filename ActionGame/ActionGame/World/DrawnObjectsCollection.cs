using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Space;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;

namespace ActionGame.World
{
    struct DrawnObjectsCollection : ITransformedDrawable
    {
        readonly List<ITransformedDrawable> objects;
        readonly float azimuthDelta;
        readonly Vector2 positionDelta;
        readonly Matrix transformMatrix;

        public DrawnObjectsCollection(IEnumerable<ITransformedDrawable> objects, float azimuthDelta, Vector2 positionDelta)
        {
            this.objects = new List<ITransformedDrawable>(objects);
            this.azimuthDelta = azimuthDelta;
            this.positionDelta = positionDelta;
            transformMatrix = Matrix.CreateRotationY(-azimuthDelta) * Matrix.CreateTranslation(positionDelta.ToVector3(0)); //azimuth is clockwise
        }

        //public IDrawableObject Object { get { return @object; } }
/*        public float AzimuthDelta { get { return azimuthDelta; } }
        public Vector2 PositionDelta { get { return positionDelta; } }
        public Matrix TransformMatrix
        {
            get
            {
                return transformMatrix;
            }
        }*/

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            foreach (ITransformedDrawable obj in objects)
            {
                obj.Draw(view, projection, transformMatrix * world);
            }
        }

        public void Add(ITransformedDrawable obj)
        {
            objects.Add(obj);
        }

        public void Add(IEnumerable<ITransformedDrawable> objs)
        {
            objects.AddRange(objs);
        }

        public void Remove(ITransformedDrawable obj)
        {
            objects.Remove(obj);
        }

        /*public override bool Equals(object obj)
        {
            if (obj is DrawnObjects)
            {
                ((DrawnObjects)obj).Object.Equals(this.Object);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Object.GetHashCode();
        }

        public static bool operator ==(DrawnObjects a, DrawnObjects b)
        {
            return a.Object == b.Object;
        }
        public static bool operator !=(DrawnObjects a, DrawnObjects b)
        {
            return a.Object != b.Object;
        }*/
    }

}
