using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ActionGame.World;
using ActionGame.People;
using ActionGame.Components;
using ActionGame.Tasks;
using ActionGame.MenuForms;
using ActionGame.Tools;
using ActionGame.Extensions;
using System.Xml;
using System.Globalization;
using ActionGame.Space;

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
        Opponent opponent;
        readonly Dictionary<Human, PositionInTown> defaultPositions = new Dictionary<Human, PositionInTown>();
        Camera camera;
        Debug debug;
        Drawer drawer;
        Town town;
        readonly Random random = new Random();
        public Random Random
        {
            get
            {
                return random;
            }
        }


        public Town Town
        {
            get
            {
                return town;
            }
        }
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
        readonly List<GunType> guardDefaultGuns = new List<GunType>();
        public List<GunType> GuardDefaultGuns
        {
            get { return guardDefaultGuns; }
        }

        SoundEffectInstance backgroundSound;
        public Opponent Opponent { get { return opponent; } }

        readonly bool doInitialize = true;
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
                string sound = gunNode.SelectSingleNode("sound").InnerText;
                SoundEffect shotSound = Content.Load<SoundEffect>("Sounds/ToolActions/" + sound);
                bool infinity = bool.Parse(gunNode.SelectSingleNode("infinity").InnerText);
                TimeSpan shotTimeout = new TimeSpan(0,0,0,0,int.Parse(gunNode.SelectSingleNode("shotTimeout").InnerText));
                GunType gunType = new GunType(
                    int.Parse(gunNode.SelectSingleNode("damage").InnerText),
                    float.Parse(gunNode.SelectSingleNode("range").InnerText, CultureInfo.InvariantCulture.NumberFormat),
                    infinity,
                    shotTimeout,
                    (infinity ? 0 : int.Parse(gunNode.SelectSingleNode("defaultBulletCount").InnerText)),
                    icon,
                    shotSound
                    );
                gunTypes.Add(gunNode.Attributes["name"].Value, gunType);
                if (gunNode.Attributes["available"].Value == "human")
                    humanDefaultGuns.Add(gunType);
                if (gunNode.Attributes["available"].Value == "guard" || gunNode.Attributes["available"].Value == "player" || gunNode.Attributes["available"].Value == "box")
                {
                    boxDefaultGuns.Add(gunType);
                    if (gunNode.Attributes["available"].Value == "player" || gunNode.Attributes["available"].Value == "guard")
                    {
                        playerDefaultGuns.Add(gunType);
                        if (gunNode.Attributes["available"].Value == "guard")
                        {
                            guardDefaultGuns.Add(gunType);
                        }
                    }
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
                GameTime gameTime =new GameTime(TimeSpan.Zero, TimeSpan.Zero);
                loadingForm.Show();

                loadingForm.SetLabel("Loading graphics device...");
                spriteBatch = new SpriteBatch(GraphicsDevice);

                loadingForm.SetLabel("Loading gun types...");
                LoadGunTypes();

                player = new Player(this);
                opponent = new Opponent(this);

                town = new Town(this, loadingForm);

                loadingForm.SetLabel("Loading player...");
                loadingForm.SetValue(0);
                Point playerPoint =  town.CurrentQuarter.GetRandomSquare(s => s == MapFillType.Sidewalk);
                PositionInTown playerPosition = new PositionInTown(town.CurrentQuarter, playerPoint.ToVector2() * TownQuarter.SquareWidth + Vector2.One * 0.5f * TownQuarter.SquareWidth);
                defaultPositions.Add(player, playerPosition);
                player.Load(Content.Load<Model>("Objects/Humans/human0"), playerPosition, MathHelper.PiOver2, drawer.WorldTransformMatrix);
                town.CurrentQuarter.SpaceGrid.AddObject(player);
                town.CurrentQuarter.SetOwner(player, gameTime);
                player.AddEnemy(opponent);

                loadingForm.SetLabel("Loading opponent...");
                loadingForm.SetValue(0);
                /*TownQuarter oppQuarter = (from q in town.Quarters where q != town.CurrentQuarter orderby rand.Next() select q).First();
                Point oppPoint = oppQuarter.GetRandomSquare(s => s == MapFillType.Sidewalk);
                PositionInTown oppPosition = new PositionInTown(oppQuarter, oppPoint.ToVector2() * TownQuarter.SquareWidth);*/
                TownQuarter oppQuarter = town.CurrentQuarter;
                Point oppPoint = playerPoint;
                PositionInTown oppPosition = playerPosition;
                opponent.Load(Content.Load<Model>("Objects/Humans/human0"), oppPosition, 0, drawer.WorldTransformMatrix);
                oppQuarter.BeEnteredBy(opponent);
                oppQuarter.SetOwner(opponent, gameTime);
                //opponent.AddEnemy(player);
                foreach (var quarter in town.Quarters)
                {
                    opponent.AddTask(new ActionObjectTask(quarter.Flag, opponent));
                }
                Components.Add(town);


                BulletVisualisation.Texture = Content.Load<Texture2D>("Textures/halfWhite");
                backgroundSound = Content.Load<SoundEffect>("Sounds/background").CreateInstance();

                loadingForm.SetLabel("Content loaded. Get ready to play!");
                loadingForm.SetValue(100);
                loadingForm.Close();

                backgroundSound.IsLooped = true;
                backgroundSound.Play();
                drawer.ShowMessage(new GameTime(), String.Format("Wellcome in the game. You're in {0}.", player.Position.Quarter.Name));
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
                base.Update(gameTime);
                bool opponentInTheSameQuarter =opponent.Position.Quarter == town.CurrentQuarter;
                if (!opponentInTheSameQuarter)
                {
                    opponent.Position.Quarter.Update(gameTime, true);
                }
                opponent.Update(gameTime, !opponentInTheSameQuarter);
                player.Update(gameTime);
                if (player.Health <= 0)
                    Exit();
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

            if(backgroundSound != null) backgroundSound.Dispose();
        }
    }
}
