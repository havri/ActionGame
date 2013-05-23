using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ActionGame.World;
using Microsoft.Xna.Framework.Graphics;
using ActionGame.People;
using ActionGame.Components;

namespace ActionGame.Objects
{
    /// <summary>
    /// Town quarter flag action object. It can be captured. This is its action.
    /// </summary>
    public class Flag : ActionObject
    {
        const float ActionDistance = 1.5f;
        static readonly TimeSpan TakeTheFlagTimeout = new TimeSpan(0, 0, 5);

        Human taker = null;
        TimeSpan takeBeginTime = TimeSpan.Zero;
        ProgressBar drawedProgressBar;

        /// <summary>
        /// Creates a new flag action object
        /// </summary>
        /// <param name="game">The game</param>
        /// <param name="model">Model</param>
        /// <param name="position">Position</param>
        /// <param name="azimuth">Azimuth</param>
        /// <param name="worldTransform">World transform matrix</param>
        public Flag(ActionGame game, Model model, PositionInTown position, double azimuth, Matrix worldTransform)
            : base(game, ActionDistance, model, position, azimuth, worldTransform)
        {
            
        }

        /// <summary>
        /// Gets the whole action duration.
        /// </summary>
        public override TimeSpan ActionDuration
        {
            get { return TakeTheFlagTimeout; }
        }

        /// <summary>
        /// Update its logic. This handles the proper capturing.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ActionRunning)
            {
                float progress = (float)(gameTime.TotalGameTime - takeBeginTime).TotalMilliseconds / (float)TakeTheFlagTimeout.TotalMilliseconds;
                drawedProgressBar.Value = progress;

                if (taker != null && !IsAvailableFor(taker))
                {
                    EndAction(taker, gameTime);
                }
            }
        }

        public override void StartAction(Human actor, GameTime gameTime)
        {
            if (!ActionRunning)
            {  
                if (Position.Quarter.Owner != actor)
                {
                    base.StartAction(actor, gameTime);
                    if (taker != actor)
                    {
                        taker = actor;
                        takeBeginTime = gameTime.TotalGameTime;
                        drawedProgressBar = Game.Drawer.CreateProgressBar(actor.Content.ColorTexture);
                    }
                }
            }
        }

        public override void EndAction(Human actor, GameTime gameTime)
        {
            if (ActionRunning && actor == taker)
            {
                base.EndAction(actor, gameTime);

                if (gameTime.TotalGameTime - takeBeginTime > TakeTheFlagTimeout)
                {
                    Position.Quarter.SetOwner(actor, gameTime);
                    if (actor is Player)
                    {
                        Game.Drawer.ShowMessage(gameTime, String.Format("Congratulations! You've captured {0}.", Position.Quarter.Name));
                    }
                    else
                    {
                        Game.Drawer.ShowMessage(gameTime, String.Format("Warning! Your enemy has captured {0}.", Position.Quarter.Name));
                    }
                }
                taker = null;
                Game.Drawer.DestroyProgressBar(drawedProgressBar);
                drawedProgressBar = null;
            }
        }
    }
}
