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
        LinkedList<DrawedTownQuarter> drawedQuarters = new LinkedList<DrawedTownQuarter>();
        DrawingOrderComparer objectComparer;
        Matrix projectionMatrix;
        Matrix worldMatrix = Matrix.Identity;
        Texture2D quarterMapPicture;
        Texture2D townGraphPicture;

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

        public void StartDrawingQuarter(TownQuarter quarter, TownQuarterInterfacePosition position, Vector2 delta)
        {
            drawedQuarters.AddLast(new DrawedTownQuarter { TownQuarter = quarter, JoiningInterfacePosition = position, Delta = delta });
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ///TODO: Maybe this should be called from other Update. For ex. Player's.
            this.ShowQuatterMap = Keyboard.GetState().IsKeyDown(Keys.M);
            this.ShowTownGraph = Keyboard.GetState().IsKeyDown(Keys.N);

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
            if (ShowTownGraph && townGraphPicture != null)
            {
                game.SpriteBatch.Draw(townGraphPicture, new Vector2((game.WindowWidth - townGraphPicture.Width) / 2, (game.WindowHeight - townGraphPicture.Height) / 2), Color.White);
            }
            game.SpriteBatch.End();
        }

        public Texture2D QuarterMapPicture
        {
            set { quarterMapPicture = value; }
        }

        public Texture2D TownGraphPicture
        { 
            set
            {
            	townGraphPicture = value;
            }
        }

        public bool ShowQuatterMap
        { get; set; }

        public bool ShowTownGraph
        { get; set; }
    }
}
