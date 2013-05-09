using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ActionGame.Extensions;
using Microsoft.Xna.Framework.Graphics;
using ActionGame.World;

namespace ActionGame.Space
{
    public class BulletVisualisation : IDrawableObject, IDisposable
    {
        const float Width = 0.005f;
        public static readonly TimeSpan ShowTimeSpan = new TimeSpan(0, 0, 0, 0, 100);
        public static Texture2D Texture;

        readonly Plate horizontalPlate;
        readonly Plate verticalPlate;

        public BulletVisualisation(TownQuarter quarter, Vector2 start, float azimuth, float range, float startHeight, float endHeight)
        {
            Vector2 end = start.Go(range, azimuth);
            Vector2 startL = start.Go(Width, azimuth - MathHelper.PiOver2);
            Vector2 startR = start.Go(Width, azimuth + MathHelper.PiOver2);
            Vector2 endL = end.Go(Width, azimuth - MathHelper.PiOver2);
            Vector2 endR = end.Go(Width, azimuth + MathHelper.PiOver2);
            horizontalPlate = new Plate(quarter, startL.ToVector3(startHeight), startR.ToVector3(startHeight), endL.ToVector3(endHeight), endR.ToVector3(endHeight), Texture, Texture, false);
            verticalPlate = new Plate(quarter, start.ToVector3(startHeight + Width), start.ToVector3(startHeight - Width), end.ToVector3(endHeight + Width), end.ToVector3(endHeight - Width), Texture, Texture, false);
        }

        void IDrawableObject.Draw(Matrix view, Matrix projection, Matrix world)
        {
            horizontalPlate.Draw(view, projection, world);
            verticalPlate.Draw(view, projection, world);
        }

        public void Dispose()
        {
            horizontalPlate.Dispose();
            verticalPlate.Dispose();
        }
    }
}
