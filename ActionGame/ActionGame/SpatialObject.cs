using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame
{
    class SpatialObject : GameObject
    {
        Model model;
        float verticalPosition;
        float verticalSize;

        public SpatialObject(Model model, Vector3 position, double azimuth, Matrix worldTransform)
            : base( position.XZToVector2(), azimuth, (model == null? Vector2.Zero : model.GetSize(worldTransform).XZToVector2()) )
        {
            this.model = model;
            verticalPosition = position.Y;
            if(model != null)
                verticalSize = model.GetSize(worldTransform).Y;
        }

        protected void load(Model model, Vector3 position, double azimuth, Matrix worldTransform)
        {
            this.model = model;
            verticalPosition = position.Y;
            verticalSize = model.GetSize(worldTransform).Y;
            base.load(position.XZToVector2(), azimuth, model.GetSize(worldTransform).XZToVector2());
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
                        * Matrix.CreateTranslation(Position)
                        * Matrix.CreateTranslation((-Pivot).ToVector3(0)) 
                        * Matrix.CreateRotationY(-(float)azimuth) 
                        * Matrix.CreateTranslation(Pivot.ToVector3(0)) 
                        * world ;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }

        public new Vector3 Position
        { 
            get { return base.position.ToVector3(verticalPosition);  }
        }

        protected new Vector3 size
        {
            get { return base.size.ToVector3(verticalSize); }
        }
    }
}
