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
using ActionGame.QSP;
using Microsoft.Xna.Framework.Audio;

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

                IEnumerable<GridField> affectedFields = quarter.SpaceGrid.GetFieldsByObject(bullet);
                List<Quadrangle> colliders = new List<Quadrangle>();
                foreach (GridField field in affectedFields)
                {
                    colliders.AddRange(field.GetCollisions(bullet));
                }
                colliders.RemoveAll(x => x == Holder);

                //Half-interval search
                Stack<RangeF> testedParts = new Stack<RangeF>();
                testedParts.Push(new RangeF(0, type.Range));

                Quadrangle nearest = null;
                float distance = type.Range;
                while (testedParts.Count != 0)
                {
                    RangeF bulletRangePart = testedParts.Pop();
                    Quadrangle bulletPart = new Quadrangle(
                        left.Go(bulletRangePart.Begin, azimuth),
                        right.Go(bulletRangePart.Begin, azimuth),
                        left.Go(bulletRangePart.End, azimuth),
                        right.Go(bulletRangePart.End, azimuth));
                    List<Quadrangle> newColliders = new List<Quadrangle>( colliders.Where(x => x.IsInCollisionWith(bulletPart)) );
                    if (newColliders.Count != 0)
                    {
                        if (newColliders.Count == 1 || bulletRangePart.Length <= bulletWidthHalf)
                        {
                            nearest = newColliders[0];
                            distance = bulletRangePart.End;
                            break;
                        }
                        colliders = newColliders;
                        testedParts.Push(new RangeF(bulletRangePart.Begin + bulletRangePart.Length * 0.5f, bulletRangePart.End));
                        testedParts.Push(new RangeF(bulletRangePart.Begin, bulletRangePart.Begin + bulletRangePart.Length * 0.5f));
                    }
                }

                if (nearest != null)
                {
                    Debug.Write("Shot", nearest.ToString());
                    nearest.BecomeShot(type.Damage);
                }
                quarter.AddBullet(gameTime, new BulletVisualisation(quarter, quarterPosition, distance, azimuth));
                type.ShotSount.Play();
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
