using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ActionGame.People
{
    public class Player : Human
    {
        const float MouseXSensitivityCoef = 0.5f;
        const float MouseYSensitivityCoef = 0.4f;
        static readonly Keys Left = Keys.A;
        static readonly Keys Right = Keys.D;
        static readonly Keys Forward = Keys.W;
        static readonly Keys Backward = Keys.S;
        static readonly Keys GunUp = Keys.LeftControl;
        static readonly Keys GunDown = Keys.LeftAlt;
        static readonly Keys ShotKey = Keys.Space;
        static readonly Keys EnterCar = Keys.Enter;
        static readonly Keys TurnLeft = Keys.Left;
        static readonly Keys TurnRight = Keys.Right;
        static readonly Keys TurnUp = Keys.Up;
        static readonly Keys TurnDown = Keys.Down;
        static readonly Keys RunSwitch = Keys.CapsLock;

        private bool running = false;
        private Point lastMousePosition = Point.Zero;
        ActionGame game;
        double lookAngle = 0f;

        public Player(ActionGame game)
            :base(null, new PositionInTown(null, Vector2.Zero), 0, Matrix.Identity)
        {
            this.game = game;
        }

        public void Load(Model model, PositionInTown position, double azimuth, Matrix worldTransform)
        {
            base.load(model, position, 0, azimuth, worldTransform);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (health > 0)
            {
                if (keyboardState.IsKeyDown(Left))
                    Step(true, seconds);
                if (keyboardState.IsKeyDown(Right))
                    Step(false, seconds);
                if (keyboardState.IsKeyDown(Forward))
                {
                    if (running)
                        Run(seconds);
                    else
                        Go(true, seconds);
                }
                if (keyboardState.IsKeyDown(Backward))
                    Go(false, seconds);
                if (keyboardState.IsKeyDown(TurnLeft))
                    Rotate(true, seconds);
                if (keyboardState.IsKeyDown(TurnRight))
                    Rotate(false, seconds);
                if (keyboardState.IsKeyDown(TurnUp))
                    lookAngle += Human.RotateAngle * seconds;
                if (keyboardState.IsKeyDown(TurnDown))
                    lookAngle -= Human.RotateAngle * seconds;
                if (keyboardState.IsKeyDown(RunSwitch))
                    running = !running;

                int windowWidth = game.Settings.ScreenSize.Width;
                int windowHeight = game.Settings.ScreenSize.Height;
                if (game.Settings.MouseIgnoresWindow || (mouseState.X >= 0 && mouseState.X < windowWidth && mouseState.Y >= 0 && mouseState.Y < windowHeight))
                {
                    azimuth += ( (mouseState.X - lastMousePosition.X) / (float)windowWidth) * game.Settings.MouseXSensitivity * MouseXSensitivityCoef * seconds * Human.RotateAngle * (game.Settings.MouseXInvert ? -1 : 1);
                    lookAngle += ((mouseState.Y - lastMousePosition.Y) / (float)windowWidth) * game.Settings.MouseYSensitivity * MouseYSensitivityCoef * seconds * Human.RotateAngle * (game.Settings.MouseYInvert ? 1 : -1);
                    if (lookAngle > MathHelper.PiOver2)
                        lookAngle = MathHelper.PiOver2;
                    if (lookAngle < -MathHelper.PiOver2)
                        lookAngle = -MathHelper.PiOver2;
                    lookingAtHeight = (float)Math.Sin(lookAngle) * LookingAtDistance + size.Y;
                    lastMousePosition = new Point(mouseState.X, mouseState.Y);
                }
            }

            CheckHits();

            Debug.Write("Player", PositionInQuarter.ToString());
            Debug.Write("Player azimuth", azimuth.ToString());
            Debug.Write("Player's grid fields", SpacePartitioningFields.Count.ToString());


            //Supress human instincts
            //base.Update(gameTime);
        }
    }
}
