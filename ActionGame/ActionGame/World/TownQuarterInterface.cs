﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ActionGame.World
{
    public class TownQuarterInterface
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
        /// Querter where is this interface located.
        /// </summary>
        public TownQuarter Quarter { get; set; }

        /// <summary>
        /// Interface connected
        /// </summary>
        public TownQuarterInterface OppositeInterface { get; set; }
        public PathGraphVertex LeftPathGraphVertex { get; set; }
        public PathGraphVertex RightPathGraphVertex { get; set; }

        /// <summary>
        /// Gets the real position of the interface in the quarter.
        /// </summary>
        /// <returns>The real position in the quarter</returns>
        public Vector2 Position()
        {
            Vector2 ret = Vector2.Zero;
            float sideLength = BitmapPosition * TownQuarter.SquareWidth + TownQuarter.SquareWidth/2f;
            switch (SidePosition)
            {
                case TownQuarterInterfacePosition.Top:
                    ret.X = sideLength;
                    ret.Y = 0;
                    break;
                case TownQuarterInterfacePosition.Right:
                    ret.X = Quarter.BitmapSize.Width * TownQuarter.SquareWidth;
                    ret.Y = sideLength;
                    break;
                case TownQuarterInterfacePosition.Bottom:
                    ret.X = sideLength;
                    ret.Y = Quarter.BitmapSize.Height * TownQuarter.SquareWidth;
                    break;
                case TownQuarterInterfacePosition.Left:
                    ret.X = 0;
                    ret.Y = sideLength;
                    break;
            }
            return ret;
        }
    }
}
