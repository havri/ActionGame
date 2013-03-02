using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ActionGame.World;
using ActionGame.People;
using ActionGame.Components;
using ActionGame.Tasks;
using ActionGame.MenuForms;
using ActionGame.Tools;
using ActionGame.Extensions;
using System.Threading;

namespace ActionGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ActionGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        public GameSettings Settings
        {
            get
            {
                return settings;
            }
        }
        GameSettings settings;


        Player player;
        Camera camera;
        Debug debug;
        Drawer drawer;
        Town town;

        /// <summary>
        /// Gets active player.
        /// </summary>
        public Player Player
        {
            get { return player; }
        }
        /// <summary>
        /// Gets the game SpriteBatch.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }
        /// <summary>
        /// Gets the game camera component.
        /// </summary>
        public Camera Camera
        {
            get { return camera; }
        }
        /// <summary>
        /// Gets the game drawer component.
        /// </summary>
        public Drawer Drawer
        {
            get
            {
                return drawer;
            }
        }

        bool doInitialize = true;
        public ActionGame()
        {
            using (MainMenu mainMenuForm = new MainMenu())
            {
                if (mainMenuForm.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    doInitialize = false;
                    Exit();
                }
                else
                {
                    settings = mainMenuForm.Settings;
                }
            }

            if (doInitialize)
            {
                graphics = new GraphicsDeviceManager(this)
                {
                    PreferredBackBufferWidth = settings.ScreenSize.Width,
                    PreferredBackBufferHeight = settings.ScreenSize.Height
                };
                if (settings.Fullscreen)
                    graphics.ToggleFullScreen();
                Content.RootDirectory = "Content";



                camera = new Camera(this);
                debug = new Debug(this);
                drawer = new Drawer(this, settings.ScreenSize.Width, settings.ScreenSize.Height);
                Components.Add(camera);
                Components.Add(drawer);
                Components.Add(debug);
            }
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            if (doInitialize)
            {
                Human.Fists = new GunType(10, 0.5f, true, Content.Load<Texture2D>("Textures/ToolIcons/fists"));
                player = new Player(this);
                base.Initialize();
            }
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            using (Loading loadingForm = new Loading())
            {
                loadingForm.Show();
                loadingForm.SetLabel("Loading graphics device...");
                spriteBatch = new SpriteBatch(GraphicsDevice);
                town = new Town(this, loadingForm);
                loadingForm.SetLabel("Loading player...");
                loadingForm.SetValue(0);
                Point playerPoint =  town.CurrentQuarter.GetRandomSquare(s => s == MapFillType.Sidewalk);
                PositionInTown playerPosition = new PositionInTown(town.CurrentQuarter, playerPoint.ToVector2() * TownQuarter.SquareWidth);
                player.Load(Content.Load<Model>("Objects/Humans/human0"), playerPosition, MathHelper.PiOver2, drawer.WorldTransformMatrix);
                town.CurrentQuarter.SpaceGrid.AddObject(player);
                drawer.TownGraphPicture = town.Map;
                Components.Add(town);

                loadingForm.SetLabel("Content loaded. Get ready to play!");
                loadingForm.SetValue(100);
                loadingForm.Close();
            }
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            base.UnloadContent();
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if(doInitialize)
            {
                player.Update(gameTime);
                base.Update(gameTime);
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer ,Color.CornflowerBlue, 1, 1);
            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(player != null) player.Dispose();
            if(town != null) town.Dispose();
            drawer.Dispose();
            camera.Dispose();
            debug.Dispose();
        }
    }
}
