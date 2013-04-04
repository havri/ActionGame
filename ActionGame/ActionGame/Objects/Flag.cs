using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ActionGame.World;
using Microsoft.Xna.Framework.Graphics;
using ActionGame.People;

namespace ActionGame.Objects
{
    public class Flag : ActionObject
    {
        const float ActionDistance = 1f;
        static readonly TimeSpan TakeTheFlagTimeout = new TimeSpan(0, 0, 5);

        Human taker = null;
        TimeSpan takeBeginTime = TimeSpan.Zero;

        public Flag(ActionGame game, Model model, PositionInTown position, double azimuth, Matrix worldTransform)
            : base(game, ActionDistance, model, position, azimuth, worldTransform)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ActionRunning)
            {
                float progress = (float)(gameTime.TotalGameTime - takeBeginTime).TotalMilliseconds / (float)TakeTheFlagTimeout.TotalMilliseconds;
                Game.Drawer.ProgressBarValue = progress;
            }
        }

        public override void StartAction(Human actor, GameTime gameTime)
        {
            if (!ActionRunning)
            {
                base.StartAction(actor, gameTime);

                if (Position.Quarter.Owner != actor)
                {
                    if (taker != actor)
                    {
                        taker = actor;
                        takeBeginTime = gameTime.TotalGameTime;
                        Game.Drawer.DrawProgressBar = true;
                        Game.Drawer.ProgressBarValue = 0f;
                        Game.Drawer.ProgressBarTexture = actor.Content.ColorTexture;
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
                    Position.Quarter.SetOwner(actor);
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
                Game.Drawer.DrawProgressBar = false;
            }
        }
    }
}
