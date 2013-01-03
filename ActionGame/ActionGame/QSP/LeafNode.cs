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
    }
}
