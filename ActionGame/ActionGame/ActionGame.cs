//Needs define in Debug.cs too
//#define debug
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
    /// This is the main type in the game.
    /// </summary>
    public class ActionGame : Microsoft.Xna.Framework.Game
    {
        ContentRepository contentRepository;
        /// <summary>
        /// Gets the repository of all the content in the game - models, textures, sounds...
        /// </summary>
        public ContentRepository ContentRepository
        {
            get
            {
                return contentRepository;
            }
        }
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// Gets the settings of this game run.
        /// </summary>
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
        Camera camera;
        Debug debug;
        Drawer drawer;
        Town town;
        SoundPlayer soundPlayer;
        readonly Random random = new Random();
        /// <summary>
        /// Gets the shared random generator.
        /// </summary>
        public Random Random
        {
            get
            {
                return random;
            }
        }

        /// <summary>
        /// Gets the town entity representing the whole game world.
        /// </summary>
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
        /// <summary>
        /// Gets the sound player component
        /// </summary>
        public SoundPlayer SoundPlayer
        {
            get
            {
                return soundPlayer;
            }
        }

        Dictionary<string, GunType> gunTypes;
        readonly List<GunType> humanDefaultGuns = new List<GunType>();
        /// <summary>
        /// Gets the list of guns that every human has at the beginnig.
        /// </summary>
        public List<GunType> HumanDefaultGuns
        {
            get { return humanDefaultGuns; }
        }
        readonly List<GunType> boxDefaultGuns = new List<GunType>();
        /// <summary>
        /// Gets the list of guns which are available only in boxes
        /// </summary>
        public List<GunType> BoxDefaultGuns
        {
            get { return boxDefaultGuns; }
        }
        readonly List<GunType> playerDefaultGuns = new List<GunType>();
        /// <summary>
        /// Gets the list of guns that both players have at the beginning.
        /// </summary>
        public List<GunType> PlayerDefaultGuns
        {
            get { return playerDefaultGuns; }
        }
        readonly List<GunType> guardDefaultGuns = new List<GunType>();
        /// <summary>
        /// Gets the list of guns that every guard has after his born.
        /// </summary>
        public List<GunType> GuardDefaultGuns
        {
            get { return guardDefaultGuns; }
        }

        SoundEffectInstance backgroundSound;
        /// <summary>
        /// Gets the opponent.
        /// </summary>
        public Opponent Opponent { get { return opponent; } }

        public new GraphicsDevice GraphicsDevice
        {
            get { return graphics.GraphicsDevice; }
        }

        bool running = true;
        /// <summary>
        /// Creates instance of the whole game.
        /// </summary>
        public ActionGame()
        {
            Content.RootDirectory = "Content";
            ShowMainMenuDialog(true);
            if (running)
            {
                graphics = new GraphicsDeviceManager(this)
                {
                    PreferredBackBufferWidth = settings.ScreenSize.Width,
                    PreferredBackBufferHeight = settings.ScreenSize.Height,
                    IsFullScreen = false
                };
            }
            PreInitialize();
        }

        /// <summary>
        /// Prepares the game for its initialization.
        /// </summary>
        private void PreInitialize()
        {
            contentRepository = new ContentRepository(this);
            if (running)
            {
                camera = new Camera(this);
#if debug
                debug = new Debug(this);
#endif
                drawer = new Drawer(this, settings.ScreenSize.Width, settings.ScreenSize.Height);
                soundPlayer = new SoundPlayer(this);
                Components.Clear();
                Components.Add(camera);
                Components.Add(drawer);
#if degub
                //Components.Add(debug);
#endif
            }
        }

        /// <summary>
        /// Shows the main menu window and handles its output - the settings and whether the game should start.
        /// </summary>
        /// <param name="videoEnabled">Enables the video settings tab</param>
        private void ShowMainMenuDialog(bool videoEnabled)
        {
            this.IsMouseVisible = true;
            using (MainMenu mainMenuForm = new MainMenu(this, videoEnabled))
            {
                if (mainMenuForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    running = false;
                    Exit();
                }
                else
                {
                    running = true;
                    settings = mainMenuForm.Settings;
                }
            }
            this.IsMouseVisible = false;
        }

        /// <summary>
        /// Loads gun types setting from attached xml file.
        /// </summary>
        void LoadGunTypes()
        {
            try
            {
                string fileName = settings.GunSetFilename;
                XmlDocument doc = new XmlDocument();
                string fullPath = String.Format(@"Content\Config\{0}", fileName);
                if (!fullPath.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
                    fullPath += ".xml";
                doc.Load(fullPath);
                XmlNodeList gunNodes = doc.SelectNodes("/guns/*");
                gunTypes = new Dictionary<string, GunType>(gunNodes.Count);
                guardDefaultGuns.Clear();
                boxDefaultGuns.Clear();
                humanDefaultGuns.Clear();
                playerDefaultGuns.Clear();
                foreach (XmlNode gunNode in gunNodes)
                {
                    string texture = gunNode.SelectSingleNode("icon").InnerText;
                    Texture2D icon = Content.Load<Texture2D>("Textures/ToolIcons/" + texture);
                    string sound = gunNode.SelectSingleNode("sound").InnerText;
                    SoundEffect shotSound = Content.Load<SoundEffect>("Sounds/ToolActions/" + sound);
                    bool infinity = bool.Parse(gunNode.SelectSingleNode("infinity").InnerText);
                    TimeSpan shotTimeout = new TimeSpan(0, 0, 0, 0, int.Parse(gunNode.SelectSingleNode("shotTimeout").InnerText));
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
            catch
            {
                Exit();
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
            if (running)
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
            if (running)
            {
                using (Loading loadingForm = new Loading())
                {
                    GameTime gameTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);
                    loadingForm.Show();

                    loadingForm.SetLabel("Loading graphics device tools...");
                    spriteBatch = new SpriteBatch(GraphicsDevice);

                    loadingForm.SetLabel("Loading content...");
                    contentRepository.LoadContent();

                    loadingForm.SetLabel("Loading gun types...");
                    LoadGunTypes();

                    player = new Player(this);
                    opponent = new Opponent(this);

                    town = new Town(this, loadingForm);

                    loadingForm.SetLabel("Loading player...");
                    loadingForm.SetValue(0);
                    Point playerPoint = town.CurrentQuarter.GetRandomSquare(s => s == MapFillType.Sidewalk);
                    PositionInTown playerPosition = new PositionInTown(town.CurrentQuarter, playerPoint.ToVector2() * TownQuarter.SquareWidth + Vector2.One * 0.5f * TownQuarter.SquareWidth);
                    player.Load(contentRepository.Player, playerPosition, MathHelper.PiOver2, drawer.WorldTransformMatrix);
                    town.CurrentQuarter.SpaceGrid.AddObject(player);
                    town.CurrentQuarter.SetOwner(player, gameTime);
                    player.AddEnemy(opponent);

                    loadingForm.SetLabel("Loading opponent...");
                    loadingForm.SetValue(0);
                    TownQuarter oppQuarter = (from q in town.Quarters where q != town.CurrentQuarter orderby random.Next() select q).First();
                    Point oppPoint = oppQuarter.GetRandomSquare(s => s == MapFillType.Sidewalk);
                    PositionInTown oppPosition = new PositionInTown(oppQuarter, oppPoint.ToVector2() * TownQuarter.SquareWidth);
                    opponent.Load(contentRepository.Opponent, oppPosition, 0, drawer.WorldTransformMatrix);
                    oppQuarter.BeEnteredBy(opponent);
                    oppQuarter.SetOwner(opponent, gameTime);
                    opponent.AddEnemy(player);
                    Components.Add(town);


                    BulletVisualisation.Texture = Content.Load<Texture2D>("Textures/white");
                    backgroundSound = Content.Load<SoundEffect>("Sounds/background").CreateInstance();

                    loadingForm.SetLabel("Cleaning memory...");
                    loadingForm.SetValue(0);
                    GC.Collect();
                    loadingForm.SetValue(100);

                    loadingForm.SetLabel("Content loaded. Get ready to play!");
                    loadingForm.SetValue(100);
                    loadingForm.Close();

                    backgroundSound.IsLooped = true;
                    backgroundSound.Play();
                    drawer.ShowMessage(new GameTime(), String.Format("Wellcome in the game. You're in {0}.", player.Position.Quarter.Name));
                }
                if (settings.Fullscreen)
                {
                    graphics.ToggleFullScreen();
                }
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
            if(running)
            {
                base.Update(gameTime);
                Debug.Write("UPS", 1d / gameTime.ElapsedGameTime.TotalSeconds);

                // Allows the game to exit
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    this.Restart();
                    return;
                }
                bool opponentInTheSameQuarter = opponent.Position.Quarter == town.CurrentQuarter;
                if (!opponentInTheSameQuarter)
                {
                    opponent.Position.Quarter.Update(gameTime, true);
                }
                if (town.SecondaryDrawnQuarter != null && town.SecondaryDrawnQuarter != opponent.Position.Quarter && town.SecondaryDrawnQuarter != player.Position.Quarter && !gameTime.IsRunningSlowly)
                {
                    town.SecondaryDrawnQuarter.Update(gameTime, false);
                }
                opponent.Update(gameTime, !opponentInTheSameQuarter);
                player.Update(gameTime);

                ITownQuarterOwner owner = town.Quarters[0].Owner;

                if ((owner == player || owner == opponent) && town.Quarters.All(q => q.Owner == owner))
                {
                    System.Drawing.Bitmap townMap = town.CreateTownMapImage();
                    PrepareEnd();
                    bool playAgain;
                    using (GameOver gameOver = new GameOver(owner == player, townMap))
                    {
                        System.Windows.Forms.DialogResult result = gameOver.ShowDialog();
                        playAgain = result != System.Windows.Forms.DialogResult.Cancel;
                    }
                    townMap.Dispose();
                    if (playAgain)
                    {
                        ShowMainMenuDialog(false);
                        PrepareStart();
                    }
                    else
                    {
                        Exit();
                    }
                    return;
                }

                if (player.Health <= 0)
                {
                    TownQuarter newQuarter = FindAndRespawnQuartersFor(player, gameTime);
                    player.RespawnInto(newQuarter);
                    drawer.ShowMessage(gameTime, String.Format("You were killed! You are now in {0}.", newQuarter.Name));
                    town.CurrentQuarter = newQuarter;
                    drawer.ShowFullscreenEffect(gameTime, contentRepository.RespawnFullscreenEffect, Player.RespawnFullscreenEffectDuration);
                }
                if(opponent.Health <= 0)
                {
                    TownQuarter newQuarter = FindAndRespawnQuartersFor(opponent, gameTime);
                    opponent.RespawnInto(newQuarter);
                    opponent.ClearTasks();
                    drawer.ShowMessage(gameTime, String.Format("Congratulations, your opponent was killed. He respawned in {0}.", newQuarter.Name));
                }
            }
        }

        /// <summary>
        /// Prepares the game for its end.
        /// </summary>
        private void PrepareEnd()
        {
            running = false;
            backgroundSound.Stop();
            SuppressDraw();
            UnloadContent();
            if (settings.Fullscreen)
            {
                graphics.ToggleFullScreen();
            }
            this.ResetElapsedTime();
        }

        /// <summary>
        /// Restarts the whole game logic.
        /// </summary>
        private void Restart()
        {
            PrepareEnd();
            ShowMainMenuDialog(false);
            PrepareStart();
        }

        /// <summary>
        /// Prepares the game for start. It does pre-initializing and initializing.
        /// </summary>
        private void PrepareStart()
        {
            PreInitialize();
            Initialize();
        }

        /// <summary>
        /// Seareches for the available respawn quarter for the given playerDefaultGuns.
        /// </summary>
        /// <param name="quarterOwner">The given player that has to be respawned</param>
        /// <param name="gameTime">Game time</param>
        /// <returns>Quarter where the given player can be respawned</returns>
        TownQuarter FindAndRespawnQuartersFor(ITownQuarterOwner quarterOwner, GameTime gameTime)
        {
            TownQuarter newPosQuarter = null;
            IOrderedEnumerable<TownQuarter> randomizedQuarters = from q in town.Quarters where q.Owner == quarterOwner || q.Owner == EmptyTownQuarterOwner.Instance orderby random.Next() select q;
            foreach (TownQuarter quarter in town.Quarters)
            {
                if (quarter.Owner == quarterOwner)
                {
                    if (newPosQuarter == null)
                    {
                        newPosQuarter = quarter;
                    }
                    else
                    {
                        quarter.SetOwner(EmptyTownQuarterOwner.Instance, gameTime);
                    }
                }
            }
            if (newPosQuarter == null)
            {
                newPosQuarter = randomizedQuarters.First();
            }
            return newPosQuarter;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (running)
            {
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkOliveGreen, 1, 1);
                base.Draw(gameTime);
            }
        }
    }
}
