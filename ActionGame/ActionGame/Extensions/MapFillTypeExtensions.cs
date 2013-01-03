using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionGame.Extensions
{
    public static class MapFillTypeExtensions
    {
        public static MapFillType Index2D(this MapFillType[] mapBitmap, int mapHeight, int x, int y)
        {
            return mapBitmap[x * mapHeight + y];
        }
    }
}
