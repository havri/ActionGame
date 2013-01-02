using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using Microsoft.Xna.Framework;

namespace ActionGame.Tasks
{
    public abstract class Task
    {
        protected Human holder;
        protected Queue<WayPoint> wayPoints;

        TimeSpan lastStepMade;

        public Task(Human holder)
        {
            this.holder = holder;
            this.wayPoints = new Queue<WayPoint>();
            lastStepMade = new TimeSpan(0,0,0);
        }

        public abstract bool IsComplete();

        public virtual void Update(GameTime gameTime)
        {
            if (wayPoints.Count > 0)
            {
                WayPoint nextWayPoint = wayPoints.Peek();
                if (holder.Pivot.DistanceTo(nextWayPoint.Point) <= Human.EpsilonDistance)
                {
                    wayPoints.Dequeue();
                }
            }

            if (wayPoints.Count > 0 && gameTime.TotalGameTime - lastStepMade > Human.StepTimeOut)
            {
                lastStepMade = gameTime.TotalGameTime;
                WayPoint nextWayPoint = wayPoints.Peek();
                holder.GoThisWay(nextWayPoint.Point);
            }
        }
    }
}
