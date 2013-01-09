using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Space;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;

namespace ActionGame.QSP
{
    class Grid
    {
        readonly GridField[] fields;
        readonly int width, height;
        readonly float fieldWidth, fieldHeight;

        public Grid(int width, int height, float fieldWidth, float fieldHeight)
        {
            fields = new GridField[width * height];
            this.width = width;
            this.height = height;
            this.fieldWidth = fieldWidth;
            this.fieldHeight = fieldHeight;
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

        public void AddObject(Quadrangle obj)
        {
            Vector2[] corners = new Vector2[] {
                obj.UpperLeftCorner,
                obj.UpperRightCorner,
                obj.LowerRightCorner,
                obj.LowerLeftCorner
            };

            float minX = corners.Min(c => c.X),
                maxX = corners.Max(c => c.X),
                 minY = corners.Min(c => c.Y),
                maxY = corners.Max(c => c.Y);

            int wX = (int)(minX / fieldWidth), //auto floor
                wY = (int)(minY / fieldHeight);
            Rectangle window = new Rectangle(wX, wY,
                (int)(maxX / fieldWidth) - wX,
                (int)((maxY - maxY) / fieldHeight) - wY
                );

            for (int y = 0; y < window.Height; y++)
            {
                for (int x = 0; x < window.Width; x++)
                {
                    Quadrangle fieldOne = new Quadrangle(
                        new Vector2(x * fieldWidth, y * fieldHeight),
                        new Vector2(x * fieldWidth + fieldWidth, y * fieldHeight),
                        new Vector2(x * fieldWidth, y * fieldHeight + fieldHeight),
                        new Vector2(x * fieldWidth + fieldWidth, y * fieldHeight + fieldHeight)
                        );

                    if (fieldOne.IsInCollisionWith(obj))
                    {
                        GetField(x, y).AddObject(obj);
                    }
                }
            }
        }
    }
}
