using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.Space;
using ActionGame.Tools;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;
using ActionGame.World;
using ActionGame.QSP;

namespace ActionGame.Tasks
{
    /// <summary>
    /// Task for killing people.
    /// </summary>
    public class KillTask : Task
    {
        static readonly TimeSpan RecomputeWaypointsTimeout = new TimeSpan(0, 0, 3);
        TimeSpan lastUpdatedWaypoints = TimeSpan.Zero;
        TownQuarter lastTargetQuarter = null;
        readonly Human target;
        
        public Human Target
        {
            get
            {
                return target;
            }
        }
        bool goStraightToTarget = false;

        /// <summary>
        /// Creates a new kill task.
        /// </summary>
        /// <param name="holder">The holder of the task</param>
        /// <param name="target">The target human that has to be killed</param>
        public KillTask(Human holder, Human target)
            : base(holder)
        {
            this.target = target; 
        }
        /// <summary>
        /// Updates the kill task logic.  It navigates the holder to the target and waits for him to shoot the target. If the holder has not a usable gun, it leads him to search for a toolbox. It can also lead him to for a healbox if it is needed.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Update(GameTime gameTime)
        {
            if (Holder.SelectedTool is Gun && ((Gun)Holder.SelectedTool).Type.Range < TownQuarter.SquareWidth)
            { 
                Box nearestToolBox = Holder.Position.Quarter.GetNearestBox(Holder.Position, true);
                if (nearestToolBox != null)
                {
                    Holder.AddUrgentTask(new MoveTask(Holder, nearestToolBox.Pivot));
                }
            }
            int neededHealth = target.Health - (Holder.SelectedTool is Gun ? ((Gun)Holder.SelectedTool).Type.Damage : 0);
            if (Holder.Health < neededHealth)
            {
                Box nearestHealBox = Holder.Position.Quarter.GetNearestBox(Holder.Position, false);
                if (nearestHealBox != null)
                {
                    Holder.AddUrgentTask(new MoveTask(Holder, nearestHealBox.Pivot));
                }
            }

            if (gameTime.TotalGameTime - lastUpdatedWaypoints > RecomputeWaypointsTimeout && ( lastTargetQuarter != target.Position.Quarter || target.Position.Quarter == Holder.Position.Quarter ))
            {
                lastTargetQuarter = target.Position.Quarter;
                goStraightToTarget = false;
                if (Holder.Position.Quarter == target.Position.Quarter)
                {
                    Vector2 wayVect = (target.Position.PositionInQuarter - Holder.Position.PositionInQuarter);
                    float direction = (wayVect.GetAngle() + 1 * MathHelper.PiOver2) % MathHelper.TwoPi;
                    Quadrangle viewLine = Quadrangle.CreateBand(Holder.Pivot.PositionInQuarter, direction, Holder.Size.X, wayVect.Length());
                    goStraightToTarget = !Grid.IsInCollision(viewLine, c => !(c is Human));
                }
                if (!goStraightToTarget)
                {
                    RecomputeWaypoints(Holder.Position, target.Position);
                }
                lastUpdatedWaypoints = gameTime.TotalGameTime;
            }

            if (goStraightToTarget)
            {
                if (Holder.Position.MinimalDistanceTo(target.Position) > TownQuarter.SquareWidth)
                {

                    Holder.GoThisWay(target.Position, (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else
                {
                    Holder.TurnThisWay(target.Position, (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
            }
            else
            {
                base.Update(gameTime);
            }
        }

        /// <summary>
        /// Says whether the target is dead.
        /// </summary>
        /// <returns>True if the target was killed.</returns>
        public override bool IsComplete()
        {
            return target.Health <= 0;
        }

        /// <summary>
        /// Gets the target's current town quarter.
        /// </summary>
        public override TownQuarter TargetQuarter
        {
            get { return target.Position.Quarter; }
        }
    }
}
