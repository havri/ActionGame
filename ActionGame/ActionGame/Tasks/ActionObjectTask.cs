using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Objects;
using ActionGame.People;
using Microsoft.Xna.Framework;
using ActionGame.Extensions;
using ActionGame.Space;
using ActionGame.QSP;

namespace ActionGame.Tasks
{
    /// <summary>
    /// Task for performing the action object's action.
    /// </summary>
    class ActionObjectTask : Task
    {
        static readonly TimeSpan RecomputeWaypointsTimeout = new TimeSpan(0, 0, 3);
        TimeSpan lastUpdatedWaypoints = TimeSpan.Zero;
        bool goStraightToObject= false;
        readonly ActionObject actionObject;
        TimeSpan actionStart = TimeSpan.Zero;
        bool started = false;
        bool complete = false;
        /// <summary>
        /// Creates a new action object task.
        /// </summary>
        /// <param name="actionObject">The specified action object</param>
        /// <param name="holder">The holder of this task</param>
        public ActionObjectTask(ActionObject actionObject, Human holder)
            :base(holder)
        {
            this.actionObject = actionObject;
        }

        /// <summary>
        /// Updates the task logic. Leads the human toward the action object and then lets him perform the action.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - lastUpdatedWaypoints > RecomputeWaypointsTimeout && actionObject.Position.Quarter == Holder.Position.Quarter)
            {
                goStraightToObject = false;

                Vector2 wayVect = (actionObject.Position.PositionInQuarter - Holder.Position.PositionInQuarter);
                float direction = (wayVect.GetAngle() + 1 * MathHelper.PiOver2) % MathHelper.TwoPi;
                Quadrangle viewLine = Quadrangle.CreateBand(Holder.Pivot.PositionInQuarter, direction, Holder.Size.X, wayVect.Length());
                goStraightToObject = !Grid.IsInCollision(viewLine, c => !(c is Human));
                
                if (!goStraightToObject)
                {
                    RecomputeWaypoints(Holder.Position, actionObject.Position);
                }
                lastUpdatedWaypoints = gameTime.TotalGameTime;
            }

            if (goStraightToObject)
            {
                Holder.GoThisWay(actionObject.Position, (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
            {
                base.Update(gameTime);
            }

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
                    lastUpdatedWaypoints = gameTime.TotalGameTime;
                }
            }
        }
        /// <summary>
        /// Says whether the task is complete.
        /// </summary>
        /// <returns>True if the action was performed yet</returns>
        public override bool IsComplete()
        {
            return complete;
        }
        /// <summary>
        /// Gets the target quarter - quarter the action object is located.
        /// </summary>
        public override World.TownQuarter TargetQuarter
        {
            get { return actionObject.Position.Quarter; }
        }
    }
}
