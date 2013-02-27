using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame
{
    class Debug : DrawableGameComponent
    {
        static Dictionary<string, string> writings = new Dictionary<string, string>();

        public static void Write(string key, string text)
        {
            if (writings.ContainsKey(key))
                writings[key] = text;
            else
                writings.Add(key, text);
        }

        ActionGame game;
        SpriteFont font;

        public Debug(ActionGame game)
            : base(game)
        {
            this.game = game;
        }

        protected override void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("Fonts/SpriteFont1");
            
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            game.SpriteBatch.Begin();
            int i = 0;
            foreach (var writing in writings)
            {
                game.SpriteBatch.DrawString(font, String.Format("{0}: {1}", writing.Key, writing.Value), new Vector2(10,100 + i * 16), Color.White);
                i++;
            }
            game.SpriteBatch.End();
        }
    }
}
