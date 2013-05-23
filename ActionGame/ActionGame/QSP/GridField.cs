using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Space;
using ActionGame.World;

namespace ActionGame.QSP
{
    /// <summary>
    /// One field inside a grid partitioning system.
    /// </summary>
    public class GridField
    {
        readonly ISet<Quadrangle> objects;
        readonly ISet<PathGraphVertex> pathGraphVertices;
        bool isInCollisionProcessing = false;
        readonly List<Quadrangle> objectsForRemove;

        /// <summary>
        /// Creates a new field for the partitioning system.
        /// </summary>
        public GridField()
        {
            objects = new HashSet<Quadrangle>();
            pathGraphVertices = new HashSet<PathGraphVertex>();
            objectsForRemove = new List<Quadrangle>();
        }

        /// <summary>
        /// Registers an object in this field.
        /// </summary>
        /// <param name="obj">The object</param>
        public void AddObject(Quadrangle obj)
        {
            objects.Add(obj);
            obj.SpacePartitioningFields.Add(this);
        }
        /// <summary>
        /// Unregisters the specified object.
        /// </summary>
        /// <param name="obj">The object</param>
        public void RemoveObject(Quadrangle obj)
        {
            if (!isInCollisionProcessing)
            {
                objects.Remove(obj);
                obj.SpacePartitioningFields.Remove(this);
            }
            else
            {
                objectsForRemove.Add(obj);
            }
        }
        /// <summary>
        /// Registers a path graph vertex.
        /// </summary>
        /// <param name="vertex">The path graph vertex</param>
        public void AddPathGraphVertex(PathGraphVertex vertex)
        {
            pathGraphVertices.Add(vertex);
        }
        /// <summary>
        /// Gets set of registered objects.
        /// </summary>
        public ISet<Quadrangle> Objects
        {
            get { return objects; }
        }
        /// <summary>
        /// Gets set of registered path graph vertices/
        /// </summary>
        public ISet<PathGraphVertex> PathGraphVertices
        {
            get { return pathGraphVertices; }
        }
        /// <summary>
        /// Gets the number of registered objects.
        /// </summary>
        public int Count
        {
            get { return objects.Count; }
        }
        /// <summary>
        /// Calculates all callision of the specified objects inside this field.
        /// </summary>
        /// <param name="obj">The tested object</param>
        /// <returns>Set of colliding objects from this field</returns>
        public IEnumerable<Quadrangle> GetCollisions(Quadrangle obj)
        {
            isInCollisionProcessing = true;
            foreach (Quadrangle quad in objects)
            {
                if (obj.IsInCollisionWith(quad) && obj != quad)
                {
                    yield return quad;
                }
            }
            isInCollisionProcessing = false;
            foreach (Quadrangle rem in objectsForRemove)
            {
                RemoveObject(rem);
            }
            objectsForRemove.Clear();
        }
    }
}
