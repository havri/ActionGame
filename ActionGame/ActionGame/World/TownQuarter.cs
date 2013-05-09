using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ActionGame.Components;
using ActionGame.People;
using ActionGame.Space;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ActionGame.QSP;
using ActionGame.Objects;
using ActionGame.Tools;


namespace ActionGame.World
{
    public partial class TownQuarter : IDisposable
    {
        /// <summary>
        /// Block without road width. In road square count.
        /// </summary>
        const int BlockWidth = 5;
        /// <summary>
        /// Number of walkers in  quarter.
        /// </summary>
        const int WalkerCount = 15;
        /// <summary>
        /// Number of each walker waypoints.
        /// </summary>
        const int WalkerWayPointCount = 4;
        /// <summary>
        /// Width of road or sidewalk in pictured map. In pixels.
        /// </summary>
        const int PictureMapRoadWidth = 15;

        const float BetweenBuildingSpace = 4f;
        readonly static List<string> nameRepository = new List<string>(new string[] {
            "Downtown", "Czech Quarter", "New Prague", "White Hills", "New Land", "Little Side", "Little Troy", "Old York"
        });
        readonly static string emptyName = "Unnamed";
        public readonly static TimeSpan GuardAddTimeout = new TimeSpan(0, 0, 0, 30);
        public const int MaxGuardCount = 20;

        TimeSpan lastTimeGuardAdded = TimeSpan.Zero;
        public ITownQuarterOwner Owner { get { return owner; } }
        ITownQuarterOwner owner;
        TimeSpan ownershipBeginTime = TimeSpan.Zero;
        public TimeSpan OwnershipBeginTime
        {
            get
            {
                return ownershipBeginTime;
            }
        }
        /// <summary>
        /// Object what makes ground textures.
        /// </summary>
        readonly LinkedList<FlatObject> groundObjects = new LinkedList<FlatObject>();
        readonly LinkedList<Plate> magicPlates = new LinkedList<Plate>();
        readonly HashSet<BulletVisualisation> magicBullets = new HashSet<BulletVisualisation>();
        readonly List<KeyValuePair<BulletVisualisation, TimeSpan>> bulletAddedTimes = new List<KeyValuePair<BulletVisualisation,TimeSpan>>();
        readonly List<Human> walkers = new List<Human>(WalkerCount);
        readonly List<Human> guards = new List<Human>();
        readonly List<Plate> roadSignsPointingToMe = new List<Plate>();
        /// <summary>
        /// Really spatial objects - buildings, etc.
        /// </summary>
        readonly LinkedList<SpatialObject> solidObjects = new LinkedList<SpatialObject>();
        readonly LinkedList<Plate> solidPlates = new LinkedList<Plate>();
        readonly List<Box> boxes = new List<Box>();

        readonly List<TownQuarterInterface> interfaces;
        public List<TownQuarterInterface> Interfaces { get { return interfaces; } }
        IList<PathGraphVertex> pathGraph;
        readonly Grid spaceGrid;
        public Grid SpaceGrid { get { return spaceGrid; } }
        /// <summary>
        /// Picture map of this quarter.
        /// </summary>
        Texture2D map;
        Size bitmapSize;
        public Size BitmapSize
        {
            get
            {
                return bitmapSize;
            }
        }
        MapFillType[] mapBitmap;
        /// <summary>
        /// Width of road and sidewalk. In meters.
        /// </summary>
        public const float SquareWidth = 4.5f;


        /// <summary>
        /// Quarter name
        /// </summary>
        public string Name;
        Texture2D roadSignTexture;

        float currentDrawingAzimuthDelta;
        Vector2 currentDrawingPositionDelta;
        bool updateProcessing = false;
        readonly List<SpatialObject> awaitingDestroy = new List<SpatialObject>();
        readonly List<Human> awaitingLeave = new List<Human>();
        readonly List<Human> awaitingEnter = new List<Human>();
        readonly ActionGame game;
        Flag flag;
        public Flag Flag
        {
            get
            {
                return flag;
            }
        }
        public float CurrentDrawingAzimuthDelta
        {
            get
            {
                return currentDrawingAzimuthDelta;
            }
        }
        public Vector2 CurrentDrawingPositionDelta
        {
            get
            {
                return currentDrawingPositionDelta;
            }
        }
        bool currentlyDrawed = false;
        public bool CurrentlyDrawed
        {
            get
            {
                return currentlyDrawed;
            }
        }
        /// <summary>
        /// Creates new town quarter as map fragment. Generates roads, buildings, etc.
        /// </summary>
        /// <param name="size">Size of quarter without joining interface</param>
        /// <param name="degree">Number of quarter's interfaces (joining streets)</param>
        /// <param name="content">ContentManager for loading objects</param>
        /// <param name="worldTransform">World transform matrix</param>
        /// <param name="graphicsDevice">Graphics device for creating textures</param>
        public TownQuarter(ActionGame game, Vector2 size, int degree)
        {
            owner = EmptyTownQuarterOwner.Instance;

            this.game = game;
            interfaces = new List<TownQuarterInterface>(degree);
            if (nameRepository.Count > 0)
            {
                Random rand = new Random();
                int nameIndex = rand.Next(0, nameRepository.Count - 1);
                Name = nameRepository[nameIndex];
                nameRepository.RemoveAt(nameIndex);
            }
            else
            {
                Name = emptyName;
            }

            int xSize = (int)Math.Floor(size.X / SquareWidth);
            int ySize = (int)Math.Floor(size.Y / SquareWidth);
            bitmapSize = new System.Drawing.Size(xSize, ySize);
            mapBitmap = new MapFillType[bitmapSize.Width * bitmapSize.Height];
            for (int i = 0; i < mapBitmap.Length; i++)
                mapBitmap[i] = MapFillType.Empty;
            spaceGrid = new Grid(bitmapSize.Width, bitmapSize.Height, SquareWidth, SquareWidth);

            try
            {
                Generate(degree);
            }
            catch (Exception ex)
            {
                if (Name != emptyName)
                {
                    nameRepository.Add(Name);
                }
                throw ex;
            }
            spaceGrid.Fill(GetAllSolidObjects());
        }

