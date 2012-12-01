using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;

namespace ActionGame.Tasks
{
    public class KillTask : Task
    {
        Human target;
        public KillTask(Human holder)
            : base(holder)
        {
            
        }

        public override void Update()
        {
            base.Update();

            ///TODO: Shut if you see the target.
        }

        public override bool IsComplete()
        {
            return target.Health <= 0;
        }
    }
}
