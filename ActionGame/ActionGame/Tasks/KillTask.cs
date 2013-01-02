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
        Human target;
        public KillTask(Human holder)
            : base(holder)
        {
            
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
