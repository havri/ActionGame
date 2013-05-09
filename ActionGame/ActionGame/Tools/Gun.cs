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
            if (gameTime.TotalGameTime - lastTimeShot >= type.ShotTimeout && Usable)
            {
                const float bulletWidth = 0.02f;
                //const float startHeight = 0.5f;
                const float bulletWidthHalf = bulletWidth / 2;

                bool solveHeight = (Holder is Player);
                double lookAngle = Holder.LookAngle;
                //Vector2 quarterPosition = position.PositionInQuarter;
                float startHeight = Holder.FirstHeadPosition.Y - bulletWidth;
                Vector2 quarterPosition = Holder.FirstHeadPosition.XZToVector2();
                Vector2 left = quarterPosition.Go(bulletWidthHalf, azimuth - MathHelper.PiOver2);
                Vector2 right = quarterPosition.Go(bulletWidthHalf, azimuth + MathHelper.PiOver2);
                Quadrangle bullet = new Quadrangle(right, left, right.Go(type.Range, azimuth), left.Go(type.Range, azimuth));
                TownQuarter quarter = position.Quarter;

                List<Quadrangle> colliders = new List<Quadrangle>(quarter.SpaceGrid.GetAllCollisions(bullet));
                colliders.RemoveAll(x => x == Holder);

                //Half-interval search
                Stack<RangeF> testedParts = new Stack<RangeF>();
                Dictionary<float, float> heights = new Dictionary<float, float>();
                testedParts.Push(new RangeF(0, type.Range));
                if(solveHeight)
                {
                    heights.Add(0, startHeight);
                    heights.Add(type.Range, startHeight + type.Range * (float)Math.Tan(lookAngle));
                }

                Quadrangle nearest = null;
                float distance = type.Range;
                while (testedParts.Count != 0)
                {
                    RangeF bulletRangePart = testedParts.Pop();
                    float h1 = 0f;
                    float h2 = 0f;
                    if (solveHeight)
                    {
                        h1 = heights[bulletRangePart.Begin];
                        h2 = heights[bulletRangePart.End];
                    }
                    Quadrangle bulletPart = new Quadrangle(
                        left.Go(bulletRangePart.Begin, azimuth),
                        right.Go(bulletRangePart.Begin, azimuth),
                        left.Go(bulletRangePart.End, azimuth),
                        right.Go(bulletRangePart.End, azimuth));
                    List<Quadrangle> newColliders = new List<Quadrangle>(
                        colliders.Where(
                        x =>
                            ( solveHeight && x is SpatialObject && (((SpatialObject)x).Size.Y >= h2 || ((SpatialObject)x).Size.Y >= h2) && (0 <= h1 || 0 <= h2) && x.IsInCollisionWith(bulletPart) )
                            ||
                            (!solveHeight && x.IsInCollisionWith(bulletPart))
                            ) 
                        );
                    if (newColliders.Count != 0)
                    {
                        if (newColliders.Count == 1 || bulletRangePart.Length <= bulletWidthHalf)
                        {
                            nearest = newColliders[0];
                            distance = bulletRangePart.End;
                            break;
                        }
                        colliders = newColliders;
                        float halfLength = bulletRangePart.Length * 0.5f;
                        float middle = bulletRangePart.Begin + halfLength;
                        testedParts.Push(new RangeF(middle, bulletRangePart.End));
                        testedParts.Push(new RangeF(bulletRangePart.Begin, middle));
                        if (solveHeight && halfLength != 0f)
                        {
                            heights.Add(middle, startHeight + middle * (float)Math.Tan(lookAngle));
                        }
                    }
                }

                if (nearest != null)
                {
                    Debug.Write("Shot", nearest.ToString());
                    nearest.BecomeShot(type.Damage, Holder);
                }
                quarter.AddBullet(gameTime, new BulletVisualisation(quarter, quarterPosition, azimuth, distance, startHeight, startHeight + distance * (float)Math.Tan(lookAngle)));
                type.ShotSount.Play();
                lastTimeShot = gameTime.TotalGameTime;
                bullets--;
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

        public override bool Usable
        {
            get
            {
                return Type.InfinityBullets || bullets > 0;
            }
        }
    }
}
