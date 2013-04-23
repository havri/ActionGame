using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionGame.Extensions
{
    static class RandomExtensions
    {
        /// <summary>
        /// Gets random number from specified interval.
        /// </summary>
        /// <param name="rand">Random generator</param>
        /// <param name="min">Minumun, inclusive</param>
        /// <param name="max">Maximum, inclusive</param>
        /// <returns>Random double</returns>
        public static double NextDouble(this Random rand, double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }
    }
}
