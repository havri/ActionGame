using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace ActionGame.Components
{
    /// <summary>
    /// Component for playing sounds.
    /// </summary>
    public class SoundPlayer
    {
        readonly ActionGame game;
        /// <summary>
        /// Creates a new sound playing component.
        /// </summary>
        /// <param name="game"></param>
        public SoundPlayer(ActionGame game)
        {
            this.game = game;
        }

        /// <summary>
        /// Plays specified sound at specified place. Automaticly set the volume.
        /// </summary>
        /// <param name="sound">The specified sound</param>
        /// <param name="position">The specified position</param>
        public void PlaySound(SoundEffect sound, PositionInTown position)
        {
            if (game.Player.Position.Quarter == position.Quarter)
            { 
                float quarterDiagonalLength = (new Vector2(position.Quarter.BitmapSize.Width, position.Quarter.BitmapSize.Height) * TownQuarter.SquareWidth).Length();
                sound.Play(position.MinimalDistanceTo(game.Player.Position) / quarterDiagonalLength, 0f, 0f);
            }
        }
    }
}
