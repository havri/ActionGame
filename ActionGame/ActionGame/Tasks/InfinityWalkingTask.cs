using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.World;
using Microsoft.Xna.Framework;

namespace ActionGame.Tasks
{
    /// <summary>
    /// Task for infinity walking - used by walkers and guards. It is formed by set of move-tasks which are repeated in a infinite loop.
    /// </summary>
    public class InfinityWalkingTask : Task
    {
        Queue<MoveTask> moveTasks;

        /// <summary>
        /// Creates new infinity walking task.
        /// </summary>
        /// <param name="holder">The holder of this task</param>
        /// <param name="positionCyrcle">Set of positions the inner move-tasks will lead toward</param>
        public InfinityWalkingTask(Human holder, IEnumerable<PositionInTown> positionCyrcle)
            : base(holder)
        {
            moveTasks = new Queue<MoveTask>();
            foreach (PositionInTown pos in positionCyrcle)
            {
                MoveTask mt = new MoveTask(holder, pos);
                moveTasks.Enqueue(mt);
            }
        }

        /// <summary>
        /// Update task logic - leads the human according the current inner move-task.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            MoveTask moveTask = moveTasks.Peek();

            moveTask.Update(gameTime);

            if (moveTask.IsComplete())
            {
                moveTasks.Dequeue();
                moveTasks.Enqueue(moveTask);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Says nothing. The infinity walking task is never complete.
        /// </summary>
        /// <returns>False</returns>
        public override bool IsComplete()
        {
            return false;
        }


        /// <summary>
        /// Gets the current target quarter.
        /// </summary>
        public override TownQuarter TargetQuarter
        {
            get { return moveTasks.Peek().TargetQuarter; }
        }
    }
}
