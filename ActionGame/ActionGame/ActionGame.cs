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
using System.Xml;
using System.Globalization;

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

        Dictionary<string, GunType> gunTypes;
        readonly List<GunType> humanDefaultGuns = new List<GunType>();
        public List<GunType> HumanDefaultGuns
        {
            get { return humanDefaultGuns; }
        }
        readonly List<GunType> boxDefaultGuns = new List<GunType>();
        public List<GunType> BoxDefaultGuns
        {
            get { return boxDefaultGuns; }
        }
        readonly List<GunType> playerDefaultGuns = new List<GunType>();
        public List<GunType> PlayerDefaultGuns
        {
            get { return playerDefaultGuns; }
        }

        SoundEffectInstance backgroundSound;

        bool doInitialize = true;
        public ActionGame()
        {
            using (MainMenu mainMenuForm = new MainMenu(this))
            {
                if (mainMenuForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
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

        void LoadGunTypes()
        {
            string fileName = settings.GunSetFilename;
            XmlDocument doc = new XmlDocument();
            string fullPath = String.Format(@"Content\Config\{0}", fileName);
            if (!fullPath.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
                fullPath += ".xml";
            doc.Load(fullPath);
            var gunNodes = doc.SelectNodes("/guns/*");
            gunTypes = new Dictionary<string, GunType>(gunNodes.Count);
            foreach (XmlNode gunNode in gunNodes)
            {
                string texture = gunNode.SelectSingleNode("icon").InnerText;
                Texture2D icon = Content.Load<Texture2D>("Textures/ToolIcons/" + texture);
                bool infinity = bool.Parse(gunNode.SelectSingleNode("infinity").InnerText);
                GunType gunType = new GunType(
                    int.Parse(gunNode.SelectSingleNode("damage").InnerText),
                    float.Parse(gunNode.SelectSingleNode("range").InnerText, CultureInfo.InvariantCulture.NumberFormat),
                    infinity,
                    int.Parse(gunNode.SelectSingleNode("shotTimeout").InnerText),
                    (infinity ? 0 : int.Parse(gunNode.SelectSingleNode("defaultBulletCount").InnerText)),
                    icon
                    );
                gunTypes.Add(gunNode.Attributes["name"].Value, gunType);
                if (gunNode.Attributes["available"].Value == "human")
                    humanDefaultGuns.Add(gunType);
                if (gunNode.Attributes["available"].Value == "box" || gunNode.Attributes["available"].Value == "player")
                {
                    if (gunNode.Attributes["available"].Value == "player")
                    {
                        playerDefaultGuns.Add(gunType);
                    }
                    boxDefaultGuns.Add(gunType);
                }
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
                loadingForm.SetLabel("Loading gun types...");
                LoadGunTypes();
                player = new Player(this);
                town = new Town(this, loadingForm);
                loadingForm.SetLabel("Loading player...");
                loadingForm.SetValue(0);
                Point playerPoint =  town.CurrentQuarter.GetRandomSquare(s => s == MapFillType.Sidewalk);
                PositionInTown playerPosition = new PositionInTown(town.CurrentQuarter, playerPoint.ToVector2() * TownQuarter.SquareWidth);
                player.Load(Content.Load<Model>("Objects/Humans/human0"), playerPosition, MathHelper.PiOver2, drawer.WorldTransformMatrix);
                town.CurrentQuarter.SpaceGrid.AddObject(player);
                drawer.TownGraphPicture = town.Map;
                Components.Add(town);

                backgroundSound = Content.Load<SoundEffect>("Sounds/background").CreateInstance();

                loadingForm.SetLabel("Content loaded. Get ready to play!");
                loadingForm.SetValue(100);
                loadingForm.Close();

                backgroundSound.IsLooped = true;
                backgroundSound.Play();
            }
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
            spriteBatch.Dispose();
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
        }
    }
}
