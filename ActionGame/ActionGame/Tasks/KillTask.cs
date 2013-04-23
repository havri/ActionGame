using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.Space;
using ActionGame.Tools;
using Microsoft.Xna.Framework;

namespace ActionGame.Tasks
{
    public class KillTask : Task
    {
        static readonly TimeSpan RecomputeWaypointsInSameQuarterTimeout = new TimeSpan(0, 0, 4);
        static readonly TimeSpan RecomputeWaypointsInOtherQuarterTimeout = new TimeSpan(0, 0, 16);
        TimeSpan lastUpdatedWaypoints = TimeSpan.Zero;
        readonly Human target;
        public Human Target
        {
            get
            {
                return target;
            }
        }
        public KillTask(Human holder, Human target)
            : base(holder)
        {
            this.target = target; 
        }

        public override void Update(GameTime gameTime)
        {
            bool moved = false;
            if (Holder.Position.Quarter == target.Position.Quarter)
            {
                if (gameTime.TotalGameTime - lastUpdatedWaypoints > RecomputeWaypointsInSameQuarterTimeout)
                {
                    Quadrangle viewLine = new Quadrangle(Holder.Position.PositionInQuarter, Holder.Position.PositionInQuarter, target.Position.PositionInQuarter, target.Position.PositionInQuarter);
                    IEnumerable<Quadrangle> colliders = from c in Holder.Position.Quarter.SpaceGrid.GetAllCollisions(viewLine) where c != Holder && c != target select c;
                    if (!colliders.Any())
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
                    }   
                }
            }
            else
            {
                if (gameTime.TotalGameTime - lastUpdatedWaypoints > RecomputeWaypointsInOtherQuarterTimeout)
                {
                    RecomputeWaypoints(Holder.Position, target.Position);
                    lastUpdatedWaypoints = gameTime.TotalGameTime;
                }
            }
            if (!moved)
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
