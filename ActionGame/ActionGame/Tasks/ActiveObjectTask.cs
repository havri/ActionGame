using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Objects;
using ActionGame.People;
using Microsoft.Xna.Framework;

namespace ActionGame.Tasks
{
    class ActionObjectTask : Task
    {

        readonly ActionObject actionObject;
        TimeSpan actionStart = TimeSpan.Zero;
        bool started = false;
        bool complete = false;
        public ActionObjectTask(ActionObject actionObject, Human holder)
            :base(holder)
        {
            this.actionObject = actionObject;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (actionObject.IsAvailableFor(Holder))
            {
                if (!started)
                {
                    actionObject.StartAction(Holder, gameTime);
                    actionStart = gameTime.TotalGameTime;
                    started = true;
                }
                else
                {
                    if (gameTime.TotalGameTime - actionStart > actionObject.ActionDuration)
                    {
                        actionObject.EndAction(Holder, gameTime);
                        complete = true;
                    }
                }
            }
            else
            {
                if (WayPoints.Count == 0 && !IsComplete())
                {
                    RecomputeWaypoints(Holder.Position, actionObject.Position);
                }
            }
        }

        public override bool IsComplete()
        {
            return complete;
        }
    }
}
