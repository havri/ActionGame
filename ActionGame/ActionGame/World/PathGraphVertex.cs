using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionGame.World
{
    /// <summary>
    /// Vertex of the path graph. The path graph is represented by a set of these vertices and list of neighbors inside them.
    /// </summary>
    public class PathGraphVertex
    {
        readonly PositionInTown position;
        readonly HashSet<PathGraphVertex> neightbors;
        readonly Dictionary<PathGraphVertex, float> distances;

        /// <summary>
        /// Creates a new path graph vertex.
        /// </summary>
        /// <param name="position">Position in the town</param>
        public PathGraphVertex(PositionInTown position)
        {
            neightbors = new HashSet<PathGraphVertex>();
            distances = new Dictionary<PathGraphVertex, float>();
            this.position = position;
        }
        /// <summary>
        /// Adds a neighbor to this vertex.
        /// </summary>
        /// <param name="neighbor">The neighbor vertex</param>
        /// <param name="distance">The real distance between this vertex and the neighbor one</param>
        public void AddNeighbor(PathGraphVertex neighbor, float distance)
        {
            neightbors.Add(neighbor);
            distances.Add(neighbor, distance);
        }
        /// <summary>
        /// Adds edges between this and the given neighbor vertex in both directions.
        /// </summary>
        /// <param name="neighbor">The neighbor vertex</param>
        /// <param name="distance">The real distance between this vertex and the neighbor one</param>
        public void AddNeighborBothDirection(PathGraphVertex neighbor, float distance)
        {
            AddNeighbor(neighbor, distance);
            neighbor.AddNeighbor(this, distance);
        }
        /// <summary>
        /// Gets distance to specified neighbor.
        /// </summary>
        /// <param name="neighbor">The tested vertex</param>
        /// <returns>Distance in meters</returns>
        public float GetDistanceToNeighbor(PathGraphVertex neighbor)
        {
            return distances[neighbor];
        }
        /// <summary>
        /// Gets list of neighbors of this vertex.
        /// </summary>
        public IEnumerable<PathGraphVertex> Neighbors
        {
            get { return neightbors; }
        }
        /// <summary>
        /// Gets position of this vertex in the town.
        /// </summary>
        public PositionInTown Position
        {
            get
            {
                return position;
            }
        }
    }
}
