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

        static readonly Keys left = Keys.A;
        static readonly Keys right = Keys.D;
        static readonly Keys forward = Keys.W;
        static readonly Keys backward = Keys.S;
        static readonly Keys gunUp = Keys.LeftControl;
        static readonly Keys gunDown = Keys.LeftAlt;
        static readonly Keys shotKey = Keys.Space;
        static readonly Keys enterCar = Keys.Enter;
        static readonly Keys turnLeft = Keys.Left;
        static readonly Keys turnRIght = Keys.Right;

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
                if (keyboardState.IsKeyDown(left))
                    Step(true, seconds);
                if (keyboardState.IsKeyDown(right))
                    Step(false, seconds);
                if (keyboardState.IsKeyDown(forward))
                    Go(true, seconds);
                if (keyboardState.IsKeyDown(backward))
                    Go(false, seconds);
                if (keyboardState.IsKeyDown(turnLeft))
                    Rotate(true, seconds);
                if (keyboardState.IsKeyDown(turnRIght))
                    Rotate(false, seconds);

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
            Debug.Write("Player azimuth", Azimuth.ToString());
            Debug.Write("Player's grid fields", SpacePartitioningFields.Count.ToString());


            //Supress human instincts
            //base.Update(gameTime);
        }
    }
}
