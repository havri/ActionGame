﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Tools
{
    /// <summary>
    /// Describes gun type - gun's tech. specification.
    /// </summary>
    public class GunType : IDisposable
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
        /// <summary>
        /// Shot action sound.
        /// </summary>
        public SoundEffect ShootSount
        {
            get { return shotSount; }
        } 
        private readonly SoundEffect shotSount;

        /// <summary>
        /// Creates a new gun type
        /// </summary>
        /// <param name="damage">The one bullet damage</param>
        /// <param name="range">The shootSount range</param>
        /// <param name="infinity">Whether it does not need ammo indicator</param>
        /// <param name="shotTimeout">Timeout between two shoot</param>
        /// <param name="defaultBulletCount">The default number of bullets loaded in the gun</param>
        /// <param name="icon">The gun's icon</param>
        /// <param name="shot">Shoot soundeffect</param>
        public GunType(int damage, float range, bool infinity, TimeSpan shotTimeout, int defaultBulletCount, Texture2D icon, SoundEffect shot)
        {
            this.damage = damage;
            this.range = range;
            this.infinityBullets = infinity;
            this.icon = icon;
            this.shotTimeout = shotTimeout;
            this.defaultBulletCount = defaultBulletCount;
            shotSount = shot;
        }

        public void Dispose()
        {
            shotSount.Dispose();
        }
    }
}
