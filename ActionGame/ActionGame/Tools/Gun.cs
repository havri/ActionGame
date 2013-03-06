using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using ActionGame.Space;
using ActionGame.World;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Tools
{
    public class Gun : Tool
    {
        private readonly GunType type;
        private int bullets;
        TimeSpan lastTimeShot;

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

        public override void DoAction(GameTime gameTime, PositionInTown position, float azimuth)
        {
            if (gameTime.TotalGameTime - lastTimeShot >= type.ShotTimeout)
            { 
                const float bulletWidthHalf = 0.1f;
                Vector2 quarterPosition = position.PositionInQuarter;
                Vector2 left = quarterPosition.Go(bulletWidthHalf, azimuth - MathHelper.PiOver2);
                Vector2 right = quarterPosition.Go(bulletWidthHalf, azimuth + MathHelper.PiOver2);
                Quadrangle bullet = new Quadrangle(left, right, left.Go(type.Range, azimuth), right.Go(type.Range, azimuth));
                TownQuarter quarter = position.Quarter;
                quarter.AddBullet(gameTime, new BulletVisualisation(quarter, quarterPosition, type.Range, azimuth));
                lastTimeShot = gameTime.TotalGameTime;
            }
        }

        public GunType Type
        {
            get
            {
                return type;
            }
        }

        public override string ToolBarText
        {
            get { return (type.InfinityBullets ? String.Empty : bullets.ToString()); }
        }
    }
}
