using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Tools
{
    /// <summary>
    /// Describes gun type - gun's tech. specification.
    /// </summary>
    public class GunType
    {
        /// <summary>
        /// Value what is substracted from injured human. Human's health is between 0-100.
        /// </summary>
        public int Damage
        {
            get { return damage; }
        }
        private readonly int damage;
        /// <summary>
        /// Range of fired bullet can reach. In meters.
        /// </summary>
        public float Range
        {
            get { return range; }  
        }
        private readonly float range;
        /// <summary>
        /// Icon for Tool class.
        /// </summary>
        public Texture2D Icon { get { return icon; } }
        private readonly Texture2D icon;
        /// <summary>
        /// Determines if the gun needs to refill (classical) or shuts all the time (ex. hand fists).
        /// </summary>
        public bool InfinityBullets { get { return infinityBullets; } }
        private readonly bool infinityBullets;
        /// <summary>
        /// Timeout between two shots.
        /// </summary>
        public TimeSpan ShotTimeout { get { return shotTimeout; } }
        private readonly TimeSpan shotTimeout;
        /// <summary>
        /// Number of bullets you get with this gun from the box or as default equip.
        /// </summary>
        public int DefaultBulletCount { get { return defaultBulletCount; } }
        private readonly int defaultBulletCount;

        public GunType(int damage, float range, bool infinity, TimeSpan shotTimeout, int defaultBulletCount, Texture2D icon)
        {
            this.damage = damage;
            this.range = range;
            this.infinityBullets = infinity;
            this.icon = icon;
            this.shotTimeout = shotTimeout;
            this.defaultBulletCount = defaultBulletCount;
        }
    }
}
