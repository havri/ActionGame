using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Space;
using Microsoft.Xna.Framework;
using ActionGame.World;
using ActionGame.Exceptions;

namespace ActionGame.QSP
{
    public class Grid
    {
        readonly ISet<Quadrangle> objects;
        readonly GridField[] fields;
        readonly GridField outside;
        readonly int width, height;
        readonly float fieldWidth, fieldHeight;

        public Grid(int width, int height, float fieldWidth, float fieldHeight)
        {
            fields = new GridField[width * height];
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = new GridField();
            }
            this.width = width;
            this.height = height;
            this.fieldWidth = fieldWidth;
            this.fieldHeight = fieldHeight;
            objects = new HashSet<Quadrangle>();
            outside = new GridField();
        }

        public void Fill(IEnumerable<Quadrangle> objects)
        {
            foreach (Quadrangle obj in objects)
            {
                AddObject(obj);
            }
        }

        public void Update()
        {
            foreach (Quadrangle obj in objects)
            {
                IEnumerable<GridField> rightFields = GetFieldsByObject(obj);
                HashSet<GridField> newFields = new HashSet<GridField>(rightFields);
                HashSet<GridField> oldFields = new HashSet<GridField>(obj.SpacePartitioningFields);
                newFields.ExceptWith(obj.SpacePartitioningFields);
                oldFields.ExceptWith(rightFields);
                foreach (GridField oldField in oldFields)
                {
                    oldField.RemoveObject(obj);
                }
                foreach (GridField newField in newFields)
                {
                    newField.AddObject(obj);
                }
            }
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

        public void AddPathGraphVertex(PathGraphVertex vertex)
        {
            int x = (int)(vertex.Position.PositionInQuarter.X / fieldWidth);
            int y = (int)(vertex.Position.PositionInQuarter.Y / fieldHeight);
            GetField(x, y).AddPathGraphVertex(vertex);
        }

        IEnumerable<GridField> GetFieldsByObject(Quadrangle obj)
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
                (int)(maxX / fieldWidth) - wX + 1,
                (int)(maxY / fieldHeight) - wY + 1
                );

            List<GridField> result = new List<GridField>();
            for (int y = 0; y < window.Height; y++)
            {
                int actY = y + window.Y;
                for (int x = 0; x < window.Width; x++)
                {
                    int actX = x + window.X;
                    Quadrangle fieldOne = new Quadrangle(
                        new Vector2(actX * fieldWidth, actY * fieldHeight),
                        new Vector2(actX * fieldWidth + fieldWidth, actY * fieldHeight),
                        new Vector2(actX * fieldWidth, actY * fieldHeight + fieldHeight),
                        new Vector2(actX * fieldWidth + fieldWidth, actY * fieldHeight + fieldHeight)
                        );

                    if (fieldOne.IsInCollisionWith(obj) && actX < width && actY < height)
                    {
                        result.Add(GetField(actX, actY));
                    }
                }
            }
            if (result.Count == 0)
            {
                result.Add(outside);
            }
            return result;
        }

        public static bool IsInCollision(Quadrangle obj)
        {
            foreach (GridField field in obj.SpacePartitioningFields)
            {
                if (field.GetCollisions(obj).Any())
                {
                    return true;
                }
            }
            return false;
        }

        public void AddObject(Quadrangle obj)
        {
            objects.Add(obj);
        }

        public void RemoveObject(Quadrangle obj)
        {
            objects.Remove(obj);
        }

        public PathGraphVertex FindNearestPathGraphVertex(Vector2 from)
        {
            foreach (GridField field in GetFieldsByRounds(from))
            {
                foreach (PathGraphVertex v in field.PathGraphVertices)
                {
                    Quadrangle pathObj = new Quadrangle(v.Position.PositionInQuarter, v.Position.PositionInQuarter, from, from);
                    if (!IsInCollision(pathObj))
                    {
                        return v;
                    }
                }
            }
            throw new PathNotFoundException("Couldn't find path graph vertex witch has clear way to specified position.");
        }

        IEnumerable<GridField> GetFieldsByRounds(Vector2 from)
        {
            int x = (int)(from.X / fieldWidth);
            int y = (int)(from.Y / fieldHeight);

            yield return GetField(x, y);

            int r = 1;
            bool found;
            do
            {
                found = false;
                foreach (int v in new int[] { y - r, y + r })
                {
                    if (v >= 0 && v < height)
                    {
                        for (int h = x - r; h <= x + r; h++)
                        {
                            if (h >= 0 && h < width)
                            {
                                found = true;
                                yield return GetField(h, v);
                            }
                        }
                    }
                }

                foreach (int h in new int[] { y - r + 1, y + r - 1 })
                {
                    if (h >= 0 && h < width)
                    {
                        for (int v = y - r; v <= y + r; v++)
                        {
                            if (v >= 0 && v < height)
                            {
                                found = true;
                                yield return GetField(h, v);
                            }
                        }
                    }
                }
                r++;
            }
            while (found);
        }
    }
}
