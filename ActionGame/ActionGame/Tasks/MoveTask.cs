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
        readonly PositionInTown destination;
        public MoveTask(Human holder, PositionInTown destination)
            : base(holder)
        {
            this.destination = destination;
        }

        public override void Update(GameTime gameTime)
        {
            if (WayPoints.Count == 0 && !IsComplete())
            {
                ComputeWayPoints();
            }

            base.Update(gameTime);
        }

        void ComputeWayPoints()
        {
            ComputeWayPointsFrom(Holder.Position);
        }

        void ComputeWayPointsFrom(PositionInTown from)
        {
            RecomputeWaypoints(from, destination);
        }

        public override bool IsComplete()
        {
            return (Holder.Pivot.MinimalDistanceTo(destination) <= Human.EpsilonDistance);
        }



        public override TownQuarter TargetQuarter
        {
            get { return destination.Quarter; }
        }
    }
}
