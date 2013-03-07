using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Tools;
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
        static readonly Keys GunUp = Keys.LeftAlt;
        static readonly Keys GunDown = Keys.LeftControl;
        static readonly Keys GunDrop = Keys.Back;
        static readonly Keys ShotKey = Keys.Space;
        static readonly Keys EnterCar = Keys.Enter;
        static readonly Keys TurnLeft = Keys.Left;
        static readonly Keys TurnRight = Keys.Right;
        static readonly Keys TurnUp = Keys.Up;
        static readonly Keys TurnDown = Keys.Down;  
        static readonly Keys RunSwitch = Keys.CapsLock;
        static readonly TimeSpan KeyPressedTimeout = new TimeSpan(0, 0, 0, 0, 250);

        private bool running = false;
        readonly Point defaultMousePosition;
        double lookAngle = 0f;
        private int lastMouseWheelState = 0;
        readonly Dictionary<Keys, TimeSpan> lastKeyPressedGameTime = new Dictionary<Keys, TimeSpan>();

        public Player(ActionGame game)
            :base(game, null, new PositionInTown(null, Vector2.Zero), 0, Matrix.Identity)
        {
            defaultMousePosition = new Point(game.Settings.ScreenSize.Width / 2, game.Settings.ScreenSize.Height / 2);
            foreach (Tool gun in from gunType in game.PlayerDefaultGuns select new Gun(gunType, gunType.DefaultBulletCount, this))
            {
                AddTool(gun);
            }
            lastKeyPressedGameTime.Add(GunUp, TimeSpan.Zero);
            lastKeyPressedGameTime.Add(GunDown, TimeSpan.Zero);
            lastKeyPressedGameTime.Add(RunSwitch, TimeSpan.Zero);
            lastKeyPressedGameTime.Add(GunDrop, TimeSpan.Zero);
        }

        public void Load(Model model, PositionInTown position, double azimuth, Matrix worldTransform)
        {
            base.Load(model, position, 0, azimuth, worldTransform);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Health > 0 || true)
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
                if (keyboardState.IsKeyDown(RunSwitch) && gameTime.TotalGameTime - lastKeyPressedGameTime[RunSwitch] > KeyPressedTimeout)
                {
                    running = !running;
                    lastKeyPressedGameTime[RunSwitch] = gameTime.TotalGameTime;
                }
                if (keyboardState.IsKeyDown(GunUp) && gameTime.TotalGameTime - lastKeyPressedGameTime[GunUp] > KeyPressedTimeout)
                {
                    SelectNextGun(1);
                    lastKeyPressedGameTime[GunUp] = gameTime.TotalGameTime;
                }
                if (keyboardState.IsKeyDown(GunDown) && gameTime.TotalGameTime - lastKeyPressedGameTime[GunDown] > KeyPressedTimeout)
                {
                    SelectNextGun(-1);
                    lastKeyPressedGameTime[GunDown] = gameTime.TotalGameTime;
                }
                if (keyboardState.IsKeyDown(GunDrop) && gameTime.TotalGameTime - lastKeyPressedGameTime[GunDrop] > KeyPressedTimeout)
                {
                    DropSelectedTool();
                    lastKeyPressedGameTime[GunDrop] = gameTime.TotalGameTime;
                }
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    DoToolAction(gameTime);
                }

                SelectNextGun((mouseState.ScrollWheelValue - lastMouseWheelState) / 120);
                lastMouseWheelState = mouseState.ScrollWheelValue;


                int windowWidth = Game.Settings.ScreenSize.Width;
                int windowHeight = Game.Settings.ScreenSize.Height;
                if (Game.Settings.MouseIgnoresWindow || (mouseState.X >= 0 && mouseState.X < windowWidth && mouseState.Y >= 0 && mouseState.Y < windowHeight))
                {
                    azimuth += ( (mouseState.X - defaultMousePosition.X) / (float)windowWidth) * Game.Settings.MouseXSensitivity * MouseXSensitivityCoef * seconds * Human.RotateAngle * (Game.Settings.MouseXInvert ? -1 : 1);
                    lookAngle += ((mouseState.Y - defaultMousePosition.Y) / (float)windowWidth) * Game.Settings.MouseYSensitivity * MouseYSensitivityCoef * seconds * Human.RotateAngle * (Game.Settings.MouseYInvert ? 1 : -1);
                    if (lookAngle > MathHelper.PiOver2)
                        lookAngle = MathHelper.PiOver2;
                    if (lookAngle < -MathHelper.PiOver2)
                        lookAngle = -MathHelper.PiOver2;
                    lookingAtHeight = (float)Math.Sin(lookAngle) * LookingAtDistance + size.Y;
                    //lastMousePosition = new Point(mouseState.X, mouseState.Y);
                }
                Mouse.SetPosition(defaultMousePosition.X, defaultMousePosition.Y);
            }

            CheckHits();

            Debug.Write("Player", PositionInQuarter.ToString());
            Debug.Write("Player azimuth", azimuth.ToString());
            Debug.Write("Player's grid fields", SpacePartitioningFields.Count.ToString());
            Debug.Write("Wheel", mouseState.ScrollWheelValue.ToString());


            //Supress human instincts
            //base.Update(gameTime);
        }
    }
}
