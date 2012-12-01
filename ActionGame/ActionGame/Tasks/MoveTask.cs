using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.World;
using ActionGame.People;

namespace ActionGame.Tasks
{
    /// <summary>
    /// Task of type "move somewhere".
    /// </summary>
    public class MoveTask : Task
    {
        PositionInTown destination;
        public MoveTask(Human holder, PositionInTown destination)
            : base(holder)
        {
            this.destination = destination;
            computeWayPoints();
        }
         
        void computeWayPoints()
        {
            this.wayPoints.Clear();
            this.wayPoints.Enqueue(new WayPoint(destination));
        }

        public override bool IsComplete()
        {
            return (holder.Pivot.DistanceTo(destination) <= Human.EpsilonDistance);
        }
    }
}