        private void BuildFlag()
        {
            Microsoft.Xna.Framework.Point square = GetRandomSquare(m => m == MapFillType.Sidewalk);
            Model flagModel = owner.Content.FlagModel;
            Vector3 flagSize = flagModel.GetSize(game.Drawer.WorldTransformMatrix);
            PositionInTown pos = new PositionInTown(this, ((square.ToVector2() + new Vector2(0.5f, 0.5f)) * SquareWidth) - (flagSize.XZToVector2() * 0.5f));
            flag = new Flag(game, flagModel, pos, 0, game.Drawer.WorldTransformMatrix);
            solidObjects.AddLast(flag);
        }

        public void SetOwner(ITownQuarterOwner newOwner, GameTime gameTime)
        {
            owner = newOwner;
            roadSignTexture.Dispose();
            GenerateRoadSignPicture();
            foreach (Plate roadSign in roadSignsPointingToMe)
            { 
                roadSign.SetFront(roadSignTexture, true);
            }
            flag.SetModel(owner.Content.FlagModel, game.Drawer.WorldTransformMatrix);
            ownershipBeginTime = gameTime.TotalGameTime;
        }

        public void RegisterNewRoadSign(Plate roadSign)
        {
            roadSignsPointingToMe.Add(roadSign);
        }
        /// <summary>
        /// Configures drawer for drawing this quarter as main.
        /// </summary>
        /// <param name="drawer">Display drawer</param>
        public void FillDrawer()
        {
            FillDrawer(0, Vector2.Zero);
        }

        /// <summary>
        /// Configures drawer for drawing this quarter joined to others.
        /// </summary>
        /// <param name="drawer">Display drawer</param>
        /// <param name="delta">Defines difference of quarter placement from drawing center</param>
        /// <param name="position">Defines position of joining interface - determines whole quarter azimuth</param>
        public void FillDrawer(float angle, Vector2 delta)
        {
            currentDrawingAzimuthDelta = angle;
            currentDrawingPositionDelta = delta;
            foreach (IDrawableObject o in GetAllDrawalbleObjects())
            {
                game.Drawer.StartDrawingObject(o, angle, delta);
            }
            currentlyDrawed = true;
        }

        public void RemoveFromDrawer()
        {
            foreach (IDrawableObject o in GetAllDrawalbleObjects())
            {
                game.Drawer.StopDrawingObject(o);
            }
            currentlyDrawed = false;
        }

        public Texture2D Map
        {
            get { return map; }
        }

        public Vector2 QuarterSize
        {
            get 
            {
                return new Vector2(BitmapSize.Width * SquareWidth, BitmapSize.Height * SquareWidth);
            }
        }

        public void Dispose()
        {
            foreach (var obj in groundObjects)
                obj.Dispose();
            map.Dispose();
            roadSignTexture.Dispose();
        }

