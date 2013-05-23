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
using System.Text.RegularExpressions;

namespace ActionGame.Components
{
    /// <summary>
    /// Drawing component provides rendering right drawing of drawable objects.
    /// </summary>
    public class Drawer : DrawableGameComponent
    {
        public const float ViewDistance = 1000f;
        static readonly TimeSpan MessageTimeout = new TimeSpan(0, 0, 0, 5);

        readonly Dictionary<ITransformedDrawable, Matrix> objects = new Dictionary<ITransformedDrawable, Matrix>();
        readonly Matrix projectionMatrix;
        readonly Matrix worldMatrix = Matrix.Identity;
        Texture2D toolPanelBackground;
        Texture2D playerIcon;
        Texture2D actionAvailableIcon;
        Texture2D tmpTownMap;
        TownQuarter currentQuarter;
        SpriteFont font;
        SpatialObject panorama;
        TimeSpan messageShowBegin = TimeSpan.Zero;
        bool showMessage = false;
        string message = string.Empty;
        Texture2D messageBackground;
        readonly HashSet<ProgressBar> progressBars = new HashSet<ProgressBar>();
        Texture2D fullscreenEffect;
        bool showFullscreenEffect = false;
        TimeSpan fullscreenEffectBegin = TimeSpan.Zero;
        TimeSpan fullscreenEffectDuration = TimeSpan.Zero;


        /// <summary>
        /// Constructs new drawing component.
        /// </summary>
        /// <param name="game">Game for the context</param>
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

        /// <summary>
        /// Initializes the component the drawer.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Gets the world transformation matrix. It is the identity matrix in our game.
        /// </summary>
        public Matrix WorldTransformMatrix
        {
            get { return worldMatrix; }
        }

        /// <summary>
        /// Starts drawing an object with specified transformation.
        /// </summary>
        /// <param name="obj">The object for drawing</param>
        /// <param name="azimuthDelta">Y rotation</param>
        /// <param name="positionDelta">Translate delta</param>
        public void StartDrawingObject(ITransformedDrawable obj, float azimuthDelta, Vector2 positionDelta)
        {
            Matrix transform = Matrix.CreateRotationY(-azimuthDelta) * Matrix.CreateTranslation(positionDelta.ToVector3(0));
            objects.Add(obj, transform);
        }

        /// <summary>
        /// Ends drawing of specified object.
        /// </summary>
        /// <param name="obj">The drawed object</param>
        public void StopDrawingObject(ITransformedDrawable obj)
        {
            objects.Remove(obj);
        }

        /// <summary>
        /// Show message on the screen for a while.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="text">The message</param>
        public void ShowMessage(GameTime gameTime, String text)
        {
            showMessage = true;
            messageShowBegin = gameTime.TotalGameTime;
            message = text;
        }
        /// <summary>
        /// Blinks the specified texture on the screen.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="effect">The texture</param>
        /// <param name="duration">Blink duration</param>
        public void ShowFullscreenEffect(GameTime gameTime, Texture2D effect, TimeSpan duration)
        {
            fullscreenEffect = effect;
            fullscreenEffectDuration = duration;
            fullscreenEffectBegin = gameTime.TotalGameTime;
            showFullscreenEffect = true;
        }

        /// <summary>
        /// Updates the drawing logic.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (showMessage && gameTime.TotalGameTime - messageShowBegin > MessageTimeout)
            {
                showMessage = false;
            }
            if (showFullscreenEffect && gameTime.TotalGameTime - fullscreenEffectBegin > fullscreenEffectDuration)
            {
                showFullscreenEffect = false;
            }

            panorama.MoveTo(Game.Player.Position.PositionInQuarter - panorama.Size.XZToVector2() * 0.5f, panorama.Azimuth);

            ShowQuatterMap = Keyboard.GetState().IsKeyDown(Game.Settings.ShowQuarterMap);
            ShowTownGraph = Keyboard.GetState().IsKeyDown(Game.Settings.ShowTownMap);

            Debug.Write("Drawed objects", objects.Count.ToString());
        }

