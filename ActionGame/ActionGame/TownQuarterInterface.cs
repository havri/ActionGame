using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionGame
{
    class TownQuarterInterface
    {
        /// <summary>
        /// Specifies side of rectangle where is interface located.
        /// </summary>
        public TownQuarterInterfacePosition SidePosition { get; set; }

        /// <summary>
        /// Position of interface on the side of quarter rectangle. In quarter bitmap points.
        /// </summary>
        public int BitmapPosition { get; set; }

        /// <summary>
        /// Querter where this interface belongs.
        /// </summary>
        public TownQuarter Quarter { get; set; }

        /// <summary>
        /// Interface connected
        /// </summary>
        public TownQuarterInterface OppositeInterface { get; set; }
    }
}
