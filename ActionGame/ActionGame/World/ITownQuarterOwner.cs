using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;

namespace ActionGame.World
{
    public interface ITownQuarterOwner
    {
        TownQuarterOwnerContent Content { get; }
        Human CreateAllyGuard(TownQuarter targetQuarter);
        TimeSpan GuardAddTimeout { get; }
        int GuardFullHealth { get; }
    }
}
