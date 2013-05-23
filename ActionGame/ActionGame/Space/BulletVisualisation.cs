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
    /// <summary>
    /// Bullet object. Straight visible line in the space.
    /// </summary>
    public class BulletVisualisation : ITransformedDrawable, IDisposable
    {
        const float Width = 0.005f;
        public static readonly TimeSpan ShowTimeSpan = new TimeSpan(0, 0, 0, 0, 100);
        public static Texture2D Texture;

        readonly Plate horizontalPlate;
        readonly Plate verticalPlate;

        /// <summary>
        /// Creates a new bullet visualisation.
        /// </summary>
        /// <param name="quarter">Town quarter where the bullet is located</param>
        /// <param name="start">Starting position</param>
        /// <param name="azimuth">The flight direction</param>
        /// <param name="range">The flight range</param>
        /// <param name="startHeight">Starting height</param>
        /// <param name="endHeight">Ending height</param>
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

        /// <summary>
        /// Draws this bullet.
        /// </summary>
        /// <param name="view">View transformation matrix</param>
        /// <param name="projection">Projection transformation matrix</param>
        /// <param name="world">World transformation matrix</param>
        void ITransformedDrawable.Draw(Matrix view, Matrix projection, Matrix world)
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
