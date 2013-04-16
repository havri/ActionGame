using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.People
{
    public class Opponent : Human
    {
        public Opponent(ActionGame game)
            : base(game, null, new PositionInTown(null, Vector2.Zero), 0, Matrix.Identity)
        { }

        public void Load(Model model, PositionInTown position, double azimuth, Matrix worldTransform)
        {
            base.Load(model, position, 0, azimuth, worldTransform);

            Content = new TownQuarterOwnerContent
            {
                AllyHumanModel = Game.Content.Load<Model>("Objects/Humans/botYellow"),
                FlagModel = Game.Content.Load<Model>("Objects/Decorations/flagYellow"),
                RoadSignTexture = Game.Content.Load<Texture2D>("Textures/roadSignYellow"),
                ColorTexture = Game.Content.Load<Texture2D>("Textures/yellow"),
                DrawingColor = System.Drawing.Color.Yellow
            };
        }

    }
}
