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

        /// <summary>
        /// Creates a new move task.
        /// </summary>
        /// <param name="holder">The holder of the task.</param>
        /// <param name="destination">The destination that has to be reached</param>
        public MoveTask(Human holder, PositionInTown destination)
            : base(holder)
        {
            this.destination = destination;
        }
        /// <summary>
        /// Updates the task logic - leads the human toward the destination.
        /// </summary>
        /// <param name="gameTime"></param>
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

        /// <summary>
        /// Says whether the holder has reached the destination.
        /// </summary>
        /// <returns>True if he is there</returns>
        public override bool IsComplete()
        {
            return (Holder.Pivot.MinimalDistanceTo(destination) <= Human.EpsilonDistance);
        }


        /// <summary>
        /// Gets the destination quarter.
        /// </summary>
        public override TownQuarter TargetQuarter
        {
            get { return destination.Quarter; }
        }
    }
}
