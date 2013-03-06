using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using ActionGame.Extensions;
using ActionGame.MenuForms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.World
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
        readonly TownQuarter[] quarters;
        TownQuarter currentQuarter;
        public TownQuarter CurrentQuarter
        {
            get
            {
        	    return currentQuarter;
            }
        }
        bool currentQuarterDrawed = false;
        int lastNearestInterfaceIndex = -1;
        public ActionGame Game
        { 
            get {return (ActionGame)base.Game;}
        }
        
        public Town(ActionGame game, Loading loadingFrom)
            : base(game)
        {
            quarters = new TownQuarter[game.Settings.TownQuarterCount];

            //Town graph creation
            loadingFrom.SetLabel("Generating town graph...");
            loadingFrom.SetValue(0);
            int[] degrees = new int[game.Settings.TownQuarterCount];
            bool[,] edges = new bool[game.Settings.TownQuarterCount, game.Settings.TownQuarterCount]; // Graph is unoriented (symetric). edges[i, j] can be true only if i<j!
            for (int i = 0; i < game.Settings.TownQuarterCount - 1; i++) // First is made path through all. Graph has to have only one component.
            { 
                int j = i+1;
                degrees[i]++;
                degrees[j]++;
                edges[i, j] = true;
            }
            Random rand = new Random();
            for (int i = 0; i < game.Settings.TownQuarterCount; i++)
            {
                loadingFrom.SetValue(100 * i / game.Settings.TownQuarterCount);
                for (int j = i + 1; j < game.Settings.TownQuarterCount; j++) //graph isn't oriented and reflexion is denied
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
            loadingFrom.SetLabel("Generating quarters and streets...");
            loadingFrom.SetValue(0);
            for (int i = 0; i < game.Settings.TownQuarterCount; i++)
            {
                loadingFrom.SetValue(100 * i / game.Settings.TownQuarterCount);
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
                        TownQuarter quarter = new TownQuarter(game, new Vector2(width, height), degrees[i]);
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
            loadingFrom.SetLabel("Building town...");
            loadingFrom.SetValue(0);
            for (int i = 0; i < game.Settings.TownQuarterCount; i++)
            {
                loadingFrom.SetValue(100 * i / game.Settings.TownQuarterCount);
                for (int j = i + 1; j < game.Settings.TownQuarterCount; j++)
                {
                    if (edges[i, j])
                    {
                        TownQuarterInterface ifaceI = (from iface in quarters[i].Interfaces where iface.OppositeInterface == null orderby rand.Next() select iface).First();
                        TownQuarterInterface ifaceJ = (from iface in quarters[j].Interfaces where iface.OppositeInterface == null orderby rand.Next() select iface).First();
                        ifaceI.OppositeInterface = ifaceJ;
                        ifaceJ.OppositeInterface = ifaceI;
                        ifaceI.LeftPathGraphVertex.AddNeighborBothDirection(ifaceJ.RightPathGraphVertex, TownQuarter.SquareWidth);
                        ifaceI.RightPathGraphVertex.AddNeighborBothDirection(ifaceJ.LeftPathGraphVertex, TownQuarter.SquareWidth);
                    }
                }
            }
            foreach (var quarter in quarters)
            {
                quarter.BuildInterfaceRoadSigns();
            }

            //Town graph raster map creation
            loadingFrom.SetLabel("Creating maps for player...");
            loadingFrom.SetValue(0);
            Bitmap mapRaster = new Bitmap(MapImageWidth, MapImageHeight);
            using (Graphics graphics = Graphics.FromImage(mapRaster))
            {
                graphics.FillRectangle(Brushes.White, 0, 0, mapRaster.Width, mapRaster.Height);
                float angleJump = MathHelper.TwoPi / game.Settings.TownQuarterCount;
                float radius = Math.Min(MapImageWidth, MapImageHeight)/2f - 20f;
                PointF center = new PointF(MapImageWidth/2f, MapImageHeight/2f);

                for (int i = 0; i < game.Settings.TownQuarterCount; i++)
                {
                    loadingFrom.SetValue(100 * i / game.Settings.TownQuarterCount);
                    for (int j = i + 1; j < game.Settings.TownQuarterCount; j++)
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

                for (int i = 0; i < game.Settings.TownQuarterCount; i++)
                {
                    loadingFrom.SetValue(100 * i / game.Settings.TownQuarterCount);
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
                Map = Texture2D.FromStream(game.GraphicsDevice, ms);
            }

            //Selecting starting quarter
            currentQuarter = quarters[0];
        }

        static readonly TimeSpan quarterChangeTimeOut = new TimeSpan(0, 0, 1);
        TimeSpan lastQuarterChange;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Debug.Write("Current quarter", currentQuarter.Name);

            Vector2 playerPosition = Game.Player.PositionInQuarter.XZToVector2();
            Vector2 quarterSize = new Vector2(currentQuarter.BitmapSize.Width * TownQuarter.SquareWidth, currentQuarter.BitmapSize.Height * TownQuarter.SquareWidth);
            if (gameTime.TotalGameTime - lastQuarterChange > quarterChangeTimeOut
                && ((playerPosition.X > quarterSize.X || playerPosition.Y > quarterSize.Y) || (playerPosition.X < 0 || playerPosition.Y < 0))
                && lastNearestInterfaceIndex >= 0)
            {
                TownQuarterInterface usedInterface = currentQuarter.Interfaces[lastNearestInterfaceIndex];

                //Remove drawed quaeters from drawer
                usedInterface.OppositeInterface.Quarter.RemoveFromDrawer();
                currentQuarter.RemoveFromDrawer();

                //Remove player from old quarter space grid
                currentQuarter.SpaceGrid.RemoveObject(Game.Player);

                //Changes current quarter
                currentQuarter = usedInterface.OppositeInterface.Quarter;

                //Moves player into new current quarter
                float angle = ResolveQuarterAzimuthDelta(usedInterface.SidePosition, usedInterface.OppositeInterface.SidePosition);
                Vector2 delta = ResolveQuarterPositionDelta(TownQuarter.SquareWidth, usedInterface);
                Game.Player.MoveTo(
                    new PositionInTown(currentQuarter,
                        Vector3.Transform(Game.Player.PositionInQuarter, Matrix.CreateTranslation(-delta.ToVector3(0)) * Matrix.CreateRotationY(angle)).XZToVector2() // reverse transform of nearest quarter
                        ),
                    Game.Player.Azimuth - angle
                    );

                //Assings player to new space grid
                currentQuarter.SpaceGrid.AddObject(Game.Player);

                //Restart for drawing
                lastNearestInterfaceIndex = -1;
                currentQuarterDrawed = false;

                lastQuarterChange = gameTime.TotalGameTime;
            }
            
            FillDrawer();

            currentQuarter.Update(gameTime);
        }

        void FillDrawer()
        {
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
                if (lastNearestInterfaceIndex >= 0)
                {
                    currentQuarter.Interfaces[lastNearestInterfaceIndex].OppositeInterface.Quarter.RemoveFromDrawer();
                }
                lastNearestInterfaceIndex = nearestInterfaceIndex;

                float squareWidth = TownQuarter.SquareWidth;
                TownQuarterInterface iface = currentQuarter.Interfaces[nearestInterfaceIndex];
                Vector2 delta = ResolveQuarterPositionDelta(squareWidth, iface);
                float angle = ResolveQuarterAzimuthDelta(iface.SidePosition, iface.OppositeInterface.SidePosition);
                iface.OppositeInterface.Quarter.FillDrawer(angle, delta);

                if (!currentQuarterDrawed)
                {
                    Game.Drawer.CurrentQuarter = currentQuarter;
                    currentQuarter.FillDrawer();
                    currentQuarterDrawed = true;
                }
            }
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
            Map.Dispose();
            foreach (TownQuarter quarter in quarters)
                quarter.Dispose();
        }
    }
}