        public void Update(GameTime gameTime, bool gameLogicOnly)
        {
            updateProcessing = true;
            //Add guard if it's time
            if (owner != EmptyTownQuarterOwner.Instance && gameTime.TotalGameTime - lastTimeGuardAdded > GuardAddTimeout && guards.Count < MaxGuardCount)
            {
                int count = (int)((gameTime.TotalGameTime - lastTimeGuardAdded).TotalSeconds / GuardAddTimeout.TotalSeconds);
                for (int i = 0; i < count; i++)
                {
                    Human newGuard = owner.CreateAllyGuard(this);
                    guards.Add(newGuard);
                    spaceGrid.AddObject(newGuard);
                    if (currentlyDrawed)
                    {
                        game.Drawer.StartDrawingObject(newGuard, currentDrawingAzimuthDelta, currentDrawingPositionDelta);
                    }
                }
                lastTimeGuardAdded = gameTime.TotalGameTime;
            }

            //Update humans
            foreach (Human guard in guards)
            {
                guard.Update(gameTime, gameLogicOnly);
            }

            if (!gameLogicOnly)
            {
                foreach (Human walker in walkers)
                {
                    walker.Update(gameTime, gameLogicOnly);
                }
            }

            spaceGrid.Update();

            flag.Update(gameTime);

            if (bulletAddedTimes.Count != 0)
            {
                foreach (KeyValuePair<BulletVisualisation, TimeSpan> bulletAddedTime in bulletAddedTimes)
                {
                    if (bulletAddedTime.Value + BulletVisualisation.ShowTimeSpan < gameTime.TotalGameTime)
                    {
                        magicBullets.Remove(bulletAddedTime.Key);
                        game.Drawer.StopDrawingObject(bulletAddedTime.Key);
                        bulletAddedTime.Key.Dispose();
                    }
                }
                bulletAddedTimes.RemoveAll(x => x.Value + BulletVisualisation.ShowTimeSpan < gameTime.TotalGameTime);
            }
            updateProcessing = false;


            SolveAwaings();
        }

        private void SolveAwaings()
        {
            if (awaitingDestroy.Count + awaitingEnter.Count + awaitingLeave.Count != 0)
            {
                foreach (SpatialObject obj in awaitingDestroy)
                {
                    DestroyObject(obj);
                }
                awaitingDestroy.Clear();
                foreach (Human human in awaitingLeave)
                {
                    BeLeftBy(human);
                }
                awaitingLeave.Clear();
                foreach (Human human in awaitingEnter)
                {
                    BeEnteredBy(human);
                }
                awaitingEnter.Clear();
            }
        }

        public Texture2D RoadSignTexture
        {
            get { return roadSignTexture; }
        }

        private List<IDrawableObject> GetAllDrawalbleObjects()
        {
            List<IDrawableObject> result = new List<IDrawableObject>();
            result.AddRange(groundObjects);
            result.AddRange(magicPlates);
            result.AddRange(solidPlates);
            result.AddRange(solidObjects);
            result.AddRange(boxes);
            result.AddRange(walkers);
            result.AddRange(guards);
            result.AddRange(magicBullets);
            if (game.Opponent.Position.Quarter == this)
            {
                result.Add(game.Opponent);
            }
            return result;
        }

        List<Quadrangle> GetAllSolidObjects()
        {
            List<Quadrangle> result = new List<Quadrangle>(solidPlates);
            result.AddRange(solidObjects);
            result.AddRange(guards);
            result.AddRange(walkers);
            result.AddRange(boxes);
            if (game.Opponent.Position.Quarter == this)
            {
                result.Add(game.Opponent);
            }
            return result;
        }

        public PathGraphVertex FindNearestPathGraphVertex(Vector2 from)
        {
            return spaceGrid.FindNearestPathGraphVertex(from);
        }

        public void DestroyObject(SpatialObject obj)
        {
            if (!updateProcessing)
            {
                if (obj is Human)
                {        
                    guards.Remove(obj as Human);
                    walkers.Remove(obj as Human);
                }
                else if(obj is Box)
                {
                    //solidObjects.Remove(obj);
                    boxes.Remove(obj as Box);
                }
                game.Drawer.StopDrawingObject(obj);
                spaceGrid.RemoveObject(obj);
            }
            else
            {
                awaitingDestroy.Add(obj);
            }
        }

        public void AddBullet(GameTime gameTime, BulletVisualisation bullet)
        {
            magicBullets.Add(bullet);
            bulletAddedTimes.Add(new KeyValuePair<BulletVisualisation, TimeSpan>(bullet, gameTime.TotalGameTime));
            game.Drawer.StartDrawingObject(bullet, currentDrawingAzimuthDelta, currentDrawingPositionDelta);
        }

        public void BeLeftBy(Human human)
        {
            if (!updateProcessing)
            {
                spaceGrid.RemoveObject(human);
            }
            else
            {
                awaitingLeave.Add(human);
            }
        }

        public void BeEnteredBy(Human human)
        {
            if (!updateProcessing)
            {
                spaceGrid.AddObject(human);
            }
            else
            {
                awaitingEnter.Add(human);
            }
        }

        /// <summary>
        /// Finds nearest box in this quarter. If the position parameter is situated in another quarter, the returned box will not be actually the nearest.
        /// </summary>
        /// <param name="position">You're position</param>
        /// <param name="toolBox">What are you seeking for: True for tool box; False for heal box</param>
        /// <returns>Box or null if there are no boxes in the quarter at all</returns>
        public Box GetNearestBox(PositionInTown position, bool toolBox)
        {
            Box nearest = null;
            float distance = float.MaxValue;
            foreach (Box box in boxes)
            {
                float d = box.Position.MinimalDistanceTo(position);
                if (d < distance)
                {
                    if ((toolBox && box is ToolBox) || (!toolBox && box is HealBox))
                    {
                        distance = d;
                        nearest = box;
                    }
                }
            }
            return nearest;
        }
    }
}
