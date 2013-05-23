using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.World;
using Microsoft.Xna.Framework;

namespace ActionGame.Tasks
{
    /// <summary>
    /// General abstraction for human's task. It provides path finding and human navigating and controlling.
    /// </summary>
    public abstract class Task
    {
        readonly Human holder;
        /// <summary>
        /// Gets the holder of this task.
        /// </summary>
        public Human Holder
        {
            get
            {
                return holder;
            }
        }
        readonly Queue<WayPoint> wayPoints;
        /// <summary>
        /// Gets the current waypoint queue.
        /// </summary>
        protected Queue<WayPoint> WayPoints
        {
            get
            {
                return wayPoints;
            }
        }
        /// <summary>
        /// Creates a new task
        /// </summary>
        /// <param name="holder">Holder of the task</param>
        public Task(Human holder)
        {
            this.holder = holder;
            this.wayPoints = new Queue<WayPoint>();
        }

        /// <summary>
        /// Says whether it is complete.
        /// </summary>
        /// <returns></returns>
        public abstract bool IsComplete();

        /// <summary>
        /// Updates the logic and leads the human follow the task objective.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            if (wayPoints.Count != 0)
            {
                WayPoint nextWayPoint = wayPoints.Peek();
                if (holder.Pivot.MinimalDistanceTo(nextWayPoint.Point) <= Human.EpsilonDistance)
                {
                    wayPoints.Dequeue();
                }
            }

            if (wayPoints.Count != 0)
            {
                WayPoint nextWayPoint = wayPoints.Peek();
                float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                holder.GoThisWay(nextWayPoint.Point, seconds);
            }
        }

        /// <summary>
        /// Recomputes the waypoints according to path graph to get to the task target.
        /// </summary>
        /// <param name="from">Start position</param>
        /// <param name="to">Destination position</param>
        protected void RecomputeWaypoints(PositionInTown from, PositionInTown to)
        {
            wayPoints.Clear();
            foreach (PathGraphVertex v in PathGraph.FindShortestPath(from, to, !(holder is Opponent)))
            {
                wayPoints.Enqueue(new WayPoint(v.Position));
            }
            wayPoints.Enqueue(new WayPoint(to));
        }
        
        /// <summary>
        /// Clears the waypoint queue.
        /// </summary>
        public void ClearWaypoints()
        {
            wayPoints.Clear();
        }

        /// <summary>
        /// Gets the target town quarter.
        /// </summary>
        public abstract TownQuarter TargetQuarter { get; }

        public override string ToString()
        {
            return String.Format("{0}WP[{1}]", base.ToString(), wayPoints.Count);
        }
    }
}
