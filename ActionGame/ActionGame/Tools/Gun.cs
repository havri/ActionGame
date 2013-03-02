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
        private int bullets;

        public Gun(GunType type, int bullets)
            : this(type, bullets, null)
        { }
        public Gun(GunType type, int bullets, Human handler)
            : base (type.Icon, handler)
        {
            this.type = type;
            this.bullets = bullets;
        }

        public void Load(int bullets)
        {
            this.bullets += bullets;
        }

        public int Bullets
        {
            get { return bullets; }
        }

        public override void DoAction()
        {
            throw new NotImplementedException();
        }

        public GunType Type
        {
            get
            {
                return type;
            }
        }
    }
}
