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
    public class Flag : ActionObject
    {
        const float ActionDistance = 1f;
        static readonly TimeSpan TakeTheFlagTimeout = new TimeSpan(0, 0, 5);

        Human taker = null;
        TimeSpan takeBeginTime = TimeSpan.Zero;
        ProgressBar drawedProgressBar;

        public Flag(ActionGame game, Model model, PositionInTown position, double azimuth, Matrix worldTransform)
            : base(game, ActionDistance, model, position, azimuth, worldTransform)
        {
            
        }

        public override TimeSpan ActionDuration
        {
            get { return TakeTheFlagTimeout; }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ActionRunning)
            {
                float progress = (float)(gameTime.TotalGameTime - takeBeginTime).TotalMilliseconds / (float)TakeTheFlagTimeout.TotalMilliseconds;
                drawedProgressBar.Value = progress;
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
                Game.Drawer.DestroyProgressBar(drawedProgressBar);
                drawedProgressBar = null;
            }
        }
    }
}
