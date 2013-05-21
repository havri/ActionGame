﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.People;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ActionGame.Components
{
    public class Camera : GameComponent
    {
        const float birdZPosition = 0.5f;
        static readonly TimeSpan changeTimeOut = new TimeSpan(0, 0, 0, 0, 140);

        public Matrix ViewMatrix {get;set;}

        
        CameraMode mode = CameraMode.ThirdPersonLook;
        TimeSpan lastChange = new TimeSpan(0);
        Vector3 cameraPosition;
        Vector3 cameraSubjectPosition;

        Human holdingHuman
        {
            get
            {
                return ((ActionGame)Game).Player;
            }
        }

        protected new ActionGame Game
        {
            get
            {
                return (ActionGame)base.Game;
            }
        }

        public Camera(ActionGame game)
            :base(game)
        { }

        /// <summary>
        /// Gets current camera side.
        /// </summary>
        public Vector3 Position
        {
            get { return cameraPosition; }
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Game.Settings.CameraSwitch) && (gameTime.TotalGameTime - lastChange) > changeTimeOut)
            {
                switch (mode)
                {
                    case CameraMode.FirstPersonLook:
                        mode = CameraMode.ThirdPersonLook;
                        break;
                    case CameraMode.ThirdPersonLook:
                        mode = CameraMode.FirstPersonLook;
                        break;
                    default:
                        mode = CameraMode.FirstPersonLook;
                        break;
                }

                lastChange = gameTime.TotalGameTime;
            }

            switch (mode)
            {
                case CameraMode.FirstPersonLook:
                    cameraPosition = holdingHuman.FirstHeadPosition;
                    cameraSubjectPosition = holdingHuman.LookingAt;
                    break;
                case CameraMode.ThirdPersonLook:
                    cameraPosition = holdingHuman.ThirdHeadPosition;
                    cameraSubjectPosition = holdingHuman.LookingAt;
                    break;
                default:
                    cameraPosition = Vector3.Zero;
                    cameraSubjectPosition = Vector3.Zero;
                    break;
            }

            Debug.Write("Camera mode", mode.ToString());
            Debug.Write("Camera", cameraPosition.ToString());
            Debug.Write("Camera subject", cameraSubjectPosition.ToString());

            ViewMatrix = Matrix.CreateLookAt(cameraPosition, cameraSubjectPosition, Vector3.Up);
            

            base.Update(gameTime);
        }
    }
}
