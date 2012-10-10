using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ActionGame
{
    class Player : Human
    {
        static readonly TimeSpan movingKeyTimeOut = new TimeSpan(0, 0, 0, 0, 50);

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
            :base(null, Vector3.Zero, 0, Matrix.Identity)
        { 

        }

        public void Load(Model model, Vector3 position, double azimuth, Matrix worldTransform)
        {
            base.load(model, position, azimuth, worldTransform);
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

                    azimuth += ((float)(mouseState.X - (windowWidth / 2)) / (float)(windowWidth/2) ) * Human.RotateAngle;
                    lookingAtHeight -= ((float)(mouseState.Y - (windowHeight / 2)) / (float)(windowHeight / 2));
                }
            }

            Debug.Write("Player", Position.ToString());
            Debug.Write("Player size", size.ToString());
            Debug.Write("Player pivot", Pivot.ToString());
            Debug.Write("Player UpperLeftCorner", UpperLeftCorner.ToString());
            Debug.Write("Player UpperRightCorner", UpperRightCorner.ToString());
            Debug.Write("Player LowerLeftCorner", LowerLeftCorner.ToString());
            Debug.Write("Player LowerRightCorner", LowerRightCorner.ToString());
        }
    }
}
