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

        const int resolutionWidth = 1024;
        const int resolutionHeight = 800;
        const bool fullscreen = false;

        Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)resolutionWidth / (float)resolutionHeight, float.Epsilon, 1000);
        Matrix worldMatrix = Matrix.Identity;

        Player player;

        Camera camera;
        Debug debug;

        List<SpatialObject> DrawableObjects = new List<SpatialObject>();

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
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
            Components.Add(camera);
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

            player.Load(Content.Load<Model>("Objects/Humans/human0"), new Vector3(0, 0, 0), 0, worldMatrix);
            Model grassModel = Content.Load<Model>("Objects/Flat/grass");
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    SpatialObject grass = new SpatialObject(grassModel, new Vector3(x * grassModel.GetSize(worldMatrix).X, 0, y * grassModel.GetSize(worldMatrix).Z), 0, worldMatrix);
                    DrawableObjects.Add(grass);
                }
            }

            SpatialObject house = new SpatialObject(Content.Load<Model>("Objects/Buildings/house0"), new Vector3(5, 0, 5), 0, worldMatrix);

            DrawableObjects.Add(player);
            DrawableObjects.Add(house);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawableObjects.ForEach(obj => obj.Draw(camera.ViewMatrix, projectionMatrix, worldMatrix));
            

            base.Draw(gameTime);
        }
    }
}
