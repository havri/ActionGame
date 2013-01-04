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

        public GridField()
        {
            objects = new HashSet<Quadrangle>();
            node = null;
        }

        public void AddObject(Quadrangle obj)
        {
            objects.Add(obj);
        }

        public void SetNode(LeafNode node)
        {
            this.node = node;
        }

        public ISet<Quadrangle> Objects
        {
            get { return new HashSet<Quadrangle>(objects); }
        }

        public int Count
        {
            get { return objects.Count; }
        }
    }
}
