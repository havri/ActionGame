using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Exceptions;

namespace ActionGame.World
{
    public class PathGraph
    {
        public static bool IsConnected(IEnumerable<PathGraphVertex> graph)
        {
            Dictionary<PathGraphVertex, bool> visited = new Dictionary<PathGraphVertex,bool>();
            foreach(var v in graph)
            {
                if(!visited.ContainsKey(v))
                {
                    visited.Add(v, false);
                }
            }
            if (visited.Count == 0)
                return true;
            int visitedCount = 0;
            Queue<PathGraphVertex> queue = new Queue<PathGraphVertex>();
            queue.Enqueue(graph.First());
            while (queue.Count != 0)
            {
                PathGraphVertex current = queue.Dequeue();
                if (!visited[current])
                {
                    visited[current] = true;
                    visitedCount++;
                    foreach(var n in current.Neighbors)
                    {
                        queue.Enqueue(n);
                    }
                }
            }
            return visitedCount == visited.Count;
        }

        /// <summary>
        /// Searches for shortest path in the graph. Uses Dijkstra 1-1 form (A*).
        /// </summary>
        /// <param name="from">Source vertex</param>
        /// <param name="to">Target vertex</param>
        /// <returns>Set of vertices forming result path</returns>
        public static IEnumerable<PathGraphVertex> FindShortestPath(PathGraphVertex from, PathGraphVertex to)
        {
            LinkedList<PathGraphVertex> resultPath = new LinkedList<PathGraphVertex>();
            HashSet<PathGraphVertex> closed = new HashSet<PathGraphVertex>();
            HashSet<PathGraphVertex> open = new HashSet<PathGraphVertex>();
            open.Add(from);
            Dictionary<PathGraphVertex, PathGraphVertex> cameFrom = new Dictionary<PathGraphVertex, PathGraphVertex>();
            Dictionary<PathGraphVertex, float> gScore = new Dictionary<PathGraphVertex, float>();
            gScore.Add(from, 0);
            Dictionary<PathGraphVertex, float> fScore = new Dictionary<PathGraphVertex, float>();
            fScore.Add(from, gScore[from] + EuklidDistance(from, to));

            while (open.Count != 0)
            {
                ///TODO: This selection must be faster.
                PathGraphVertex current = open.OrderBy(x => fScore[x]).First();
                if (current == to)
                {
                    PathGraphVertex t = to;
                    while(t != from)
                    {
                        resultPath.AddFirst(t);
                        t = cameFrom[t];
                    }
                    resultPath.AddFirst(from);
                    return resultPath;
                }
                open.Remove(current);
                closed.Add(current);
                foreach (PathGraphVertex n in current.Neighbors)
                {
                    if (closed.Contains(n))
                    {
                        continue;
                    }
                    else
                    {
                        float tempGSore = gScore[current] + current.DistanceToNeighbor(n);
                        if (!open.Contains(n) || tempGSore <= gScore[n])
                        {
                            cameFrom.Add(n, current);
                            gScore.Add(n, tempGSore);
                            fScore.Add(n, gScore[n] + EuklidDistance(current, n));
                            if (!open.Contains(n))
                            {
                                open.Add(n);
                            }
                        }
                    }
                }
            }

            throw new PathNotFoundException("Source and target vertices aren't in the same component.");
        }

        static float EuklidDistance(PathGraphVertex u, PathGraphVertex v)
        {
            return u.Position.DistanceTo(v.Position);
        }
    }
}
