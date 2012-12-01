using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ActionGame.Components;
using ActionGame.Space;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace ActionGame.World
{
    public partial class TownQuarter : IDisposable
    {
        /// <summary>
        /// Block without road width. In road square count.
        /// </summary>
        const int BlockWidth = 5;

        /// <summary>
        /// Width of road or sidewalk in pictured map. In pixels.
        /// </summary>
        const int PictureMapRoadWidth = 15;

        const float BetweenBuildingSpace = 4f;

        /// <summary>
        /// Object what makes ground textures.
        /// </summary>
        LinkedList<SpatialObject> groundObjects = new LinkedList<SpatialObject>();

        /// <summary>
        /// Really spatial objects - buildings, etc.
        /// </summary>
        LinkedList<SpatialObject> solidObjects = new LinkedList<SpatialObject>();

        List<TownQuarterInterface> interfaces;
        public List<TownQuarterInterface> Interfaces { get { return interfaces; } }
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
        float squareWidth;
        public float SquareWidth
        {
            get
            {
                return squareWidth;
            }
        }


        /// <summary>
        /// Quarter name
        /// </summary>
        public string Name;

        readonly static List<string> nameRepository = new List<string>(new string[] {
            "Downtown", "Czech Quarter", "New Prague", "White Hills", "New Land", "Little Side"
        });
        readonly static string emptyName = "Unnamed";

        /// <summary>
        /// Creates new town quarter as map fragment. Generates roads, buildings, etc.
        /// </summary>
        /// <param name="size">Size of quarter without joining interface</param>
        /// <param name="degree">Number of quarter's interfaces (joining streets)</param>
        /// <param name="content">ContentManager for loading objects</param>
        /// <param name="worldTransform">World transform matrix</param>
        /// <param name="graphicsDevice">Graphics device for creating textures</param>
        public TownQuarter(Vector2 size, int degree, ContentManager content, Matrix worldTransform, GraphicsDevice graphicsDevice)
        {
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

            //Generate(ref size, degree, content, ref worldTransform, graphicsDevice);

            
            try
            {
                Generate(ref size, degree, content, ref worldTransform, graphicsDevice);
            }
            catch (Exception ex)
            {
                if (Name != emptyName)
                {
                    nameRepository.Add(Name);
                }
                throw ex;
            }
        }

        /// <summary>
        /// Configures drawer for drawing this quarter as main.
        /// </summary>
        /// <param name="drawer">Display drawer</param>
        public void FillDrawer(Drawer drawer)
        {
            FillDrawer(drawer, 0, Vector2.Zero);
        }

        /// <summary>
        /// Configures drawer for drawing this quarter joined to others.
        /// </summary>
        /// <param name="drawer">Display drawer</param>
        /// <param name="delta">Defines difference of quarter placement from drawing center</param>
        /// <param name="position">Defines position of joining interface - determines whole quarter azimuth</param>
        public void FillDrawer(Drawer drawer, float angle, Vector2 delta)
        {
            foreach (SpatialObject o in groundObjects)
            {
                drawer.StartDrawingSpatialObject(o, angle, delta, true);
            }
            foreach (SpatialObject o in solidObjects)
            {
                drawer.StartDrawingSpatialObject(o, angle, delta, false);
            }
        }

        public void RemoveFromDrawer(Drawer drawer)
        {
            foreach (SpatialObject o in groundObjects)
            {
                drawer.StopDrawingSpatialObject(o, true);
            }
            foreach (SpatialObject o in solidObjects)
            {
                drawer.StopDrawingSpatialObject(o, false);
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
        }
    }
}
