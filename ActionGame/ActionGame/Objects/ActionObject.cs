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
    /// <summary>
    /// The action object abstraction. Object in the game that can do some actions if a human wants to.
    /// </summary>
    public abstract class ActionObject : SpatialObject
    {
        static readonly TimeSpan CheckRangeTimeout = new TimeSpan(0, 0, 0, 0, 250);

        readonly float actionDistance;
        TimeSpan lastTimeRangeChecked = TimeSpan.Zero;
        HashSet<Human> availibleHumans = new HashSet<Human>();
        /// <summary>
        /// Gets the indicator whether the action is running now.
        /// </summary>
        public bool ActionRunning { get { return actionRunning; } }
        bool actionRunning = false;
        protected ActionGame Game { get { return game; } } 
        readonly ActionGame game;

        /// <summary>
        /// Creates new action object.
        /// </summary>
        /// <param name="game">The game</param>
        /// <param name="actionDistance">Distance from which the action can be performed</param>
        /// <param name="model">Model of the action object</param>
        /// <param name="position">Position</param>
        /// <param name="azimuth">Azimuth</param>
        /// <param name="worldTransform">World transform matrix</param>
        public ActionObject(ActionGame game, float actionDistance, Model model, PositionInTown position, double azimuth, Matrix worldTransform)
            : base(model, position, azimuth, worldTransform)
        {
            this.actionDistance = actionDistance;
            this.game = game;
        }

        /// <summary>
        /// Gets the duration of the whole action.
        /// </summary>
        public abstract TimeSpan ActionDuration { get; }

        /// <summary>
        /// Updates the object logic. Checks availability for humans and let them know about it.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public virtual void Update(GameTime gameTime)
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
                        Human human = ((Human)collision);
                        if (!availibleHumans.Contains(human))
                        {
                            human.RegisterAvailableAction(this);
                        }
                        newHumans.Add(human);
                    }
                }
                availibleHumans.ExceptWith(newHumans);
                foreach (Human oldHuman in availibleHumans)
                {
                    oldHuman.UnregisterAvailableAction(this);
                }
                availibleHumans = newHumans;
            }
        }

        /// <summary>
        /// Says whether the specified human can perform the action right now.
        /// </summary>
        /// <param name="human"></param>
        /// <returns></returns>
        public bool IsAvailableFor(Human human)
        { 
            return availibleHumans.Contains(human);
        }

        /// <summary>
        /// Starts the action.
        /// </summary>
        /// <param name="actor">The action performer</param>
        /// <param name="gameTime">Game time</param>
        public virtual void StartAction(Human actor, GameTime gameTime)
        {
            actionRunning = true;
        }

        /// <summary>
        /// End the performing of the action.
        /// </summary>
        /// <param name="actor">The action performer</param>
        /// <param name="gameTime">Game time</param>
        public virtual void EndAction(Human actor, GameTime gameTime)
        {
            actionRunning = false;
        }
    }
}
