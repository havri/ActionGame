using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Space;
using ActionGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ActionGame.Extensions;
using ActionGame.People;

namespace ActionGame.Components
{
    /// <summary>
    /// Drawing component provides rendering right drawing of drawable objects.
    /// </summary>
    public class Drawer : DrawableGameComponent
    {
        readonly ActionGame game;
        readonly List<DrawedObject> objects;
        readonly Matrix projectionMatrix;
        readonly Matrix worldMatrix = Matrix.Identity;
        Texture2D toolPanelBackground;
        Texture2D townGraphPicture;
        Texture2D playerIcon;
        TownQuarter currentQuarter;
        SpriteFont font;
        SpatialObject panorama;

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
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, resolutionWidth / resolutionHeight, 0.1f, 600);
        }

        public static Texture2D Blue__, Green__, Red__;
        public override void Initialize()
        {
            base.Initialize();
            Blue__ = game.Content.Load<Texture2D>("Textures/blue");
            Red__ = game.Content.Load<Texture2D>("Textures/red");
            Green__ = game.Content.Load<Texture2D>("Textures/green");
        }

        public Matrix WorldTransformMatrix
        {
            get { return worldMatrix; }
        }

        public void StartDrawingObject(IDrawableObject obj, float azimuthDelta, Vector2 positionDelta)
        {
            DrawedObject dObj = new DrawedObject(obj, azimuthDelta, positionDelta);
            objects.Add(dObj);

            if (obj is SpatialObject && !(obj is Human))
            {
                GameObject q = obj as GameObject;
                Tuple<Vector2, Texture2D>[] corners = new Tuple<Vector2, Texture2D>[]
                {
                    new Tuple<Vector2, Texture2D>(q.UpperLeftCorner,Drawer.Blue__),
                    new Tuple<Vector2, Texture2D>(q.UpperRightCorner,Drawer.Blue__),
                    new Tuple<Vector2, Texture2D>(q.LowerLeftCorner,Drawer.Blue__),
                    new Tuple<Vector2, Texture2D>(q.LowerRightCorner,Drawer.Blue__),

                    //new Tuple<Vector2, Texture2D>(q.Pivot.PositionInQuarter,Drawer.Green__),

                    //new Tuple<Vector2, Texture2D>(q.Position.PositionInQuarter,Drawer.Red__)
                };

                foreach (Tuple<Vector2, Texture2D> corner in corners)
                {
                    const float pointHeight = 0.02f;
                    const float radius = 0.15f;
                    Plate vplate = new Plate(
                        q.Position.Quarter,
                        corner.Item1.Go(radius, 0).ToVector3(pointHeight),
                        corner.Item1.Go(radius, MathHelper.PiOver2).ToVector3(pointHeight),
                        corner.Item1.Go(radius, -MathHelper.PiOver2).ToVector3(pointHeight),
                        corner.Item1.Go(radius, MathHelper.Pi).ToVector3(pointHeight),
                        corner.Item2,
                        corner.Item2);
                    objects.Add(new DrawedObject(vplate, azimuthDelta, positionDelta));
                }
            }
        }

        public void StopDrawingObject(IDrawableObject obj)
        {
            objects.RemoveAll(x => x.Object == obj);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ///TODO: Maybe this should be called from other Update. For ex. Player's.
            ShowQuatterMap = Keyboard.GetState().IsKeyDown(Keys.M);
            ShowTownGraph = Keyboard.GetState().IsKeyDown(Keys.N);

            Debug.Write("Drawed objects", objects.Count.ToString());
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            panorama.Draw(game.Camera.ViewMatrix, projectionMatrix, worldMatrix);

            //DrawPanoramaBackground();


            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (DrawedObject dObj in objects)
            {
                dObj.Object.Draw(game.Camera.ViewMatrix, projectionMatrix, dObj.TransformMatrix * worldMatrix);
            }
            game.Player.Draw(game.Camera.ViewMatrix, projectionMatrix, worldMatrix);

            game.SpriteBatch.Begin();
            game.SpriteBatch.Draw(toolPanelBackground, new Vector2(0, 0), Color.AliceBlue);
            game.SpriteBatch.DrawString(font, game.Player.Health.ToString(), new Vector2(20, 80), Color.Maroon);
            if (game.Player.SelectedTool != null)
            {
                game.SpriteBatch.Draw(game.Player.SelectedTool.Icon, new Vector2(100, 0), Color.AliceBlue);
                game.SpriteBatch.DrawString(font, game.Player.SelectedTool.ToolBarText.ToString(), new Vector2(120, 80), Color.Black);
            }
            game.SpriteBatch.End();

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
                Vector2 mapPosition = new Vector2((game.Settings.ScreenSize.Width - currentQuarter.Map.Width) / 2, (game.Settings.ScreenSize.Height - currentQuarter.Map.Height) / 2);
                game.SpriteBatch.Draw(currentQuarter.Map, mapPosition, Color.White);

                Vector2 playerPosition = new Vector2(
                    currentQuarter.Map.Width * (game.Player.PositionInQuarter.X / currentQuarter.QuarterSize.X) - playerIcon.Width / 2,
                    currentQuarter.Map.Height * (game.Player.PositionInQuarter.Z / currentQuarter.QuarterSize.Y) - playerIcon.Height / 2);
                playerPosition += mapPosition;
                game.SpriteBatch.Draw(playerIcon, playerPosition, null, Color.White, (float)game.Player.Azimuth, new Vector2(playerIcon.Width / 2, playerIcon.Height / 2), 1, SpriteEffects.None, 0);
            }
            if (ShowTownGraph && townGraphPicture != null)
            {
                game.SpriteBatch.Draw(townGraphPicture, new Vector2((game.Settings.ScreenSize.Width - townGraphPicture.Width) / 2, (game.Settings.ScreenSize.Height - townGraphPicture.Height) / 2), Color.White);
            }
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
            toolPanelBackground = Game.Content.Load<Texture2D>("Textures/toolPanel");
            font = game.Content.Load<SpriteFont>("Fonts/SpriteFont1");
            panorama = new SpatialObject(game.Content.Load<Model>("Objects/panorama"), null, Vector3.Zero, 0, worldMatrix);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (townGraphPicture != null)
                townGraphPicture.Dispose();
            if(playerIcon != null)
                playerIcon.Dispose();
        }
    }
}
