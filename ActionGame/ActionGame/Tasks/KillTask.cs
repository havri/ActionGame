using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.Space;
using ActionGame.Tools;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;

namespace ActionGame.Tasks
{
    public class KillTask : Task
    {
        static readonly TimeSpan RecomputeWaypointsTimeout = new TimeSpan(0, 0, 4);
        TimeSpan lastUpdatedWaypoints = TimeSpan.Zero;
        readonly Human target;
        public Human Target
        {
            get
            {
                return target;
            }
        }
        bool goStraightToTarget = false;

        public KillTask(Human holder, Human target)
            : base(holder)
        {
            this.target = target; 
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - lastUpdatedWaypoints > RecomputeWaypointsTimeout)
            {
                goStraightToTarget = false;
                if (Holder.Position.Quarter == target.Position.Quarter)
                {
                    Vector2 wayVect = (target.Position.PositionInQuarter - Holder.Position.PositionInQuarter);
                    float direction = (wayVect.GetAngle() + 1 * MathHelper.PiOver2) % MathHelper.TwoPi;
                    Quadrangle viewLine = Quadrangle.CreateBand(Holder.Pivot.PositionInQuarter, direction, Holder.Size.X, wayVect.Length());
                    IEnumerable<Quadrangle> colliders = from c in Holder.Position.Quarter.SpaceGrid.GetAllCollisions(viewLine) where c != Holder && c != target select c;
                    goStraightToTarget = !colliders.Any();
                }
                if (!goStraightToTarget)
                {
                    RecomputeWaypoints(Holder.Position, target.Position);
                    lastUpdatedWaypoints = gameTime.TotalGameTime;
                }
                /*if (!colliders.Any())
                {
                    WayPoints.Clear();
                    if (!(Holder.SelectedTool is Gun && ((Gun)Holder.SelectedTool).Type.Range < Holder.Position.MinimalDistanceTo(target.Position)))
                    {
                        WayPoints.Enqueue(new WayPoint(target.Position));
                    }
                    else
                    {
                        Holder.GoThisWay(target.Position, (float)gameTime.ElapsedGameTime.TotalSeconds);
                        moved = true;
                    }
                }
                else
                {
                    RecomputeWaypoints(Holder.Position, target.Position);
                    lastUpdatedWaypoints = gameTime.TotalGameTime;
                }*/
            }

            if (goStraightToTarget)
            {
                Holder.GoThisWay(target.Position, (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
            {
                base.Update(gameTime);
            }
        }

        public override bool IsComplete()
        {
            return target.Health <= 0;
        }

        public override World.TownQuarter TargetQuarter
        {
            get { return target.Position.Quarter; }
        }
    }
}
