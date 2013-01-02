using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Space;
using ActionGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ActionGame.Components
{
    /// <summary>
    /// Drawing component provides rendering right drawing of drawable objects.
    /// </summary>
    public class Drawer : DrawableGameComponent
    {
        ActionGame game;
        List<DrawedObject> objects;
        List<DrawedObject> groundObjects;
        Matrix projectionMatrix;
        Matrix worldMatrix = Matrix.Identity;
        Texture2D townGraphPicture;
        Texture2D playerIcon;
        Texture2D townPanorama;
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
            objects = new List<DrawedObject>();
            groundObjects = new List<DrawedObject>();
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, resolutionWidth / resolutionHeight, 0.1f, 500);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public Matrix WorldTransformMatrix
        {
            get { return worldMatrix; }
        }

        public void StartDrawingSpatialObject(IDrawableObject obj, float azimuthDelta, Vector2 positionDelta, bool ground)
        {
            DrawedObject dObj = new DrawedObject(obj, azimuthDelta, positionDelta );
            if (ground)
                groundObjects.Add(dObj);
            else
                objects.Add(dObj);   
        }

        public void StopDrawingSpatialObject(IDrawableObject obj, bool ground)
        {
            if (ground)
            {
                groundObjects.RemoveAll(x => x.Object == obj);
            }
            else
            {
                objects.RemoveAll(x => x.Object == obj);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ///TODO: Maybe this should be called from other Update. For ex. Player's.
            this.ShowQuatterMap = Keyboard.GetState().IsKeyDown(Keys.M);
            this.ShowTownGraph = Keyboard.GetState().IsKeyDown(Keys.N);

            Debug.Write("Drawed objects", (groundObjects.Count + objects.Count).ToString());
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            DrawPanoramaBackground();


            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (DrawedObject dObj in groundObjects)
            {
                dObj.Object.Draw(game.Camera.ViewMatrix, projectionMatrix, dObj.TransformMatrix * worldMatrix);
            }

            foreach (DrawedObject dObj in objects)
            {
                dObj.Object.Draw(game.Camera.ViewMatrix, projectionMatrix, dObj.TransformMatrix * worldMatrix);
            }
            game.Player.Draw(game.Camera.ViewMatrix, projectionMatrix, worldMatrix);


            DrawMaps();
        }
        /// <summary>
        /// If the boolean flag is set draw town graph scheme or current quarter map.
        /// </summary>
        private void DrawMaps()
        {
            game.SpriteBatch.Begin();
            if (ShowQuatterMap)
            {
                Vector2 mapPosition = new Vector2((game.WindowWidth - currentQuarter.Map.Width) / 2, (game.WindowHeight - currentQuarter.Map.Height) / 2);
                game.SpriteBatch.Draw(currentQuarter.Map, mapPosition, Color.White);

                Vector2 playerPosition = new Vector2(
                    currentQuarter.Map.Width * (game.Player.PositionInQuarter.X / currentQuarter.QuarterSize.X) - playerIcon.Width / 2,
                    currentQuarter.Map.Height * (game.Player.PositionInQuarter.Z / currentQuarter.QuarterSize.Y) - playerIcon.Height / 2);
                playerPosition += mapPosition;
                game.SpriteBatch.Draw(playerIcon, playerPosition, null, Color.White, (float)game.Player.Azimuth, new Vector2(playerIcon.Width / 2, playerIcon.Height / 2), 1, SpriteEffects.None, 0);
            }
            if (ShowTownGraph && townGraphPicture != null)
            {
                game.SpriteBatch.Draw(townGraphPicture, new Vector2((game.WindowWidth - townGraphPicture.Width) / 2, (game.WindowHeight - townGraphPicture.Height) / 2), Color.White);
            }
            game.SpriteBatch.End();
        }
        /// <summary>
        /// Draws panoramatic town background.
        /// </summary>
        private void DrawPanoramaBackground()
        {
            game.SpriteBatch.Begin();
            float ratio = townPanorama.Height / (game.WindowHeight * 0.5f);
            double percentAngle = game.Player.Azimuth / MathHelper.TwoPi;
            game.SpriteBatch.Draw(townPanorama,
                new Rectangle(-(int)(percentAngle * game.WindowWidth), 0, game.WindowWidth, game.WindowHeight / 2),
                new Rectangle(0, 0, townPanorama.Width, townPanorama.Height),
                Color.White);
            game.SpriteBatch.Draw(townPanorama,
                new Rectangle(-(int)((percentAngle - 1) * game.WindowWidth), 0, game.WindowWidth, game.WindowHeight / 2),
                new Rectangle(0, 0, townPanorama.Width, townPanorama.Height),
                Color.White);
            game.SpriteBatch.End();
        }
        /// <summary>
        /// Sets current quarter.
        /// </summary>
        public TownQuarter CurrentQuarter
        {
            set { currentQuarter = value; }
        }
        /// <summary>
        /// Sets town graph scheme picture.
        /// </summary>
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
            townPanorama = Game.Content.Load<Texture2D>("Textures/panorama");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (townGraphPicture != null)
                townGraphPicture.Dispose();

            playerIcon.Dispose();
            townPanorama.Dispose();
        }
    }
}
