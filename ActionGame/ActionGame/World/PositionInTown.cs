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

        /// <summary>
        /// Computes euclid heuristic distance between two locations. If they are in the same quarter, returns true value in meters. Otherwise returns minimal value needed for way through quarter interfaces.
        /// </summary>
        /// <param name="otherPosition">Destination location</param>
        /// <returns>Distance in meters.</returns>
        public float DistanceTo(PositionInTown otherPosition)
        {
            if (quarter == otherPosition.quarter)
            {
                return (positionInQuarter - otherPosition.positionInQuarter).Length();
            }
            else
            {
                ///TODO: This is used as heuristic in A* graph search. Set this not breaking validity and monotone. Or verify this establishment.

                return Math.Min(
                        Math.Min(positionInQuarter.X, positionInQuarter.Y),
                        Math.Min(quarter.QuarterSize.X - positionInQuarter.X, quarter.QuarterSize.X - positionInQuarter.Y)
                    )
                    +
                    Math.Min(
                        Math.Min(otherPosition.positionInQuarter.X, otherPosition.positionInQuarter.Y),
                        Math.Min(otherPosition.quarter.QuarterSize.X - otherPosition.positionInQuarter.X, otherPosition.quarter.QuarterSize.X - otherPosition.positionInQuarter.Y)
                    );
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
