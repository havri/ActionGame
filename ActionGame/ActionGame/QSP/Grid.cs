using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ActionGame.QSP
{
    class Grid
    {
        readonly GridField[] fields;
        int width, height;

        public Grid(int width, int height)
        {
            fields = new GridField[width * height];
            this.width = width;
            this.height = height;
        }

        public GridField GetField(int x, int y)
        {
            return fields[y * width + x];
        }

        public IEnumerable<GridField> GetFields(Rectangle window)
        { 
            GridField[] result = new GridField[window.Height * window.Width];
            for (int y = 0; y < window.Height; y++)
            {
                for (int x = 0; x < window.Width; x++)
                {
                    result[y * window.Width + x] = fields[(window.Y + y) * width + (window.X + x)];
                }
            }
            return result;
        }
    }
}
