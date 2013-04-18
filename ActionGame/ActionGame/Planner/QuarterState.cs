using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionGame.Planner
{
    struct QuarterState
    {
        public QuarterOwnership Ownership{get;set;}
        public TimeSpan OwnershipDuration{get;set;}
    }
}
