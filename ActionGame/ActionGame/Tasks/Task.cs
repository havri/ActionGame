using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.World;
using Microsoft.Xna.Framework;

namespace ActionGame.Tasks
{
    public abstract class Task
    {
        readonly Human holder;
        public Human Holder
        {
            get
            {
                return holder;
            }
        }
        readonly Queue<WayPoint> wayPoints;
        protected Queue<WayPoint> WayPoints
        {
            get
            {
                return wayPoints;
            }
        }

        public Task(Human holder)
        {
            this.holder = holder;
            this.wayPoints = new Queue<WayPoint>();
        }

        public abstract bool IsComplete();

        public virtual void Update(GameTime gameTime)
        {
            if (wayPoints.Count > 0)
            {
                WayPoint nextWayPoint = wayPoints.Peek();
                if (holder.Pivot.MinimalDistanceTo(nextWayPoint.Point) <= Human.EpsilonDistance)
                {
                    wayPoints.Dequeue();
                }
            }

            if (wayPoints.Count > 0)
            {
                WayPoint nextWayPoint = wayPoints.Peek();
                float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                holder.GoThisWay(nextWayPoint.Point, seconds);
            }
        }

        protected void RecomputeWaypoints(PositionInTown from, PositionInTown to)
        {
            wayPoints.Clear();
            foreach (PathGraphVertex v in PathGraph.FindShortestPath(from, to))
            {
                wayPoints.Enqueue(new WayPoint(v.Position));
            }
            wayPoints.Enqueue(new WayPoint(to));
        }
    }
}
