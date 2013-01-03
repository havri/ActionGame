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

        Keys left = Keys.A;
        Keys right = Keys.D;
        Keys forward  = Keys.W;
        Keys backward = Keys.S;
        Keys gunUp = Keys.LeftControl;
        Keys gunDown = Keys.LeftAlt;
        Keys shotKey = Keys.Space;
        Keys enterCar = Keys.Enter;
        Keys turnLeft = Keys.Left;
        Keys turnRIght = Keys.Right;

        TimeSpan lastMove = new TimeSpan(0);

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
            if (health > 0)
            {
                if ((gameTime.TotalGameTime - lastMove) > movingKeyTimeOut )
                {
                    if (keyboardState.IsKeyDown(left))
                        Step(true);
                    if (keyboardState.IsKeyDown(right))
                        Step(false);
                    if (keyboardState.IsKeyDown(forward))
                        Go(true);
                    if (keyboardState.IsKeyDown(backward))
                        Go(false);
                    if (keyboardState.IsKeyDown(turnLeft))
                        Rotate(true);
                    if (keyboardState.IsKeyDown(turnRIght))
                        Rotate(false);

                    ///TODO: Better look. Cross in the middle of screen...
                    
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
                }
            }

            Debug.Write("Player", PositionInQuarter.ToString());
            Debug.Write("Player azimuth", Azimuth.ToString());

            //Supress human instincts
            //base.Update(gameTime);
        }
    }
}
