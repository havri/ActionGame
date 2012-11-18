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
    public class Drawer : DrawableGameComponent
    {
        ActionGame game;
        List<DrawedSpatialObject> drawableObjects;
        List<DrawedSpatialObject> groundDrawableObjects;
        DrawingOrderComparer objectComparer;
        Matrix projectionMatrix;
        Matrix worldMatrix = Matrix.Identity;
        Texture2D townGraphPicture;
        Texture2D playerIcon;
        TownQuarter currentQuarter;


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
            drawableObjects = new List<DrawedSpatialObject>();
            groundDrawableObjects = new List<DrawedSpatialObject>();
            objectComparer = new DrawingOrderComparer(game.Camera);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, resolutionWidth / resolutionHeight, float.Epsilon, 1000);
        }

        public Matrix WorldTransformMatrix
        {
            get { return worldMatrix; }
        }

        public void StartDrawingSpatialObject(SpatialObject obj, float azimuthDelta, Vector2 positionDelta, bool ground)
        {
            DrawedSpatialObject dObj = new DrawedSpatialObject(obj, azimuthDelta, positionDelta );
            if (ground)
                groundDrawableObjects.Add(dObj);
            else
                drawableObjects.Add(dObj);   
        }

        public void StopDrawingSpatialObject(SpatialObject obj, bool ground)
        {
            if (ground)
            {
                groundDrawableObjects.RemoveAll(x => x.Object == obj);
            }
            else
            {
                drawableObjects.RemoveAll(x => x.Object == obj);
            }
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

            Debug.Write("Drawed objects", (groundDrawableObjects.Count + drawableObjects.Count).ToString());
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (DrawedSpatialObject dObj in groundDrawableObjects)
            {
                dObj.Object.Draw(game.Camera.ViewMatrix, projectionMatrix, dObj.TransformMatrix * worldMatrix);
            }

            foreach (DrawedSpatialObject dObj in drawableObjects)
            {
                dObj.Object.Draw(game.Camera.ViewMatrix, projectionMatrix, dObj.TransformMatrix * worldMatrix);
            }
            game.Player.Draw(game.Camera.ViewMatrix, projectionMatrix,  worldMatrix);

            game.SpriteBatch.Begin();
            if (ShowQuatterMap)
            {
                Vector2 mapPosition = new Vector2((game.WindowWidth - currentQuarter.Map.Width) / 2, (game.WindowHeight - currentQuarter.Map.Height) / 2);
                game.SpriteBatch.Draw(currentQuarter.Map,mapPosition , Color.White);

                Vector2 playerPosition = new Vector2(
                    currentQuarter.Map.Width * (game.Player.Position.X / currentQuarter.QuarterSize.X) - playerIcon.Width / 2,
                    currentQuarter.Map.Height * (game.Player.Position.Z / currentQuarter.QuarterSize.Y) - playerIcon.Height / 2);
                playerPosition += mapPosition;
                game.SpriteBatch.Draw(playerIcon, playerPosition, null, Color.White, (float)game.Player.Azimuth, new Vector2(playerIcon.Width / 2, playerIcon.Height / 2), 1, SpriteEffects.None, 0);
            }
            if (ShowTownGraph && townGraphPicture != null)
            {
                game.SpriteBatch.Draw(townGraphPicture, new Vector2((game.WindowWidth - townGraphPicture.Width) / 2, (game.WindowHeight - townGraphPicture.Height) / 2), Color.White);
            }
            game.SpriteBatch.End();
        }


        public TownQuarter CurrentQuarter
        {
            set { currentQuarter = value; }
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

        protected override void LoadContent()
        {
            base.LoadContent();

            playerIcon = Game.Content.Load<Texture2D>("Textures/player");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (townGraphPicture != null)
                townGraphPicture.Dispose();

            playerIcon.Dispose();
        }
    }
}