        /// <summary>
        /// Creates a new progress bar for drawing.
        /// </summary>
        /// <param name="texture">Filling texture for the bar</param>
        /// <returns>A progress bar instance</returns>
        public ProgressBar CreateProgressBar(Texture2D texture)
        {
            ProgressBar pb = new ProgressBar(texture);
            progressBars.Add(pb);
            return pb;
        }

        /// <summary>
        /// Cancels drawing of specified progress bar.
        /// </summary>
        /// <param name="progressBar">The canceled progress bar</param>
        public void DestroyProgressBar(ProgressBar progressBar)
        {
            progressBars.Remove(progressBar);
        }

        /// <summary>
        /// Draws the whole game scene - every object, messages, effects...
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            panorama.Draw(Game.Camera.ViewMatrix, projectionMatrix, worldMatrix);

            foreach (KeyValuePair<ITransformedDrawable, Matrix> dObj in objects)
            {
                dObj.Key.Draw(Game.Camera.ViewMatrix, projectionMatrix, dObj.Value * worldMatrix);
            }
            Game.Player.Draw(Game.Camera.ViewMatrix, projectionMatrix, worldMatrix);

            Game.SpriteBatch.Begin();
            if (Game.Player.InGodMode)
            {
                Game.SpriteBatch.Draw(Game.ContentRepository.GodModeFullscreenEffect, new Rectangle(0, 0, Game.Settings.ScreenSize.Width, Game.Settings.ScreenSize.Height), Color.White);
            }
            if (showFullscreenEffect)
            {
                float timeRatio = (float)((gameTime.TotalGameTime - fullscreenEffectBegin).TotalSeconds / fullscreenEffectDuration.TotalSeconds);
                float alpha = 1f - timeRatio * timeRatio;
                Game.SpriteBatch.Draw(fullscreenEffect, new Rectangle(0, 0, Game.Settings.ScreenSize.Width, Game.Settings.ScreenSize.Height), Color.White * alpha);
            }
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
                if (tmpTownMap == null)
                {
                    tmpTownMap = Game.Town.CreateTownMap();
                }
                Game.SpriteBatch.Draw(tmpTownMap, new Vector2((Game.Settings.ScreenSize.Width - tmpTownMap.Width) / 2, (Game.Settings.ScreenSize.Height - tmpTownMap.Height) / 2), Color.White);
            }
            else if (tmpTownMap != null)
            {
                tmpTownMap.Dispose();
                tmpTownMap = null;
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

        /// <summary>
        /// Gets or sets whether the quarter map has to be shown.
        /// </summary>
        public bool ShowQuatterMap
        { get; set; }
        /// <summary>
        /// Gets or sets whether the town map has to be shown.
        /// </summary>
        public bool ShowTownGraph
        { get; set; }

        /// <summary>
        /// Loads its needed content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            playerIcon = Game.Content.Load<Texture2D>("Textures/player");
            toolPanelBackground = Game.Content.Load<Texture2D>("Textures/toolPanel");
            actionAvailableIcon = Game.Content.Load<Texture2D>("Textures/actionIcon");
            font = Game.Content.Load<SpriteFont>("Fonts/SpriteFont1");
            Model panoramaModel = Game.Content.Load<Model>("Objects/panorama");
            Vector3 pDiff = panoramaModel.GetSize(worldMatrix) * 0.5f;
            pDiff.Y = 0;
            panorama = new SpatialObject(panoramaModel, null, new Vector3(0, -5,0) - pDiff, 0, worldMatrix);
            
            messageBackground = Game.Content.Load<Texture2D>("Textures/green");
        }

        /// <summary>
        /// Moves the panorama surrounding object.
        /// </summary>
        /// <param name="positionDelta">Translate delta</param>
        /// <param name="azimuthDelta">Y rotation angle</param>
        public void MovePanorama(Vector2 positionDelta, float azimuthDelta)
        {
            panorama.MoveTo(Game.Player.Position.PositionInQuarter - panorama.Size.XZToVector2() * 0.5f, panorama.Azimuth + azimuthDelta);
        }

        /// <summary>
        /// Disposes the drawing component.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(playerIcon != null)
                playerIcon.Dispose();
        }
    }
}
