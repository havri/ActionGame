using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.World;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ActionGame.People;
using Microsoft.Xna.Framework.Audio;

namespace ActionGame.Space
{
    public class SpatialObject : GameObject, ITransformedDrawable
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

        public void SetModel(Model newModel, Matrix worldTransform)
        {
            Load(newModel, Position, verticalPosition, Azimuth, worldTransform);
        }

        protected void Load(Model model, PositionInTown position, float verticalPosition, double azimuth, Matrix worldTransform)
        {
            this.model = model;
            this.verticalPosition = verticalPosition;
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
                    effect.EnableDefaultLighting();
                    effect.LightingEnabled = true;
                    if(this.Position.Quarter != null)
                    {
                        Matrix rotM = Matrix.CreateRotationY(Position.Quarter.CurrentDrawingAzimuthDelta);
                        //default XNA constants - we want them transofmed...
                        effect.DirectionalLight0.Direction = Vector3.Transform(new Vector3(-0.5265408f, -0.5735765f, -0.6275069f), rotM);
                        effect.DirectionalLight1.Direction = Vector3.Transform(new Vector3(0.7198464f, 0.3420201f, 0.6040227f), rotM);
                        effect.DirectionalLight2.Direction = Vector3.Transform(new Vector3(0.4545195f, -0.7660444f, 0.4545195f), rotM);
                    }
                }
                mesh.Draw();
            }
        }



        public Vector3 PositionInQuarter
        {
            get { return base.Position.PositionInQuarter.ToVector3(verticalPosition); }
        }

        public Vector3 Size
        {
            get { return base.Size.ToVector3(verticalSize); }
        }

        public virtual void Destroy()
        {
            Position.Quarter.DestroyObject(this);
        }
    }
}
