using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.Tasks;
using ActionGame.World;

namespace ActionGame.Planner
{
    abstract class Operation
    {
        readonly ActionGame game;
        protected ActionGame Game
        {
            get
            {
                return game;
            }
        }

        public Operation(ActionGame game)
        {
            this.game = game;
        }

        public abstract Task CreateTask(Human taskHolder);

        public abstract GameState Operate(GameState currentState);
        /*{
            float duration = distanceToGo / Human.RunSpeed;
            int quarterIndex = 0;
            foreach (var qs in currentState.QuarterStates)
            {
                switch (qs.Ownership)
                {
                    case QuarterOwnership.My:
                        break;
                        quarterIndex += 1;
                    case QuarterOwnership.Empty:
                        quarterIndex -= 1;
                        break;
                }
            }
            float minDistFromPlayer = game.Player.Position.MinimalDistanceTo(game.Opponent.Position);
            GameState newState = currentState.Copy();
            if (quarterIndex >= 0 && minDistFromPlayer <= Human.RunSpeed * duration)
            {
                newState.Health = 1f;

            }
        }
        */
    }
}
