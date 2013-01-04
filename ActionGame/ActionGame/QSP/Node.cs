using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Space;
using Microsoft.Xna.Framework;

namespace ActionGame.QSP
{
    abstract class Node
    {
        protected readonly Grid grid;

        public Node(Grid grid)
        {
            this.grid = grid;
        }

        public abstract ISet<Quadrangle> Objects();
        public abstract int Count();
        public abstract LeafNode Join();
        public abstract Rectangle GridSpace
        {
            get;
        }
    }
}
