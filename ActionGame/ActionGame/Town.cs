using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame
{
    class Town : GameComponent, IDisposable
    {
        /// <summary>
        /// Maximum interface count per one quarter
        /// </summary>
        const int MaxQuarterDegree = 8;
        /// <summary>
        /// Minimal space what's needed per one interface for quarter. In meters.
        /// </summary>
        const float MinSideLengthPerInterface = 70;
        const int MapImageWidth = 800;
        const int MapImageHeight = 600;
        ///TODO: This constant can be calculated!
        const float MinQuarterSideLength = 100;

        public Texture2D Map;
        TownQuarter[] quarters;
        int currentQuarterIndex;
        int lastNearestInterfaceIndex = -1;
        
        public Town(ActionGame game, int quarterCount, ContentManager content, Matrix worldTransform, GraphicsDevice graphicsDevice)
            : base(game)
        {
            quarters = new TownQuarter[quarterCount];

            //Town graph creation
            int[] degrees = new int[quarterCount];
            bool[,] edges = new bool[quarterCount,quarterCount]; // Graph is unoriented (symetric). edges[i, j] can be true only if i<j!
            for (int i = 0; i < quarterCount-1; i++) // First is made path through all. Graph has to have only one component.
            { 
                int j = i+1;
                degrees[i]++;
                degrees[j]++;
                edges[i, j] = true;
            }
            Random rand = new Random();
            for (int i = 0; i < quarterCount; i++)
            {
                for (int j = i+1; j < quarterCount; j++) //graph isn't oriented and reflexion is denied
                {
                    if (!edges[i, j] && degrees[i] < MaxQuarterDegree && degrees[j] < MaxQuarterDegree)
                    {
                        if (rand.Next(0, 4) == 0)
                        {
                            degrees[i]++;
                            degrees[j]++;
                            edges[i, j] = true;
                        } 
                    }
                }
            }

            //Quarter creating by degrees
            for (int i = 0; i < quarterCount; i++)
            {
                float perimeterLength = MinSideLengthPerInterface * Math.Max(degrees[i], 4); // Even interface isn't needed the side must be there
                perimeterLength *= (float)rand.NextDouble() + 1f; //Minimal length can be doubled
                float width = (perimeterLength / 2f) * (float)(rand.NextDouble() * 0.3 + 0.35); //aspect ratio
                float height = (perimeterLength / 2f) - width;
                if (width < MinQuarterSideLength)
                    width = MinQuarterSideLength;
                if (height < MinQuarterSideLength)
                    height = MinQuarterSideLength;

                do
                {
                    try
                    {
                        TownQuarter quarter = new TownQuarter(new Vector2(width, height), degrees[i], content, worldTransform, graphicsDevice);
                        quarters[i] = quarter;
                    }
                    catch (NoSpaceForInterfaceException ex)
                    {
                        width += MinSideLengthPerInterface / 2;
                        height += MinSideLengthPerInterface / 2;
                    }
                }
                while (quarters[i] == null);
            }


            //Joining interfaces
            for (int i = 0; i < quarterCount; i++)
            {
                for (int j = i + 1; j < quarterCount; j++)
                {
                    if (edges[i, j])
                    {
                        TownQuarterInterface ifaceI = (from iface in quarters[i].Interfaces where iface.OppositeInterface == null orderby rand.Next() select iface).First();
                        TownQuarterInterface ifaceJ = (from iface in quarters[j].Interfaces where iface.OppositeInterface == null orderby rand.Next() select iface).First();
                        ifaceI.OppositeInterface = ifaceJ;
                        ifaceJ.OppositeInterface = ifaceI;
                    }
                }
            }

            //Town graph raster map creation
            Bitmap mapRaster = new Bitmap(MapImageWidth, MapImageHeight);
            using (Graphics graphics = Graphics.FromImage(mapRaster))
            {
                graphics.FillRectangle(Brushes.White, 0, 0, mapRaster.Width, mapRaster.Height);
                float angleJump = MathHelper.TwoPi / quarterCount;
                float radius = Math.Min(MapImageWidth, MapImageHeight)/2f - 20f;
                PointF center = new PointF(MapImageWidth/2f, MapImageHeight/2f);

                for (int i = 0; i < quarterCount; i++)
                {
                    for (int j = i + 1; j < quarterCount; j++)
                    {
                        if (edges[i, j])
                        {
                            graphics.DrawLine(Pens.Green,
                                center.X + (float)Math.Cos(i * angleJump) * radius,
                                center.Y + (float)Math.Sin(i * angleJump) * radius,
                                center.X + (float)Math.Cos(j * angleJump) * radius,
                                center.Y + (float)Math.Sin(j * angleJump) * radius
                                );
                        }
                    }
                }

                for (int i = 0; i < quarterCount; i++)
                {
                    graphics.FillEllipse(Brushes.Blue,
                        center.X + (float)Math.Cos(i * angleJump) * radius - 3.5f,
                        center.Y + (float)Math.Sin(i * angleJump) * radius - 3.5f,
                        7, 7);
                    graphics.DrawString(quarters[i].Name, new Font("Verdana", 12), Brushes.Black, center.X + (float)Math.Cos(i * angleJump) * radius, center.Y + (float)Math.Sin(i * angleJump) * radius - 16);
                }
            }
            using (MemoryStream ms = new MemoryStream())
            {
                mapRaster.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                Map = Texture2D.FromStream(graphicsDevice, ms);
            }

            //Selecting starting quarter
            currentQuarterIndex = 0;
        }

        protected new ActionGame Game
        {
            get
            {
                return (ActionGame)base.Game;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            TownQuarter currentQuarter = quarters[currentQuarterIndex];

            int nearestInterfaceIndex = -2;
            float length = float.MaxValue;
            for (int i = 0; i < currentQuarter.Interfaces.Count; i++)
            {
                Vector3 diff = Game.Camera.Position - currentQuarter.Interfaces[i].Position().ToVector3(0);
                if (diff.Length() < length)
                {
                    length = diff.Length();
                    nearestInterfaceIndex = i;
                }
            }

            if (nearestInterfaceIndex != lastNearestInterfaceIndex)
            {

                lastNearestInterfaceIndex = nearestInterfaceIndex;
                FillDrawer(nearestInterfaceIndex);
            }
        }

        void FillDrawer(int nearestInterfaceIndex)
        {
            TownQuarter currentQuarter = quarters[currentQuarterIndex];
            float squareWidth = currentQuarter.SquareWidth;
            TownQuarterInterface iface = currentQuarter.Interfaces[nearestInterfaceIndex];
            Vector2 delta = ResolveQuarterPositionDelta(squareWidth, iface);
            float angle = ResolveQuarterAzimuthDelta(iface.SidePosition, iface.OppositeInterface.SidePosition);
            iface.OppositeInterface.Quarter.FillDrawer(Game.Drawer, angle, delta);
            
            //currentQuarter.FillDrawer(Game.Drawer, MathHelper.Pi, new Vector2(50, 50));
            currentQuarter.FillDrawer(Game.Drawer);
        }

        private static Vector2 ResolveQuarterPositionDelta(float squareWidth, TownQuarterInterface iface)
        {
            Vector2 delta = Vector2.Zero;
            switch (iface.SidePosition)
            {
                case TownQuarterInterfacePosition.Top:
                    switch (iface.OppositeInterface.SidePosition)
                    {
                        case TownQuarterInterfacePosition.Top:
                            delta.X = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * squareWidth;
                            delta.Y = 0;
                            break;
                        case TownQuarterInterfacePosition.Right:
                            delta.X = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * squareWidth;
                            delta.Y = -(iface.OppositeInterface.Quarter.BitmapSize.Width * squareWidth);
                            break;
                        case TownQuarterInterfacePosition.Bottom:
                            delta.X = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * squareWidth;
                            delta.Y = -(iface.OppositeInterface.Quarter.BitmapSize.Height * squareWidth);
                            break;
                        case TownQuarterInterfacePosition.Left:
                            delta.X = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * squareWidth;
                            delta.Y = 0;
                            break;
                    }
                    break;
                case TownQuarterInterfacePosition.Right:
                    switch (iface.OppositeInterface.SidePosition)
                    {
                        case TownQuarterInterfacePosition.Top:
                            delta.X = iface.Quarter.BitmapSize.Width * squareWidth;
                            delta.Y = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * squareWidth;
                            break;
                        case TownQuarterInterfacePosition.Right:
                            delta.X = (iface.Quarter.BitmapSize.Width + iface.OppositeInterface.Quarter.BitmapSize.Width) * squareWidth;
                            delta.Y = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * squareWidth;
                            break;
                        case TownQuarterInterfacePosition.Bottom:
                            delta.X = (iface.Quarter.BitmapSize.Width + iface.OppositeInterface.Quarter.BitmapSize.Height) * squareWidth;
                            delta.Y = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * squareWidth;
                            break;
                        case TownQuarterInterfacePosition.Left:
                            delta.X = iface.Quarter.BitmapSize.Width * squareWidth;
                            delta.Y = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * squareWidth;
                            break;
                    }
                    break;

                case TownQuarterInterfacePosition.Bottom:
                    switch (iface.OppositeInterface.SidePosition)
                    {
                        case TownQuarterInterfacePosition.Top:
                            delta.X = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * squareWidth;
                            delta.Y = (iface.Quarter.BitmapSize.Height * squareWidth);
                            break;
                        case TownQuarterInterfacePosition.Right:
                            delta.X = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * squareWidth;
                            delta.Y = ((iface.Quarter.BitmapSize.Height + iface.OppositeInterface.Quarter.BitmapSize.Width) * squareWidth);
                            break;
                        case TownQuarterInterfacePosition.Bottom:
                            delta.X = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * squareWidth;
                            delta.Y = ((iface.Quarter.BitmapSize.Height + iface.OppositeInterface.Quarter.BitmapSize.Height) * squareWidth);
                            break;
                        case TownQuarterInterfacePosition.Left:
                            delta.X = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * squareWidth;
                            delta.Y = (iface.Quarter.BitmapSize.Height * squareWidth);
                            break;
                    }
                    break;


                case TownQuarterInterfacePosition.Left:
                    switch (iface.OppositeInterface.SidePosition)
                    {
                        case TownQuarterInterfacePosition.Top:
                            delta.X = 0;
                            delta.Y = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * squareWidth;
                            break;
                        case TownQuarterInterfacePosition.Right:
                            delta.X = -(iface.OppositeInterface.Quarter.BitmapSize.Width) * squareWidth;
                            delta.Y = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * squareWidth;
                            break;
                        case TownQuarterInterfacePosition.Bottom:
                            delta.X = -(iface.OppositeInterface.Quarter.BitmapSize.Height) * squareWidth;
                            delta.Y = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * squareWidth;
                            break;
                        case TownQuarterInterfacePosition.Left:
                            delta.X = 0;
                            delta.Y = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * squareWidth;
                            break;
                    }
                    break;
            }
            return delta;
        }

        private static float ResolveQuarterAzimuthDelta(TownQuarterInterfacePosition mainQuarterPosition, TownQuarterInterfacePosition neighborQuarterPosition)
        {
            float angle = 0; //clockwise
            if (
                   (mainQuarterPosition == TownQuarterInterfacePosition.Top && neighborQuarterPosition == TownQuarterInterfacePosition.Bottom)
                || (mainQuarterPosition == TownQuarterInterfacePosition.Right && neighborQuarterPosition == TownQuarterInterfacePosition.Left)
                || (mainQuarterPosition == TownQuarterInterfacePosition.Bottom && neighborQuarterPosition == TownQuarterInterfacePosition.Top)
                || (mainQuarterPosition == TownQuarterInterfacePosition.Left && neighborQuarterPosition == TownQuarterInterfacePosition.Right)
                )
            {
                angle = 0;
            }
            else if (
                   (mainQuarterPosition == TownQuarterInterfacePosition.Top && neighborQuarterPosition == TownQuarterInterfacePosition.Right)
                || (mainQuarterPosition == TownQuarterInterfacePosition.Right && neighborQuarterPosition == TownQuarterInterfacePosition.Bottom)
                || (mainQuarterPosition == TownQuarterInterfacePosition.Bottom && neighborQuarterPosition == TownQuarterInterfacePosition.Left)
                || (mainQuarterPosition == TownQuarterInterfacePosition.Left && neighborQuarterPosition == TownQuarterInterfacePosition.Top)
                )
            {
                angle = MathHelper.PiOver2;
            }
            else if (
                   (mainQuarterPosition == TownQuarterInterfacePosition.Top && neighborQuarterPosition == TownQuarterInterfacePosition.Top)
                || (mainQuarterPosition == TownQuarterInterfacePosition.Right && neighborQuarterPosition == TownQuarterInterfacePosition.Right)
                || (mainQuarterPosition == TownQuarterInterfacePosition.Bottom && neighborQuarterPosition == TownQuarterInterfacePosition.Bottom)
                || (mainQuarterPosition == TownQuarterInterfacePosition.Left && neighborQuarterPosition == TownQuarterInterfacePosition.Left)
                )
            {
                angle = MathHelper.Pi;
            }
            else if (
                   (mainQuarterPosition == TownQuarterInterfacePosition.Top && neighborQuarterPosition == TownQuarterInterfacePosition.Left)
                || (mainQuarterPosition == TownQuarterInterfacePosition.Right && neighborQuarterPosition == TownQuarterInterfacePosition.Top)
                || (mainQuarterPosition == TownQuarterInterfacePosition.Bottom && neighborQuarterPosition == TownQuarterInterfacePosition.Right)
                || (mainQuarterPosition == TownQuarterInterfacePosition.Left && neighborQuarterPosition == TownQuarterInterfacePosition.Bottom)
                )
            {
                angle = (3f * MathHelper.PiOver2);
            }
            return angle;
        }

        public void Dispose()
        {
            foreach (TownQuarter quarter in quarters)
                quarter.Dispose();
        }
    }
}
