using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ActionGame.Planner;
using ActionGame.Tools;

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

            Running = true;

        }

        void PlanTasks(GameTime gameTime)
        {
            GameState currentState = GetCurrentGameState(gameTime);

        }

        GameState GetCurrentGameState(GameTime gameTime)
        {
            QuarterState[] qStates = new QuarterState[Game.Town.Quarters.Length];
            for (int i = 0; i < qStates.Length; i++)
            {
                qStates[i] = new QuarterState();
                if (Game.Town.Quarters[i].Owner == this)
                {
                    qStates[i].Ownership = QuarterOwnership.My;
                }
                else if (Game.Town.Quarters[i].Owner == Game.Player)
                {
                    qStates[i].Ownership = QuarterOwnership.His;
                }
                else
                {
                    qStates[i].Ownership = QuarterOwnership.Empty;
                }
                qStates[i].OwnershipDuration = gameTime.TotalGameTime - Game.Town.Quarters[i].OwnershipBeginTime;
            }
            float damage = 0;
            foreach (Tool tool in Tools)
            {
                if (tool is Gun)
                {
                    Gun gun = (Gun)tool;
                    float gunUtility = gun.Type.Damage * gun.Bullets;
                    damage += gunUtility;
                }
            }
            return new GameState(Game) { QuarterStates = qStates, Damage = damage, Position = Position, Health = Health };
        }
    }
}
