using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;

namespace ActionGame.Tasks
{
    public abstract class Task
    {
        protected Human holder;
        protected Queue<WayPoint> wayPoints;

        public Task(Human holder)
        {
            this.holder = holder;
            this.wayPoints = new Queue<WayPoint>();
        }

        public abstract bool IsComplete();

        public virtual void Update()
        {
            if (wayPoints.Count > 0)
            {
                WayPoint nextWayPoint = wayPoints.Peek();
                if (holder.Pivot.DistanceTo(nextWayPoint.Point) <= Human.EpsilonDistance)
                {
                    wayPoints.Dequeue();
                }
            }

            if (wayPoints.Count > 0)
            {
                WayPoint nextWayPoint = wayPoints.Peek();
                holder.GoThisWay(nextWayPoint.Point);
            }
        }
    }
}
