using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using Microsoft.Xna.Framework;

namespace ActionGame.Tasks
{
    public class KillTask : Task
    {
        static readonly TimeSpan RecomputeWaypointsTimeout = new TimeSpan(0, 0, 30);
        TimeSpan lastUpdatedWaypoints = TimeSpan.Zero;
        readonly Human target;
        public KillTask(Human holder, Human target)
            : base(holder)
        {
            this.target = target; 
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (gameTime.TotalGameTime - lastUpdatedWaypoints > RecomputeWaypointsTimeout)
            {
                lastUpdatedWaypoints = gameTime.TotalGameTime;
                RecomputeWaypoints(Holder.Position, target.Position);
            }
        }

        public override bool IsComplete()
        {
            return target.Health <= 0;
        }
    }
}
