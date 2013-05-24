using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Extensions;
using ActionGame.Exceptions;

namespace ActionGame.World
{
    /// <summary>
    /// Path graph class. It contains set of static function for work with the path graph vertices.
    /// </summary>
    public class PathGraph
    {
        /// <summary>
        /// Scans the graph whether it is connected.
        /// </summary>
        /// <param name="graph">The set of vertices that have pointer to their neighbor vertices</param>
        /// <returns>True if the graph is connected</returns>
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
        /// Calculates length of specified path.
        /// </summary>
        /// <param name="path">Path vertices</param>
        /// <returns>Absolute length in meters</returns>
        public static float GetLengthOfPath(IEnumerable<PathGraphVertex> path)
        {
            float length = 0;
            PathGraphVertex prev = null;
            foreach (PathGraphVertex v in path)
            {
                if (prev != null)
                {
                    length += prev.GetDistanceToNeighbor(v);
                }
                prev = v;
            }
            return length;
        }

        /// <summary>
        /// Searches for shortest path in the town using path graph. Uses Dijkstra 1-1 form (A*).
        /// </summary>
        /// <param name="from">Source position</param>
        /// <param name="to">Target position</param>
        /// <param name="keepInTheSameQuarter">Indicator whether the searching has to use only vertices from the same quarter as the from position is in</param>
        /// <returns>Set of vertices forming result path</returns>
        public static IEnumerable<PathGraphVertex> FindShortestPath(PositionInTown from, PositionInTown to, bool keepInTheSameQuarter)
        {
            PathGraphVertex start = from.Quarter.FindNearestPathGraphVertex(from.PositionInQuarter);
            PathGraphVertex end = to.Quarter.FindNearestPathGraphVertex(to.PositionInQuarter);
            return FindShortestPath(start, end, keepInTheSameQuarter);
        }

        /// <summary>
        /// Searches for shortest path in the graph. Uses Dijkstra 1-1 form (A*).
        /// </summary>
        /// <param name="from">Source vertex</param>
        /// <param name="to">Target vertex</param>
        /// <param name="keepInTheSameQuarter">Indicator whether the searching has to use only vertices from the same quarter as the from position is in</param>
        /// <returns>Set of vertices forming result path</returns>
        public static IEnumerable<PathGraphVertex> FindShortestPath(PathGraphVertex from, PathGraphVertex to, bool keepInTheSameQuarter)
        {
            keepInTheSameQuarter = keepInTheSameQuarter && from.Position.Quarter == to.Position.Quarter;
            TownQuarter fromQuarter = from.Position.Quarter;

            LinkedList<PathGraphVertex> resultPath = new LinkedList<PathGraphVertex>();
            HashSet<PathGraphVertex> closed = new HashSet<PathGraphVertex>();
            Dictionary<PathGraphVertex, PathGraphVertex> cameFrom = new Dictionary<PathGraphVertex, PathGraphVertex>();
            Dictionary<PathGraphVertex, float> gScore = new Dictionary<PathGraphVertex, float>();
            gScore.Add(from, 0);
            Dictionary<PathGraphVertex, float> fScore = new Dictionary<PathGraphVertex, float>();
            fScore.Add(from, gScore[from] + HeuristicDistance(from, to));
            HashSet<PathGraphVertex> open = new HashSet<PathGraphVertex>();
            open.Add( from);

            while (open.Count != 0)
            {
                float lowestScore = float.MaxValue;
                PathGraphVertex lowestVertex = null;
                foreach (PathGraphVertex openedOne in open)
                {
                    float score = fScore[openedOne];
                    if (score < lowestScore)
                    {
                        lowestVertex = openedOne;
                    }
                }
                PathGraphVertex current = lowestVertex;//open.OrderBy(x => fScore[x]).First();
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
                    if (closed.Contains(n) || (keepInTheSameQuarter && n.Position.Quarter != fromQuarter))
                    {
                        continue;
                    }
                    else
                    {
                        float tempGSore = gScore[current] + current.GetDistanceToNeighbor(n);
                        if (!open.Contains(n) || tempGSore <= gScore[n])
                        {
                            cameFrom.SetValue(n, current);
                            gScore.SetValue(n, tempGSore);
                            fScore.SetValue(n, gScore[n] + HeuristicDistance(current, n));
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

        static float HeuristicDistance(PathGraphVertex u, PathGraphVertex v)
        {
            return u.Position.MinimalDistanceTo(v.Position);
        }
    }
}
