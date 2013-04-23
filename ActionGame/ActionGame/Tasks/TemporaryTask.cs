using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using Microsoft.Xna.Framework;

namespace ActionGame.Tasks
{
    class TemporaryTask<InnerTask> : Task where InnerTask : Task
    {
        readonly InnerTask innerTask;
        readonly Predicate<InnerTask> validPredicate;

        public TemporaryTask(Human holder, InnerTask innerTask, Predicate<InnerTask> validPredicate)
            :base(holder)
        {
            this.innerTask = innerTask;
            this.validPredicate = validPredicate;
        }

        public override void Update(GameTime gameTime)
        {
            innerTask.Update(gameTime);
        }

        public override bool IsComplete()
        {
            return innerTask.IsComplete() || !validPredicate(innerTask);
        }
    }
}
