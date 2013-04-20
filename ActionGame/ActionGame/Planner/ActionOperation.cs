using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Objects;
using ActionGame.People;
using ActionGame.Tasks;
using ActionGame.World;

namespace ActionGame.Planner
{
    abstract class ActionOperation : Operation
    {
        readonly ActionObject actionObject;

        public ActionOperation(ActionGame game, ActionObject actionObject)
            : base(game)
        {
            this.actionObject = actionObject;
        }

        public override Task CreateTask(Human taskHolder)
        {
            return new ActionObjectTask(actionObject, taskHolder);
        }

        public override GameState Operate(GameState currentState)
        {
            IEnumerable<PathGraphVertex> path = PathGraph.FindShortestPath(currentState.Position, actionObject.Position);
            float length = PathGraph.GetLengthOfPath(path);
            GameState newState = currentState.Copy();
            float duration = length / Human.RunSpeed;
            newState.AddTime(TimeSpan.FromSeconds(duration));
            newState.Position = actionObject.Position;
            return newState;
        }
    }
}
