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
        const int WalkerCount = 10;
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

        ITownQuarterOwner owner;
        /// <summary>
        /// Object what makes ground textures.
        /// </summary>
        readonly LinkedList<FlatObject> groundObjects = new LinkedList<FlatObject>();
        readonly LinkedList<Plate> magicPlates = new LinkedList<Plate>();
        readonly HashSet<BulletVisualisation> magicBullets = new HashSet<BulletVisualisation>();
        readonly List<KeyValuePair<BulletVisualisation, TimeSpan>> bulletAddedTimes = new List<KeyValuePair<BulletVisualisation,TimeSpan>>();
        readonly LinkedList<Human> walkers = new LinkedList<Human>();
        readonly List<Plate> roadSignsPointingToMe = new List<Plate>();
        /// <summary>
        /// Really spatial objects - buildings, etc.
        /// </summary>
        readonly LinkedList<SpatialObject> solidObjects = new LinkedList<SpatialObject>();
        readonly LinkedList<Plate> solidPlates = new LinkedList<Plate>();

        readonly List<TownQuarterInterface> interfaces;
        public List<TownQuarterInterface> Interfaces { get { return interfaces; } }

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
        readonly ActionGame game;

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

        public void SetOwner(ITownQuarterOwner newOwner)
        {
            owner = newOwner;
            roadSignTexture.Dispose();
            GenerateRoadSignPicture();
            foreach (Plate roadSign in roadSignsPointingToMe)
            { 
                roadSign.SetFront(roadSignTexture);
            }
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
        }

        public void RemoveFromDrawer()
        {
            foreach (IDrawableObject o in GetAllDrawalbleObjects())
            {
                game.Drawer.StopDrawingObject(o);
            }        
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
            foreach (var obj in solidObjects)
                obj.Dispose();
            map.Dispose();
            foreach (var walker in walkers)
                walker.Dispose();
            roadSignTexture.Dispose();
        }

        public void Update(GameTime gameTime)
        {
            updateProcessing = true;
            foreach (var walker in walkers)
                walker.Update(gameTime);
            spaceGrid.Update();

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
            updateProcessing = false;
            foreach (SpatialObject obj in awaitingDestroy)
            {
                DestroyObject(obj);
            }
            awaitingDestroy.Clear();
        }

        public Texture2D RoadSignTexture
        {
            get { return roadSignTexture; }
        }

        private LinkedList<IDrawableObject> GetAllDrawalbleObjects()
        {
            LinkedList<IDrawableObject> result = new LinkedList<IDrawableObject>(groundObjects);
            result.AddRange(magicPlates);
            result.AddRange(solidPlates);
            result.AddRange(solidObjects);
            result.AddRange(walkers);
            result.AddRange(magicBullets);
            return result;
        }

        LinkedList<Quadrangle> GetAllSolidObjects()
        {
            LinkedList<Quadrangle> result = new LinkedList<Quadrangle>(solidPlates);
            result.AddRange(solidObjects);
            result.AddRange(walkers);
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
                    walkers.Remove(obj as Human);
                }
                else
                {
                    solidObjects.Remove(obj);
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
    }
}
