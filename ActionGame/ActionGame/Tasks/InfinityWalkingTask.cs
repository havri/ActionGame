using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.World;
using Microsoft.Xna.Framework;

namespace ActionGame.Tasks
{
    public class InfinityWalkingTask : Task
    {
        Queue<MoveTask> moveTasks;

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

        public override bool IsComplete()
        {
            return false;
        }
    }
}
