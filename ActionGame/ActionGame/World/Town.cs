using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using ActionGame.Extensions;
using ActionGame.MenuForms;
using ActionGame.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.World
{
    public class Town : GameComponent, IDisposable
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


        public TownQuarter[] Quarters { get { return quarters; } }
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
        public new ActionGame Game
        { 
            get {return (ActionGame)base.Game;}
        }

        readonly Image mapImage;

        public Town(ActionGame game, Loading loadingFrom)
            : base(game)
        {
            EmptyTownQuarterOwner.LoadContent(game.Content);

            quarters = new TownQuarter[game.Settings.TownQuarterCount];

            //Town graph creation
            loadingFrom.SetLabel("Generating town graph...");
            loadingFrom.SetValue(0);
            int[] degrees = new int[game.Settings.TownQuarterCount];
            bool[,] edges = new bool[game.Settings.TownQuarterCount, game.Settings.TownQuarterCount]; // Graph is unoriented (symetric). edges[i, j] can be true only if i<j!
            for (int i = 0; i < game.Settings.TownQuarterCount - 1; i++) // First is made path through all. Graph has to have only one component.
            {
                int j = i + 1;
                degrees[i]++;
                degrees[j]++;
                edges[i, j] = true;
            }
            for (int i = 0; i < game.Settings.TownQuarterCount; i++)
            {
                loadingFrom.SetValue(100 * i / game.Settings.TownQuarterCount);
                for (int j = i + 1; j < game.Settings.TownQuarterCount; j++) //graph isn't oriented and reflexion is denied
                {
                    if (!edges[i, j] && degrees[i] < MaxQuarterDegree && degrees[j] < MaxQuarterDegree)
                    {
                        if (game.Random.Next(0, 4) == 0)
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
                perimeterLength *= (float)game.Random.NextDouble() + 1f; //Minimal length can be doubled
                float width = (perimeterLength / 2f) * (float)(game.Random.NextDouble() * 0.3 + 0.35); //aspect ratio
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
                    catch (NoSpaceForInterfaceException)
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
                        TownQuarterInterface ifaceI = (from iface in quarters[i].Interfaces where iface.OppositeInterface == null orderby game.Random.Next() select iface).First();
                        TownQuarterInterface ifaceJ = (from iface in quarters[j].Interfaces where iface.OppositeInterface == null orderby game.Random.Next() select iface).First();
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

            //Town map base creating
            {
                Bitmap mapRaster = new Bitmap(MapImageWidth, MapImageHeight);
                using (Graphics graphics = Graphics.FromImage(mapRaster))
                {
                    graphics.FillRectangle(Brushes.White, 0, 0, mapRaster.Width, mapRaster.Height);
                    float angleJump = MathHelper.TwoPi / Game.Settings.TownQuarterCount;
                    float radius = Math.Min(MapImageWidth, MapImageHeight) / 2f - 20f;
                    PointF center = new PointF(MapImageWidth / 2f, MapImageHeight / 2f);

                    for (int i = 0; i < Game.Settings.TownQuarterCount; i++)
                    {
                        for (int j = i + 1; j < Game.Settings.TownQuarterCount; j++)
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

                    for (int i = 0; i < Game.Settings.TownQuarterCount; i++)
                    {
                        graphics.FillEllipse(Brushes.Black,
                            center.X + (float)Math.Cos(i * angleJump) * radius - 3.5f,
                            center.Y + (float)Math.Sin(i * angleJump) * radius - 3.5f,
                            7, 7);
                        graphics.DrawString(quarters[i].Name, new Font("Verdana", 12), Brushes.Black, center.X + (float)Math.Cos(i * angleJump) * radius, center.Y + (float)Math.Sin(i * angleJump) * radius - 16);
                    }
                }
                mapImage = mapRaster;
            }

            //Selecting starting quarter
            currentQuarter = quarters[0];
        }

        public Texture2D CreateTownMap()
        {
            Texture2D map;
            using (Bitmap bmp = new Bitmap(mapImage))
            {
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    float angleJump = MathHelper.TwoPi / Game.Settings.TownQuarterCount;
                    float radius = Math.Min(MapImageWidth, MapImageHeight) / 2f - 20f;
                    PointF center = new PointF(MapImageWidth / 2f, MapImageHeight / 2f);
                    for (int i = 0; i < Game.Settings.TownQuarterCount; i++)
                    {
                        float xCoo = center.X + (float)Math.Cos(i * angleJump) * radius;
                        float yCoo = center.Y + (float)Math.Sin(i * angleJump) * radius;
                        if (quarters[i].Owner != null)
                        {
                            graphics.DrawEllipse(new Pen(quarters[i].Owner.Content.DrawingColor, 2), xCoo - 7f, yCoo - 7f, 14, 14);
                        }
                        System.Drawing.Point[] pointerPoints = new System.Drawing.Point[] { new System.Drawing.Point(0, 16), new System.Drawing.Point(0, 0), new System.Drawing.Point(16, 16), new System.Drawing.Point(0, 0), new System.Drawing.Point(16, 0) };
                        for (int j = 0; j < pointerPoints.Length; j++)
                        {
                            pointerPoints[j].X += (int)xCoo;
                            pointerPoints[j].Y += (int)yCoo;
                        }
                        if (quarters[i] == Game.Opponent.Position.Quarter)
                        {
                            graphics.DrawLines(new Pen(System.Drawing.Color.Black, 5), pointerPoints);
                            graphics.DrawLines(new Pen(Game.Opponent.Content.DrawingColor, 3), pointerPoints);
                        }
                        if (quarters[i] == Game.Player.Position.Quarter)
                        {
                            graphics.DrawLines(new Pen(System.Drawing.Color.Black, 5), pointerPoints);
                            graphics.DrawLines(new Pen(Game.Player.Content.DrawingColor, 3), pointerPoints);
                        }
                    }
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    map = Texture2D.FromStream(Game.GraphicsDevice, ms);
                }
            }
            return map;
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
                Vector2 delta = ResolveQuarterPositionDelta(usedInterface);
                Game.Player.MoveTo(
                    new PositionInTown(currentQuarter,
                        Vector3.Transform(Game.Player.PositionInQuarter, Matrix.CreateTranslation(-delta.ToVector3(0)) * Matrix.CreateRotationY(angle)).XZToVector2() // reverse transform of nearest quarter
                        ),
                    Game.Player.Azimuth - angle
                    );

                //Assings player to new space grid
                currentQuarter.SpaceGrid.AddObject(Game.Player);
                Game.Drawer.ShowMessage(gameTime, "You've entered " + currentQuarter.Name + ".");

                //Restart for drawing
                lastNearestInterfaceIndex = -1;
                currentQuarterDrawed = false;

                lastQuarterChange = gameTime.TotalGameTime;
            }
            
            FillDrawer();

            currentQuarter.Update(gameTime, false);
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

                TownQuarterInterface iface = currentQuarter.Interfaces[nearestInterfaceIndex];
                Vector2 delta = ResolveQuarterPositionDelta(iface);
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

        public static Vector2 ResolveQuarterPositionDelta(TownQuarterInterface iface)
        {
            Vector2 delta = Vector2.Zero;
            switch (iface.SidePosition)
            {
                case TownQuarterInterfacePosition.Top:
                    switch (iface.OppositeInterface.SidePosition)
                    {
                        case TownQuarterInterfacePosition.Top:
                            delta.X = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * TownQuarter.SquareWidth;
                            delta.Y = 0;
                            break;
                        case TownQuarterInterfacePosition.Right:
                            delta.X = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * TownQuarter.SquareWidth;
                            delta.Y = -(iface.OppositeInterface.Quarter.BitmapSize.Width * TownQuarter.SquareWidth);
                            break;
                        case TownQuarterInterfacePosition.Bottom:
                            delta.X = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * TownQuarter.SquareWidth;
                            delta.Y = -(iface.OppositeInterface.Quarter.BitmapSize.Height * TownQuarter.SquareWidth);
                            break;
                        case TownQuarterInterfacePosition.Left:
                            delta.X = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * TownQuarter.SquareWidth;
                            delta.Y = 0;
                            break;
                    }
                    break;
                case TownQuarterInterfacePosition.Right:
                    switch (iface.OppositeInterface.SidePosition)
                    {
                        case TownQuarterInterfacePosition.Top:
                            delta.X = iface.Quarter.BitmapSize.Width * TownQuarter.SquareWidth;
                            delta.Y = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * TownQuarter.SquareWidth;
                            break;
                        case TownQuarterInterfacePosition.Right:
                            delta.X = (iface.Quarter.BitmapSize.Width + iface.OppositeInterface.Quarter.BitmapSize.Width) * TownQuarter.SquareWidth;
                            delta.Y = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * TownQuarter.SquareWidth;
                            break;
                        case TownQuarterInterfacePosition.Bottom:
                            delta.X = (iface.Quarter.BitmapSize.Width + iface.OppositeInterface.Quarter.BitmapSize.Height) * TownQuarter.SquareWidth;
                            delta.Y = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * TownQuarter.SquareWidth;
                            break;
                        case TownQuarterInterfacePosition.Left:
                            delta.X = iface.Quarter.BitmapSize.Width * TownQuarter.SquareWidth;
                            delta.Y = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * TownQuarter.SquareWidth;
                            break;
                    }
                    break;

                case TownQuarterInterfacePosition.Bottom:
                    switch (iface.OppositeInterface.SidePosition)
                    {
                        case TownQuarterInterfacePosition.Top:
                            delta.X = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * TownQuarter.SquareWidth;
                            delta.Y = (iface.Quarter.BitmapSize.Height * TownQuarter.SquareWidth);
                            break;
                        case TownQuarterInterfacePosition.Right:
                            delta.X = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * TownQuarter.SquareWidth;
                            delta.Y = ((iface.Quarter.BitmapSize.Height + iface.OppositeInterface.Quarter.BitmapSize.Width) * TownQuarter.SquareWidth);
                            break;
                        case TownQuarterInterfacePosition.Bottom:
                            delta.X = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * TownQuarter.SquareWidth;
                            delta.Y = ((iface.Quarter.BitmapSize.Height + iface.OppositeInterface.Quarter.BitmapSize.Height) * TownQuarter.SquareWidth);
                            break;
                        case TownQuarterInterfacePosition.Left:
                            delta.X = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * TownQuarter.SquareWidth;
                            delta.Y = (iface.Quarter.BitmapSize.Height * TownQuarter.SquareWidth);
                            break;
                    }
                    break;


                case TownQuarterInterfacePosition.Left:
                    switch (iface.OppositeInterface.SidePosition)
                    {
                        case TownQuarterInterfacePosition.Top:
                            delta.X = 0;
                            delta.Y = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * TownQuarter.SquareWidth;
                            break;
                        case TownQuarterInterfacePosition.Right:
                            delta.X = -(iface.OppositeInterface.Quarter.BitmapSize.Width) * TownQuarter.SquareWidth;
                            delta.Y = (iface.BitmapPosition - iface.OppositeInterface.BitmapPosition) * TownQuarter.SquareWidth;
                            break;
                        case TownQuarterInterfacePosition.Bottom:
                            delta.X = -(iface.OppositeInterface.Quarter.BitmapSize.Height) * TownQuarter.SquareWidth;
                            delta.Y = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * TownQuarter.SquareWidth;
                            break;
                        case TownQuarterInterfacePosition.Left:
                            delta.X = 0;
                            delta.Y = (iface.BitmapPosition + iface.OppositeInterface.BitmapPosition + 1) * TownQuarter.SquareWidth;
                            break;
                    }
                    break;
            }
            return delta;
        }

        public static float ResolveQuarterAzimuthDelta(TownQuarterInterfacePosition mainQuarterPosition, TownQuarterInterfacePosition neighborQuarterPosition)
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
            mapImage.Dispose();
            foreach (TownQuarter quarter in quarters)
                quarter.Dispose();
        }

    }
}
