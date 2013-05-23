using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.Tasks;
using ActionGame.Tools;
using ActionGame.World;

namespace ActionGame.Planner
{
    /// <summary>
    /// The take box inside a quarter planning operation.
    /// </summary>
    class TakeBoxOperation : Operation
    {
        readonly Box box;

        public TakeBoxOperation(ActionGame game, Box box)
            : base(game)
        {
            this.box = box;
        }

        public override Task CreateTask(Human taskHolder)
        {
            return new MoveTask(taskHolder, box.Position);
        }

        public override GameState Operate(GameState currentState)
        {
            IEnumerable<PathGraphVertex> path = PathGraph.FindShortestPath(currentState.Position, box.Position, false);
            float length = PathGraph.GetLengthOfPath(path);
            GameState newState = currentState.Copy();
            float duration = length / Human.RunSpeed;
            newState.AddTime(TimeSpan.FromSeconds(duration));
            if (box is ToolBox)
            {
                GunType gunType = Game.BoxDefaultGuns[Game.Random.Next(Game.BoxDefaultGuns.Count)];
                newState.Damage += gunType.Damage * gunType.DefaultBulletCount;
            }
            else
            {
                newState.Health = 100;
            }
            newState.Position = box.Position;
            return newState;
        }
    }
}
