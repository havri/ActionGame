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
    /// <summary>
    /// Three-dimensional object in the space.
    /// </summary>
    public class SpatialObject : GameObject, ITransformedDrawable
    {
        Model model;
        float verticalPosition;
        float verticalSize;

        /// <summary>
        /// Creates a new spatial object.
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="position">Posistion</param>
        /// <param name="azimuth">Azimuth</param>
        /// <param name="worldTransform">World transform matrix</param>
        public SpatialObject(Model model, PositionInTown position, double azimuth, Matrix worldTransform)
            : base(position, azimuth, (model == null ? Vector2.Zero : model.GetSize(worldTransform).XZToVector2()))
        {
            Load(model, position, 0, azimuth, worldTransform);
        }
        /// <summary>
        /// Creates a new spatial object.
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="quarter">Town quarter</param>
        /// <param name="positionInQuarter">Position inside the town quarter</param>
        /// <param name="azimuth">Azimuth</param>
        /// <param name="worldTransform">World transform matrix</param>
        public SpatialObject(Model model, TownQuarter quarter, Vector3 positionInQuarter, double azimuth, Matrix worldTransform)
            : base(new PositionInTown(quarter, positionInQuarter.XZToVector2()), azimuth, (model == null? Vector2.Zero : model.GetSize(worldTransform).XZToVector2()) )
        {
            Load(model, new PositionInTown(quarter, positionInQuarter.XZToVector2()), positionInQuarter.Y, azimuth, worldTransform);
        }
        /// <summary>
        /// Changes the object's model
        /// </summary>
        /// <param name="newModel">The new used model</param>
        /// <param name="worldTransform">World transform matrix</param>
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
        /// <summary>
        /// Drawes the object on the screen.
        /// </summary>
        /// <param name="view">View transform matrix</param>
        /// <param name="projection">Projection transform matrix</param>
        /// <param name="world">World transform matrix</param>
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


        /// <summary>
        /// Gets 3D positino in the quarter.
        /// </summary>
        public Vector3 PositionInQuarter
        {
            get { return base.Position.PositionInQuarter.ToVector3(verticalPosition); }
        }
        /// <summary>
        /// Gets size of bounding box of this objects.
        /// </summary>
        public Vector3 Size
        {
            get { return base.Size.ToVector3(verticalSize); }
        }
        /// <summary>
        /// Destroys the object.
        /// </summary>
        public virtual void Destroy()
        {
            Position.Quarter.DestroyObject(this);
        }
    }
}
