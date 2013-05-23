using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using Microsoft.Xna.Framework;

namespace ActionGame.Tasks
{
    /// <summary>
    /// Task container for only temporary objectives.
    /// </summary>
    /// <typeparam name="InnerTask">The inner task type</typeparam>
    class TemporaryTask<InnerTask> : Task where InnerTask : Task
    {
        readonly InnerTask innerTask;
        readonly Predicate<InnerTask> validPredicate;
        /// <summary>
        /// Creates a new temporary task.
        /// </summary>
        /// <param name="holder">The task holder</param>
        /// <param name="innerTask">The inner task that should be accomplished</param>
        /// <param name="validPredicate">Predicate that says until when the inner task should be solved</param>
        public TemporaryTask(Human holder, InnerTask innerTask, Predicate<InnerTask> validPredicate)
            :base(holder)
        {
            this.innerTask = innerTask;
            this.validPredicate = validPredicate;
        }

        /// <summary>
        /// Updates the task logic - uses the inner task logic.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Update(GameTime gameTime)
        {
            innerTask.Update(gameTime);
        }

        /// <summary>
        /// Says whether the task was accomplished or whether it is valid.
        /// </summary>
        /// <returns>True if the inner task was accomplished or if the valid predicate is not satsified</returns>
        public override bool IsComplete()
        {
            return innerTask.IsComplete() || !validPredicate(innerTask);
        }
        /// <summary>
        /// Gets the target quarter of the inner task.
        /// </summary>
        public override World.TownQuarter TargetQuarter
        {
            get { return innerTask.TargetQuarter; }
        }
    }
}
