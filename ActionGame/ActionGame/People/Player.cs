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
        static readonly TimeSpan movingKeyTimeOut = new TimeSpan(0, 0, 0, 0, 50);
        const float YRotateQ = 0.6f;

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
        static readonly Keys RunSwitch = Keys.CapsLock;

        private bool running = false;

        public Player()
            :base(null, new PositionInTown(null, Vector2.Zero), 0, Matrix.Identity)
        { 

        }

        public void Load(Model model, PositionInTown position, double azimuth, Matrix worldTransform)
        {
            base.load(model, position, 0, azimuth, worldTransform);
        }

        public override void Update(GameTime gameTime)
        {
            throw new InvalidOperationException("This Update overload is denied!");
        }

        public void Update(GameTime gameTime, int windowWidth, int windowHeight)
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
                if (keyboardState.IsKeyDown(RunSwitch))
                    running = !running;

                ///TODO: Better look. Cross in the middle of screen...
                /*
                //horizontal
                if (Math.Abs(mouseState.X - (windowWidth / 2)) > windowWidth / 10)
                {
                    azimuth += ((float)(mouseState.X - (windowWidth / 2)) / (float)(windowWidth / 2)) * Human.RotateAngle;
                }
                //vertical
                if (Math.Abs(mouseState.Y - (windowHeight / 2)) > windowHeight / 10)
                {
                    lookingAtHeight -= (((float)(mouseState.Y - (windowHeight / 2)) / (float)(windowHeight / 2))) * Player.YRotateQ;
                }
                */
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
