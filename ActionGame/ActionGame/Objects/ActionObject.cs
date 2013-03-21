using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Space;
using Microsoft.Xna.Framework;
using ActionGame.World;
using Microsoft.Xna.Framework.Graphics;
using ActionGame.People;

namespace ActionGame.Objects
{
    public class ActionObject : SpatialObject
    {
        static readonly TimeSpan CheckRangeTimeout = new TimeSpan(0, 0, 0, 0, 250);

        readonly float actionDistance;
        TimeSpan lastTimeRangeChecked = TimeSpan.Zero;
        HashSet<Human> availibleHumans = new HashSet<Human>();

        public ActionObject(float actionDistance, Model model, PositionInTown position, double azimuth, Matrix worldTransform)
            : base(model, position, azimuth, worldTransform)
        {
            this.actionDistance = actionDistance;
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - lastTimeRangeChecked > CheckRangeTimeout)
            {
                lastTimeRangeChecked = gameTime.TotalGameTime;
                Vector2 center = Pivot.PositionInQuarter;
                Vector2[] corners = new Vector2[] 
                {
                    UpperLeftCorner,
                    UpperRightCorner,
                    LowerLeftCorner,
                    LowerRightCorner
                };
                for (int i = 0; i < corners.Length; i++ )
                {
                    Vector2 movement = Vector2.Normalize(corners[i] - center) * actionDistance;
                    corners[i] += movement;
                }
                Quadrangle range = new Quadrangle(corners[0], corners[1], corners[2], corners[3]);
                IEnumerable<Quadrangle> collisions = Position.Quarter.SpaceGrid.GetAllCollisions(range);
                HashSet<Human> newHumans = new HashSet<Human>();
                foreach (Quadrangle collision in collisions)
                {
                    if (collision is Human)
                    {
                        Human human = ((Human)collisions);
                        if (!availibleHumans.Contains(human))
                        {
                            human.RegisterAvailibleAction(this);
                        }
                        newHumans.Add(human);
                    }
                }
                availibleHumans.ExceptWith(newHumans);
                foreach (Human oldHuman in availibleHumans)
                {
                    oldHuman.UnregisterAvailibleAction(this);
                }
                availibleHumans = newHumans;
            }
        }
    }
}
