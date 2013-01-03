using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionGame.World
{
    public class PathGraphVertex
    {
        readonly PositionInTown position;
        readonly HashSet<PathGraphVertex> neightbors;
        readonly Dictionary<PathGraphVertex, float> distances;

        public PathGraphVertex(PositionInTown position)
        {
            neightbors = new HashSet<PathGraphVertex>();
            distances = new Dictionary<PathGraphVertex, float>();
            this.position = position;
        }

        public void AddNeighbor(PathGraphVertex neighbor, float distance)
        {
            neightbors.Add(neighbor);
            distances.Add(neighbor, distance);
        }

        public void AddNeighborBothDirection(PathGraphVertex neighbor, float distance)
        {
            AddNeighbor(neighbor, distance);
            neighbor.AddNeighbor(this, distance);
        }

        public float DistanceToNeighbor(PathGraphVertex neighbor)
        {
            return distances[neighbor];
        }

        public IEnumerable<PathGraphVertex> Neighbors
        {
            get { return neightbors; }
        }

        public PositionInTown Position
        {
            get
            {
                return position;
            }
        }
    }
}
