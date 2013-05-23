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
    /// Abstraction for taskt planning operation. It provides concrete steps between the game states.
    /// </summary>
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

        /// <summary>
        /// Creates a new operation.
        /// </summary>
        /// <param name="game">The game</param>
        public Operation(ActionGame game)
        {
            this.game = game;
        }

        /// <summary>
        /// Creates the coresponding task for this operation.
        /// </summary>
        /// <param name="taskHolder">Holder for the result task</param>
        /// <returns>The result task that is providing this operation in the game</returns>
        public abstract Task CreateTask(Human taskHolder);

        /// <summary>
        /// Performs a step between given game state to a new game state.
        /// </summary>
        /// <param name="currentState">The current game state</param>
        /// <returns>The state of the game after this operation is done</returns>
        public abstract GameState Operate(GameState currentState);
    }
}
