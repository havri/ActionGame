using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Space;
using Microsoft.Xna.Framework;

namespace ActionGame.QSP
{
    class LeafNode : Node
    {
        readonly Rectangle space;

        public LeafNode(Grid grid, Rectangle space)
            : base(grid)
        {
            this.space = space;

            foreach (var field in grid.GetFields(space))
            {
                field.SetNode(this);
            }
        }

        public override ISet<Quadrangle> Objects()
        {
            HashSet<Quadrangle> result = new HashSet<Quadrangle>();
            foreach (GridField field in grid.GetFields(space))
            {
                result.UnionWith(field.Objects);
            }
            return result;
        }

        public override int Count()
        {
            int result = 0;
            foreach (GridField field in grid.GetFields(space))
            {
                result += field.Count;
            }
            return result;
        }

        public Node Split()
        {
            if (space.Width < 2 || space.Height < 2)
            {
                return this;
            }
            else
            {
                Rectangle upperLeft = new Rectangle(space.X, space.Y, space.Width / 2, space.Height / 2);
                Rectangle upperRight = new Rectangle(space.X + upperLeft.Width, space.Y, space.Width - upperLeft.Width, upperLeft.Height);
                Rectangle lowerLeft = new Rectangle(space.X, space.Y + upperLeft.Height, upperLeft.Width, space.Height - upperLeft.Height);
                Rectangle lowerRight = new Rectangle(upperRight.X, lowerLeft.Y, upperRight.Width, lowerLeft.Height);

                return new QuadNode(grid,
                    new LeafNode(grid, upperLeft),
                    new LeafNode(grid, upperRight),
                    new LeafNode(grid, lowerLeft),
                    new LeafNode(grid, lowerRight));
            }
        }

        public override LeafNode Join()
        {
            return this;
        }

        public override Rectangle GridSpace
        {
            get { return space; }
        }
    }
}
