using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.World;

namespace ActionGame.Tasks
{
    public class WayPoint
    {
        PositionInTown point;
        public PositionInTown Point
        {
            get
            {
                return point;
            }
            set
            {
                point = value;
            }
        }
        public WayPoint(PositionInTown point)
        {
            this.point = point;
        }
    }
}
