//Needs define in ActionGame.cs too
//#define debug
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame
{
    /// <summary>
    /// The debuging component. It provides debug messages to be writen on the screen during the game.
    /// </summary>
    class Debug : DrawableGameComponent
    {
        static Dictionary<string, string> writings = new Dictionary<string, string>();

        /// <summary>
        /// Adds a debug message in the dictonary. 
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="text">Message</param>
        public static void Write(string key, string text)
        {
#if debug
            if (writings.ContainsKey(key))
                writings[key] = text;
            else
                writings.Add(key, text);
#endif
        }

        /// <summary>
        /// Adds a debug message in the dictonary. 
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="obj">Message object</param>
        public static void Write(string key, object obj)
        {
            Write(key, obj.ToString());
        }

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        ActionGame game;
        SpriteFont font;

        /// <summary>
        /// Creates a new debug component.
        /// </summary>
        /// <param name="game">The game</param>
        public Debug(ActionGame game)
            : base(game)
        {
            this.game = game;
        }

        /// <summary>
        /// Loads the content needed for this component.
        /// </summary>
        protected override void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("Fonts/SpriteFont1");
            
            base.LoadContent();
        }

        /// <summary>
        /// Draws the all debug messages in the dictonary.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            base.Draw(gameTime);

            game.SpriteBatch.Begin();
            int i = 0;
            foreach (var writing in writings)
            {
                game.SpriteBatch.DrawString(font, String.Format("{0}: {1}", writing.Key, writing.Value), new Vector2(10,100 + i * 16), Color.White);
                i++;
            }
            game.SpriteBatch.DrawString(font, String.Format("{0}: {1}", "FPS", frameRate), new Vector2(10, 100 + i * 16), Color.White);
            game.SpriteBatch.End();
        }

        /// <summary>
        /// Updates its content - calculates the FPS value.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

            base.Update(gameTime);
        }
    }
}
