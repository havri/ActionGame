using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Space;
using Microsoft.Xna.Framework;
using ActionGame.World;
using ActionGame.Extensions;
using ActionGame.People;

namespace ActionGame.QSP
{
    /// <summary>
    /// The space partitioning structure based on square grid system.
    /// </summary>
    public class Grid
    {
        readonly ISet<Quadrangle> objects;
        readonly GridField[] fields;
        readonly GridField outside;
        readonly int width, height;
        readonly float fieldWidth, fieldHeight;

        /// <summary>
        /// Creates a new grid of specifies size.
        /// </summary>
        /// <param name="width">Width of the whole grid</param>
        /// <param name="height">Height of the whole grid</param>
        /// <param name="fieldWidth">Width of one field</param>
        /// <param name="fieldHeight">Height of one field</param>
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

        /// <summary>
        /// Register set of objects.
        /// </summary>
        /// <param name="objects">The objects</param>
        public void Fill(IEnumerable<Quadrangle> objects)
        {
            foreach (Quadrangle obj in objects)
            {
                AddObject(obj);
            }
        }
        /// <summary>
        /// Updates the grid logic - recomputes field classification of every object in the grid.
        /// </summary>
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

        /// <summary>
        /// Gets filed inside a specific rectangle inside the grid.
        /// </summary>
        /// <param name="window">The window rectangle</param>
        /// <returns>Set of field inside the window</returns>
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

        /// <summary>
        /// Registers a new path graph vertex into the grid.
        /// </summary>
        /// <param name="vertex">The path graph vertex</param>
        public void AddPathGraphVertex(PathGraphVertex vertex)
        {
            int x = (int)(vertex.Position.PositionInQuarter.X / fieldWidth);
            int y = (int)(vertex.Position.PositionInQuarter.Y / fieldHeight);
            GetField(x, y).AddPathGraphVertex(vertex);
        }

        /// <summary>
        /// Gets all the fields where the specified object should belong to.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>Set of fields</returns>
        public IEnumerable<GridField> GetFieldsByObject(Quadrangle obj)
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

                    if (fieldOne.IsInCollisionWith(obj) && actX < width && actY < height && actX >= 0 && actY >= 0)
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
        /// <summary>
        /// Says whether the object is in collision with any of the registered objects satisfiing the predicate.
        /// </summary>
        /// <param name="obj">The tested object</param>
        /// <param name="predicate">The select predicate</param>
        /// <returns>True if there is a collision</returns>
        public static bool IsInCollision(Quadrangle obj, Func<Quadrangle, bool> predicate)
        {
            foreach (GridField field in obj.SpacePartitioningFields)
            {
                if (field.GetCollisions(obj).Any(predicate))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Says whether the object is in collision with any of the registered objects.
        /// </summary>
        /// <param name="obj">The tested object</param>
        /// <returns>True if there is a collision</returns>
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
        /// <summary>
        /// Registers a new object.
        /// </summary>
        /// <param name="obj">The object</param>
        public void AddObject(Quadrangle obj)
        {
            objects.Add(obj);
        }
        /// <summary>
        /// Unregisters a new object.
        /// </summary>
        /// <param name="obj">The object</param>
        public void RemoveObject(Quadrangle obj)
        {
            objects.Remove(obj);
            List<GridField> fields = new List<GridField>( obj.SpacePartitioningFields);
            foreach (GridField field in fields)
            {
                field.RemoveObject(obj);
            }
        }
        /// <summary>
        /// Gets the nearest path graph vertex from the specified position.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public PathGraphVertex FindNearestPathGraphVertex(Vector2 from)
        {
            PathGraphVertex res = null;
            PathGraphVertex fallDownResult = null;
            float minDistance = float.MaxValue;
            foreach (GridField field in fields)
            {
                foreach (PathGraphVertex vertex in field.PathGraphVertices)
                {
                    if (fallDownResult == null)
                    {
                        fallDownResult = vertex;
                    }
                    Vector2 way = (from - vertex.Position.PositionInQuarter);
                    float direction = (way.GetAngle() + 1 * MathHelper.PiOver2) % MathHelper.TwoPi;
                    Quadrangle pathObj = Quadrangle.CreateBand(vertex.Position.PositionInQuarter, direction, 0.5f, way.Length());
                    //Quadrangle pathObj = new Quadrangle(vertex.Position.PositionInQuarter, vertex.Position.PositionInQuarter, from, from);
                    if ((vertex.Position.PositionInQuarter - from).Length() < minDistance && !IsInCollision(pathObj, x => !(x is Human)))
                    {
                        res = vertex;
                        minDistance = (vertex.Position.PositionInQuarter - from).Length();
                    }
                }
            }
            if (res != null)
            {
                return res;
            }
            return fallDownResult;
        }

        
        /// <summary>
        /// Gets all the objects that are in collision withe the specified object.
        /// </summary>
        /// <param name="subject">The tested object</param>
        /// <returns>Set off colliding objects</returns>
        public IEnumerable<Quadrangle> GetAllCollisions(Quadrangle subject)
        {
            IEnumerable<GridField> affectedFields = GetFieldsByObject(subject);
            List<Quadrangle> colliders = new List<Quadrangle>();
            foreach (GridField field in affectedFields)
            {
                colliders.AddRange(field.GetCollisions(subject));
            }
            return colliders;
        }
    }
}
