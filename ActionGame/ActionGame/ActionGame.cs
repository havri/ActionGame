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

namespace ActionGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ActionGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int resolutionWidth = 1680;
        const int resolutionHeight = 1024;
        const bool fullscreen = false;


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

        public ActionGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = resolutionWidth;
            graphics.PreferredBackBufferHeight = resolutionHeight;
            if(fullscreen)
                graphics.ToggleFullScreen();
            Content.RootDirectory = "Content";

            player = new Player();
            camera = new Camera(player, this);
            debug = new Debug(this);
            drawer = new Drawer(this, resolutionWidth, resolutionHeight);
            Components.Add(camera);
            Components.Add(drawer);
            Components.Add(debug);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            player.Load(Content.Load<Model>("Objects/Humans/human0"), new Vector3(10, 0, 10), 0, drawer.WorldTransformMatrix);

            town = new Town(this, 6, Content, drawer.WorldTransformMatrix, GraphicsDevice);
            drawer.TownGraphPicture = town.Map;
            Components.Add(town);

            
            /*TownQuarter quatter = new TownQuarter(new Vector2(200, 170), 6, Content, drawer.WorldTransformMatrix, GraphicsDevice);
            quatter.FillDrawer(drawer, MathHelper.Pi, new Vector2(10,10));*/

            /*Model grassModel = Content.Load<Model>("Objects/Flat/grass");
            for (int rx = 0; rx < 20; rx++)
            {
                for (int ry = 0; ry < 20; ry++)
                {
                    SpatialObject grass = new SpatialObject(grassModel, new Vector3(rx * grassModel.GetSize(drawer.WorldTransformMatrix).X, 0, ry * grassModel.GetSize(drawer.WorldTransformMatrix).Z), 0, drawer.WorldTransformMatrix);
                    drawer.StartDrawingObject(grass, true);
                }
            }

            SpatialObject house0 = new SpatialObject(Content.Load<Model>("Objects/Buildings/house0"), new Vector3(5, 0, 5), 0, drawer.WorldTransformMatrix);
            SpatialObject house1 = new SpatialObject(Content.Load<Model>("Objects/Buildings/house0"), new Vector3(20, 0, 20), 0, drawer.WorldTransformMatrix);

            drawer.StartDrawingObject(house0, false);
            drawer.StartDrawingObject(house1, false);*/
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

            player.Update(gameTime, resolutionWidth, resolutionHeight);

            base.Update(gameTime);
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

        public int WindowWidth
        {
            get { return resolutionWidth; }
        }

        public int WindowHeight
        {
            get { return resolutionHeight; }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            player.Dispose();
            town.Dispose();
            drawer.Dispose();
            camera.Dispose();
            debug.Dispose();
        }
    }
}
