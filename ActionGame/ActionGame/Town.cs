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
    class Town : IDisposable
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
        
        public Town(int quarterCount, ContentManager content, Matrix worldTransform, GraphicsDevice graphicsDevice)
        {
            quarters = new TownQuarter[quarterCount];
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
        }

        public void Dispose()
        {
            foreach (TownQuarter quarter in quarters)
                quarter.Dispose();
        }
    }
}
