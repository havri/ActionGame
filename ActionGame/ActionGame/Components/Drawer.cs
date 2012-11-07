using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ActionGame
{
    /// <summary>
    /// Drawing component provides rendering right drawing of drawable objects.
    /// </summary>
    class Drawer : DrawableGameComponent
    {
        ActionGame game;
        List<SpatialObject> drawableObjects;
        List<SpatialObject> groundDrawableObjects;
        DrawingOrderComparer objectComparer;
        Matrix projectionMatrix;
        Matrix worldMatrix = Matrix.Identity;
        Texture2D quarterMapPicture;

        /// <summary>
        /// Constructs new drawing component.
        /// </summary>
        /// <param name="game">Game for context</param>
        /// <param name="resolutionWidth">Window width</param>
        /// <param name="resolutionHeight">Window height</param>
        public Drawer(ActionGame game, float resolutionWidth, float resolutionHeight)
            : base(game)
        {
            this.game = game;
            drawableObjects = new List<SpatialObject>();
            groundDrawableObjects = new List<SpatialObject>();
            objectComparer = new DrawingOrderComparer(game.Camera);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, resolutionWidth / resolutionHeight, float.Epsilon, 1000);
        }

        public Matrix WorldTransformMatrix
        {
            get { return worldMatrix; }
        }

        /// <summary>
        /// Adds new object for drawing.
        /// </summary>
        /// <param name="o">The object</param>
        /// <param name="ground">Ground mark - specifates whether the object is flat ground</param>
        public void StartDrawingObject(SpatialObject o, bool ground)
        {
            if(ground)
                groundDrawableObjects.Add(o);
            else
                drawableObjects.Add(o);

            Debug.Write("Drawed objects", (groundDrawableObjects.Count + drawableObjects.Count).ToString());
        }

        /// <summary>
        /// Removes specificated drawable object from drowing collection.
        /// </summary>
        /// <param name="o">The object</param>
        public void StopDrowingObject(SpatialObject o)
        {
            ///TODO: Make this faster.
            drawableObjects.Remove(o);
            groundDrawableObjects.Remove(o);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ///TODO: Maybe this should be called from other Update. For ex. Player's.
            this.ShowQuatterMap = Keyboard.GetState().IsKeyDown(Keys.M);

            ///TODO: Sorting  objects must be by nearest corner!
            ///TODO: This uses QuickSort - too slow. Object are almost sorted... Make it faster (Bubble, Insert).
            drawableObjects.Sort(objectComparer);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (SpatialObject ob in groundDrawableObjects)
            {
                ob.Draw(game.Camera.ViewMatrix, projectionMatrix, worldMatrix);
            }

            foreach (SpatialObject ob in drawableObjects)
            {
                ob.Draw(game.Camera.ViewMatrix, projectionMatrix, worldMatrix);
            }
            game.Player.Draw(game.Camera.ViewMatrix, projectionMatrix, worldMatrix);

            game.SpriteBatch.Begin();
            if (ShowQuatterMap && quarterMapPicture != null)
            {
                game.SpriteBatch.Draw(quarterMapPicture, new Vector2((game.WindowWidth - quarterMapPicture.Width) / 2, (game.WindowHeight - quarterMapPicture.Height) / 2), Color.White);
            }
            game.SpriteBatch.End();
        }

        public Texture2D QuarterMapPicture
        {
            set { quarterMapPicture = value; }
        }

        public bool ShowQuatterMap
        { get; set; }
    }
}
