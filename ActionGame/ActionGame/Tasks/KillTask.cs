using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using Microsoft.Xna.Framework;

namespace ActionGame.Tasks
{
    public class KillTask : Task
    {
        readonly Human target;
        public KillTask(Human holder, Human target)
            : base(holder)
        {
            this.target = target; 
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ///TODO: Shut if you see the target.
        }

        public override bool IsComplete()
        {
            return target.Health <= 0;
        }
    }
}
