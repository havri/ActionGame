using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;

namespace ActionGame.World
{
    /// <summary>
    /// Interface for quarter owning person (player, opponent, empty)
    /// </summary>
    public interface ITownQuarterOwner
    {
        /// <summary>
        /// Gets the ownner's content - specific flag, guarg model, color...
        /// </summary>
        TownQuarterOwnerContent Content { get; }
        /// <summary>
        /// Creates a new ally guard for the owned quarter.
        /// </summary>
        /// <param name="targetQuarter">The destination quarter for the guard</param>
        /// <returns>New guard</returns>
        Human CreateAllyGuard(TownQuarter targetQuarter);
        /// <summary>
        /// Gets timeout between two guards are created in the owned quarter.
        /// </summary>
        TimeSpan GuardAddTimeout { get; }
        /// <summary>
        /// Gets the full health percentage of new guards.
        /// </summary>
        int GuardFullHealth { get; }
    }
}
