using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Space;

namespace ActionGame.QSP
{
    class GridField
    {
        readonly ISet<Quadrangle> objects;
        LeafNode node;

        public GridField(LeafNode node)
        {
            objects = new HashSet<Quadrangle>();
            this.node = node;
        }

        public void AddObject(Quadrangle obj)
        {
            objects.Add(obj);
        }

        public ISet<Quadrangle> Objects
        {
            get { return objects; }
        }
    }
}
