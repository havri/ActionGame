using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Tools
{
    public class Gun : Tool
    {
        private readonly GunType type;

        public Gun(GunType type)
            : this(type, null)
        { }
        public Gun(GunType type, Human handler)
            : base (type.Icon, handler)
        {
            this.type = type;
        }

        public override void DoAction()
        {
            throw new NotImplementedException();
        }
    }
}
