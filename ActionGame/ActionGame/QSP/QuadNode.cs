using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Space;
using Microsoft.Xna.Framework;

namespace ActionGame.QSP
{
    class QuadNode : Node
    {
        Node upperLeft, upperRight, lowerLeft, lowerRight;

        public QuadNode(Grid grid, Node upperLeft, Node upperRight, Node lowerLeft, Node lowerRight)
            : base(grid)
        {
            this.upperLeft = upperLeft;
            this.upperRight = upperRight;
            this.lowerLeft = lowerLeft;
            this.lowerRight = lowerRight;
        }

        public override ISet<Quadrangle> Objects()
        {
            ISet<Quadrangle> result = upperLeft.Objects();
            result.UnionWith(upperRight.Objects());
            result.UnionWith(lowerLeft.Objects());
            result.UnionWith(lowerRight.Objects());
            return result;
        }

        public override int Count()
        {
            int result = upperLeft.Count()
                + upperRight.Count()
                + lowerLeft.Count()
                + lowerRight.Count();
            return result;
        }

        public override Rectangle GridSpace
        {
            get
            {
                return new Rectangle(upperLeft.GridSpace.X, upperLeft.GridSpace.Y, upperLeft.GridSpace.Width + upperRight.GridSpace.Width, upperLeft.GridSpace.Height + lowerLeft.GridSpace.Height);
            }
        }

        public override LeafNode Join()
        {
            return new LeafNode(grid, GridSpace);
        }
    }
}
