using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.World;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ActionGame.People;

namespace ActionGame.Space
{
    public class SpatialObject : GameObject, IDisposable, IDrawableObject
    {
        Model model;
        float verticalPosition;
        float verticalSize;

        public SpatialObject(Model model, PositionInTown position, double azimuth, Matrix worldTransform)
            : base(position, azimuth, (model == null ? Vector2.Zero : model.GetSize(worldTransform).XZToVector2()))
        {
            Load(model, position, 0, azimuth, worldTransform);
        }

        public SpatialObject(Model model, TownQuarter quarter, Vector3 positionInQuarter, double azimuth, Matrix worldTransform)
            : base(new PositionInTown(quarter, positionInQuarter.XZToVector2()), azimuth, (model == null? Vector2.Zero : model.GetSize(worldTransform).XZToVector2()) )
        {
            Load(model, new PositionInTown(quarter, positionInQuarter.XZToVector2()), positionInQuarter.Y, azimuth, worldTransform);
        }

        protected void Load(Model model, PositionInTown position, float verticalPosition, double azimuth, Matrix worldTransform)
        {
            this.model = model;
            verticalPosition = position.PositionInQuarter.Y;
            Vector3 size = Vector3.Zero;
            if (model != null)
            {
                size = model.GetSize(worldTransform);
                verticalSize = size.Y;
            }
            base.Load(position, azimuth, size.XZToVector2());
        }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            

            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    effect.World =
                        transforms[mesh.ParentBone.Index]
                        * Matrix.CreateTranslation(PositionInQuarter)
                        * Matrix.CreateTranslation((-Pivot.PositionInQuarter).ToVector3(0)) 
                        * Matrix.CreateRotationY(-(float)azimuth)
                        * Matrix.CreateTranslation(Pivot.PositionInQuarter.ToVector3(0))
                        * world ;
                    effect.View = view;
                    effect.Projection = projection;
                    //effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }



        public Vector3 PositionInQuarter
        {
            get { return base.Position.PositionInQuarter.ToVector3(verticalPosition); }
        }

        protected Vector3 Size
        {
            get { return base.Size.ToVector3(verticalSize); }
        }

        public void Dispose()
        {
            model = null;
        }

        public virtual void Destroy()
        {
            Position.Quarter.DestroyObject(this);
        }
    }
}
