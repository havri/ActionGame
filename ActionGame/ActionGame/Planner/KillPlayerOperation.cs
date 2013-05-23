using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.Tasks;
using ActionGame.World;

namespace ActionGame.Planner
{
    /// <summary>
    /// The killing the player planning operation.
    /// </summary>
    class KillPlayerOperation : Operation
    {
        public KillPlayerOperation(ActionGame game)
            : base(game)
        {
            
        }


        public override Task CreateTask(Human taskHolder)
        {
            return new KillTask(taskHolder, Game.Player);
        }

        public override GameState Operate(GameState currentState)
        {
            GameState newState = currentState.Copy();
            IEnumerable<PathGraphVertex> path = PathGraph.FindShortestPath(currentState.Position, Game.Player.Position, false);
            float length = PathGraph.GetLengthOfPath(path);
            float duration = length / Human.RunSpeed;
            newState.AddTime(TimeSpan.FromSeconds(duration));
            newState.Position = Game.Player.Position;
            newState.Damage -= Game.Player.Health;
            newState.Health -= Game.Player.Health * 0.9f;
            bool one = false;
            for (int i = 0; i < newState.QuarterStates.Length; i++)
            {
                if (newState.QuarterStates[i].Ownership == QuarterOwnership.His)
                {
                    if(one)
                    {
                        newState.QuarterStates[i].Ownership = QuarterOwnership.Empty;
                        newState.QuarterStates[i].OwnershipDuration = TimeSpan.Zero;
                    }
                    one = true;
                }
            }
            return newState;
        }
    }
}
