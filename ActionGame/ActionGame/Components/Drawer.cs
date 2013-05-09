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
        public const float ViewDistance = 600f;
        static readonly TimeSpan MessageTimeout = new TimeSpan(0, 0, 0, 5);

        readonly HashSet<DrawedObject> objects = new HashSet<DrawedObject>();
        readonly Matrix projectionMatrix;
        readonly Matrix worldMatrix = Matrix.Identity;
        Texture2D toolPanelBackground;
        Texture2D playerIcon;
        Texture2D actionAvailableIcon;
        TownQuarter currentQuarter;
        SpriteFont font;
        SpatialObject panorama;
        TimeSpan messageShowBegin = TimeSpan.Zero;
        bool showMessage = false;
        string message = string.Empty;
        Texture2D messageBackground;
        readonly HashSet<ProgressBar> progressBars = new HashSet<ProgressBar>();


        /// <summary>
        /// Constructs new drawing component.
        /// </summary>
        /// <param name="game">Game for context</param>
        /// <param name="resolutionWidth">Window width</param>
        /// <param name="resolutionHeight">Window height</param>
        public Drawer(ActionGame game, float resolutionWidth, float resolutionHeight)
            : base(game)
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, resolutionWidth / resolutionHeight, 0.1f, ViewDistance);
        }

        protected new ActionGame Game
        {
            get
            {
                return (ActionGame)base.Game;
            }
        }

        /// TODO: remove this
        public static Texture2D Blue__, Green__, Red__;
        public override void Initialize()
        {
            base.Initialize();
            Blue__ = Game.Content.Load<Texture2D>("Textures/blue");
            Red__ = Game.Content.Load<Texture2D>("Textures/red");
            Green__ = Game.Content.Load<Texture2D>("Textures/green");
        }

        public Matrix WorldTransformMatrix
        {
            get { return worldMatrix; }
        }

        public void StartDrawingObject(IDrawableObject obj, float azimuthDelta, Vector2 positionDelta)
        {
            DrawedObject dObj = new DrawedObject(obj, azimuthDelta, positionDelta);
            objects.Add(dObj);
        }

        public void StopDrawingObject(IDrawableObject obj)
        {
            objects.RemoveWhere(dObj => dObj.Object == obj);
        }

        public void ShowMessage(GameTime gameTime, String text)
        {
            showMessage = true;
            messageShowBegin = gameTime.TotalGameTime;
            message = text;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (gameTime.TotalGameTime - messageShowBegin > MessageTimeout)
            {
                showMessage = false;
            }

            ///TODO: Maybe this should be called from other Update. For ex. Player's.
            ShowQuatterMap = Keyboard.GetState().IsKeyDown(Keys.M);
            ShowTownGraph = Keyboard.GetState().IsKeyDown(Keys.N);

            Debug.Write("Drawed objects", objects.Count.ToString());
        }

        public ProgressBar CreateProgressBar(Texture2D texture)
        {
            ProgressBar pb = new ProgressBar(texture);
            progressBars.Add(pb);
            return pb;
        }

        public void DestroyProgressBar(ProgressBar progressBar)
        {
            progressBars.Remove(progressBar);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            panorama.Draw(Game.Camera.ViewMatrix, projectionMatrix, worldMatrix);

            foreach (DrawedObject dObj in objects)
            {
                dObj.Object.Draw(Game.Camera.ViewMatrix, projectionMatrix, dObj.TransformMatrix * worldMatrix);
            }
            Game.Player.Draw(Game.Camera.ViewMatrix, projectionMatrix, worldMatrix);

            Game.SpriteBatch.Begin();
            Game.SpriteBatch.Draw(toolPanelBackground, new Vector2(0, 0), Color.AliceBlue);
            Game.SpriteBatch.DrawString(font, Game.Player.Health.ToString(), new Vector2(20, 80), Color.Maroon);
            if (Game.Player.SelectedTool != null)
            {
                Game.SpriteBatch.Draw(Game.Player.SelectedTool.Icon, new Vector2(100, 0), Color.AliceBlue);
                Game.SpriteBatch.DrawString(font, Game.Player.SelectedTool.ToolBarText.ToString(), new Vector2(120, 80), Color.Black);
            }
            if (Game.Player.HasAvailableAnyAction)
            {
                Game.SpriteBatch.Draw(actionAvailableIcon, new Vector2(0, Game.Settings.ScreenSize.Height - 100), Color.White);
            }
            {
                int i = 0;
                foreach (ProgressBar prograssBar in progressBars)
                {
                    const float height = 0.05f;
                    Rectangle destRect = new Rectangle(0, (int)((i++) * height * Game.Settings.ScreenSize.Height), Game.Settings.ScreenSize.Width, (int)(height * Game.Settings.ScreenSize.Height));
                    prograssBar.Draw(Game.SpriteBatch, destRect);
                }
            }
            if (showMessage)
            {
                const float height = 0.1f;
                const float width = 0.7f;
                Rectangle destRect = new Rectangle((int)((1f - width) * 0.5f * Game.Settings.ScreenSize.Width), 0, (int)(Game.Settings.ScreenSize.Width * width), (int)(Game.Settings.ScreenSize.Height * height));
                Game.SpriteBatch.Draw(messageBackground, destRect, Color.White);
                Game.SpriteBatch.DrawString(font, message, destRect.Location.ToVector2() + new Vector2(2, 1), Color.Black);
            }
            Vector2 crossSize = new Vector2(Game.ContentRepository.Cross.Bounds.Width, Game.ContentRepository.Cross.Bounds.Height);
            Game.SpriteBatch.Draw(Game.ContentRepository.Cross, new Vector2(0.5f * Game.Settings.ScreenSize.Width, 0.5f * Game.Settings.ScreenSize.Height) - 0.5f*crossSize, Color.White);
            Game.SpriteBatch.End();

            DrawMaps();
        }
        /// <summary>
        /// If the boolean flag is set draw town graph scheme or current quarter map.
        /// </summary>
        private void DrawMaps()
        {
            Game.SpriteBatch.Begin();
            if (ShowQuatterMap)
            {
                Vector2 mapPosition = new Vector2((Game.Settings.ScreenSize.Width - currentQuarter.Map.Width) / 2, (Game.Settings.ScreenSize.Height - currentQuarter.Map.Height) / 2);
                Game.SpriteBatch.Draw(currentQuarter.Map, mapPosition, Color.White);

                Vector2 playerPosition = new Vector2(
                    currentQuarter.Map.Width * (Game.Player.PositionInQuarter.X / currentQuarter.QuarterSize.X) - playerIcon.Width / 2,
                    currentQuarter.Map.Height * (Game.Player.PositionInQuarter.Z / currentQuarter.QuarterSize.Y) - playerIcon.Height / 2);
                playerPosition += mapPosition;
                Game.SpriteBatch.Draw(playerIcon, playerPosition, null, Color.White, (float)Game.Player.Azimuth, new Vector2(playerIcon.Width / 2, playerIcon.Height / 2), 1, SpriteEffects.None, 0);
            }
            if (ShowTownGraph)
            {
                Texture2D townGraphPicture = Game.Town.CreateTownMap();
                Game.SpriteBatch.Draw(townGraphPicture, new Vector2((Game.Settings.ScreenSize.Width - townGraphPicture.Width) / 2, (Game.Settings.ScreenSize.Height - townGraphPicture.Height) / 2), Color.White);
            }
            Game.SpriteBatch.End();
        }
        /// <summary>
        /// Sets current quarter.
        /// </summary>
        public TownQuarter CurrentQuarter
        {
            set { currentQuarter = value; }
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
            actionAvailableIcon = Game.Content.Load<Texture2D>("Textures/actionIcon");
            font = Game.Content.Load<SpriteFont>("Fonts/SpriteFont1");
            panorama = new SpatialObject(Game.Content.Load<Model>("Objects/panorama"), null, new Vector3(0, -1,0), 0, worldMatrix);
            messageBackground = Game.Content.Load<Texture2D>("Textures/green");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(playerIcon != null)
                playerIcon.Dispose();
        }
    }
}
