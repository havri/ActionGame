using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ActionGame.World
{
    public struct PositionInTown
    {
        private Vector2 positionInQuarter;
        private TownQuarter quarter;

        public PositionInTown(TownQuarter quarter, Vector2 position)
        {
            this.positionInQuarter = position;
            this.quarter = quarter;
        }
        public float DistanceTo(PositionInTown otherPosition)
        {
            if (quarter == otherPosition.quarter)
            {
                return (positionInQuarter - otherPosition.positionInQuarter).Length();
            }
            else
            {
                return float.MaxValue;
            }
        }
        public Vector2 PositionInQuarter
        {
            get
            {
                return positionInQuarter;
            }
            set
            {
                positionInQuarter = value;
            }
        }
        public TownQuarter Quarter
        {
            get
            {
                return quarter;
            }
            set
            {
                quarter = value;
            }
        }
    }
}
