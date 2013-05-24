using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ActionGame.World
{
    /// <summary>
    /// Structure for describing the position inside the town. It contains the town quarter reference and position inside the qurter.
    /// </summary>
    public struct PositionInTown
    {
        private Vector2 positionInQuarter;
        private TownQuarter quarter;

        /// <summary>
        /// Creates a new position in the town
        /// </summary>
        /// <param name="quarter">The quarter</param>
        /// <param name="position">Position inside the quarter</param>
        public PositionInTown(TownQuarter quarter, Vector2 position)
        {
            this.positionInQuarter = position;
            this.quarter = quarter;
        }

        /// <summary>
        /// Computes euclid heuristic distance between two locations. If they are in the same quarter, returns true value in meters. Otherwise returns minimal value taht is neede needed for way through quarter interfaces.
        /// </summary>
        /// <param name="otherPosition">Destination location</param>
        /// <returns>Distance in meters.</returns>
        public float MinimalDistanceTo(PositionInTown otherPosition)
        {
            if (quarter == otherPosition.quarter)
            {
                return (positionInQuarter - otherPosition.positionInQuarter).Length();
            }
            else
            {
                return TownQuarter.SquareWidth;
            }
        }
        /// <summary>
        /// Gets the position inside the quarter
        /// </summary>
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
        /// <summary>
        /// Gets the quarter.
        /// </summary>
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

        public override string ToString()
        {
            return String.Format("{0} {1}", Quarter.Name, positionInQuarter);
        }
    }
}
