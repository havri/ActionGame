using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionGame.World
{
    public class PathGraphVertex
    {
        PositionInTown position;
        List<PathGraphVertex> neightbors;

        public PathGraphVertex(PositionInTown position)
        {
            neightbors = new List<PathGraphVertex>();
            this.position = position;
        }

        public void AddNeighbor(PathGraphVertex neighbor)
        {
            neightbors.Add(neighbor);
        }

        public void AddNeighborBothDirection(PathGraphVertex neighbor)
        {
            neightbors.Add(neighbor);
            neighbor.AddNeighbor(this);
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
