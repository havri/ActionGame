using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.World;
using ActionGame.People;
using Microsoft.Xna.Framework;

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
        }

        public override void Update(GameTime gameTime)
        {
            if (wayPoints.Count == 0 && !IsComplete())
            {
                computeWayPoints();
            }

            base.Update(gameTime);
        }

        void computeWayPoints()
        {
            computeWayPointsFrom(holder.Position);
        }

        void computeWayPointsFrom(PositionInTown from)
        {
            wayPoints.Clear();
            ///TODO: Find closest path vertices and find shortest path.
            wayPoints.Enqueue(new WayPoint(destination));
        }

        public override bool IsComplete()
        {
            return (holder.Pivot.DistanceTo(destination) <= Human.EpsilonDistance);
        }
    }
}
