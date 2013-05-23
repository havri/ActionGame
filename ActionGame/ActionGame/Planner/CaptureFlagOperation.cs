using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionGame.Planner
{
    /// <summary>
    /// The capturing a quarter flag planning operation.
    /// </summary>
    class CaptureFlagOperation : ActionOperation
    {
        readonly int quarterIndex;

        public CaptureFlagOperation(ActionGame game, int quarterIndex)
            : base(game, game.Town.Quarters[quarterIndex].Flag)
        {
            this.quarterIndex = quarterIndex;
        }

        public override GameState Operate(GameState currentState)
        {
            GameState newState = base.Operate(currentState);
            newState.QuarterStates[quarterIndex].OwnershipDuration = TimeSpan.Zero;
            newState.QuarterStates[quarterIndex].Ownership = QuarterOwnership.My;
            return newState;
        }
    }
}
