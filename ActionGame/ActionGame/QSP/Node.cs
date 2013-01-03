using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Space;

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
    }
}
