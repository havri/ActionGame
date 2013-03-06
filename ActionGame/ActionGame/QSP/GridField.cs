using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Space;
using ActionGame.World;

namespace ActionGame.QSP
{
    public class GridField
    {
        readonly ISet<Quadrangle> objects;
        readonly ISet<PathGraphVertex> pathGraphVertices;
        bool isInCollisionProcessing = false;
        readonly List<Quadrangle> objectsForRemove;

        public GridField()
        {
            objects = new HashSet<Quadrangle>();
            pathGraphVertices = new HashSet<PathGraphVertex>();
            objectsForRemove = new List<Quadrangle>();
        }

        public void AddObject(Quadrangle obj)
        {
            objects.Add(obj);
            obj.SpacePartitioningFields.Add(this);
        }

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

        public void AddPathGraphVertex(PathGraphVertex vertex)
        {
            pathGraphVertices.Add(vertex);
        }

        public ISet<Quadrangle> Objects
        {
            get { return objects; }
        }

        public ISet<PathGraphVertex> PathGraphVertices
        {
            get { return pathGraphVertices; }
        }

        public int Count
        {
            get { return objects.Count; }
        }

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
