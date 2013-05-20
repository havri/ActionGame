﻿using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Objects;
using ActionGame.Space;
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
        static readonly Keys TurnLeft = Keys.Left;
        static readonly Keys TurnRight = Keys.Right;
        static readonly Keys TurnUp = Keys.Up;
        static readonly Keys TurnDown = Keys.Down;  
        static readonly Keys RunSwitch = Keys.CapsLock;
        static readonly TimeSpan KeyPressedTimeout = new TimeSpan(0, 0, 0, 0, 250);
        static readonly TimeSpan HurtFullscreenEffectDuration = new TimeSpan(0, 0, 0, 0, 500);
        static public readonly TimeSpan RespawnFullscreenEffectDuration = new TimeSpan(0, 0, 0, 1);

        readonly Point defaultMousePosition;
        private int lastMouseWheelState = 0;
        readonly Dictionary<Keys, TimeSpan> lastKeyPressedGameTime = new Dictionary<Keys, TimeSpan>();
        ActionObject usedActionObject = null;

        public Player(ActionGame game)
            :base(game, null, new PositionInTown(null, Vector2.Zero), 0, Matrix.Identity)
        {
            defaultMousePosition = new Point(game.Settings.ScreenSize.Width / 2, game.Settings.ScreenSize.Height / 2);
            foreach (Tool gun in from gunType in game.PlayerDefaultGuns select new Gun(gunType, gunType.DefaultBulletCount, this))
            {
                AddTool(gun);
            }
            lastKeyPressedGameTime.Add(RunSwitch, TimeSpan.Zero);
        }

        public new void Load(Model model, PositionInTown position, double azimuth, Matrix worldTransform)
        {
            base.Load(model, position, azimuth, worldTransform);
            
            Content = new TownQuarterOwnerContent
            {
                AllyHumanModel = Game.Content.Load<Model>("Objects/Humans/botBlue"),
                FlagModel = Game.Content.Load<Model>("Objects/Decorations/flagBlue"),
                RoadSignTexture = Game.Content.Load<Texture2D>("Textures/roadSignBlue"),
                ColorTexture = Game.Content.Load<Texture2D>("Textures/blue"),
                DrawingColor = System.Drawing.Color.Blue
            };
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Math.Abs(Health) + 1 > 0 && System.Windows.Forms.Form.ActiveForm == (System.Windows.Forms.Control.FromHandle(Game.Window.Handle) as System.Windows.Forms.Form))
            {
                if (keyboardState.IsKeyDown(Left))
                    Step(true, seconds);
                if (keyboardState.IsKeyDown(Right))
                    Step(false, seconds);
                if (keyboardState.IsKeyDown(Forward))
                {
                    Go(seconds);
                }
                if (keyboardState.IsKeyDown(Backward))
                    GoBack(seconds);
                if (keyboardState.IsKeyDown(TurnLeft))
                    Rotate(true, seconds);
                if (keyboardState.IsKeyDown(TurnRight))
                    Rotate(false, seconds);
                if (keyboardState.IsKeyDown(TurnUp))
                    LookAngle += Human.RotateAngle * seconds;
                if (keyboardState.IsKeyDown(TurnDown))
                    LookAngle -= Human.RotateAngle * seconds;
                if (keyboardState.IsKeyDown(RunSwitch) && gameTime.TotalGameTime - lastKeyPressedGameTime[RunSwitch] > KeyPressedTimeout)
                {
                    Running = !Running;
                    lastKeyPressedGameTime[RunSwitch] = gameTime.TotalGameTime;
                }
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    DoToolAction(gameTime);
                }
                if (mouseState.RightButton == ButtonState.Pressed && HasAvailableAnyAction)
                {
                    if (usedActionObject != FirstActionObject && usedActionObject != null)
                    {
                        usedActionObject.EndAction(this, gameTime);
                    }
                    usedActionObject = FirstActionObject;
                    usedActionObject.StartAction(this, gameTime);
                }
                else
                {
                    if (usedActionObject != null)
                    {
                        usedActionObject.EndAction(this, gameTime);
                    }
                }

                SelectNextGun((mouseState.ScrollWheelValue - lastMouseWheelState) / 120);
                lastMouseWheelState = mouseState.ScrollWheelValue;


                int windowWidth = Game.Settings.ScreenSize.Width;
                int windowHeight = Game.Settings.ScreenSize.Height;
                if (Game.Settings.MouseIgnoresWindow || (mouseState.X >= 0 && mouseState.X < windowWidth && mouseState.Y >= 0 && mouseState.Y < windowHeight))
                {
                    azimuth += ( (mouseState.X - defaultMousePosition.X) / (float)windowWidth) * Game.Settings.MouseXSensitivity * MouseXSensitivityCoef * seconds * Human.RotateAngle * (Game.Settings.MouseXInvert ? -1 : 1);
                    LookAngle += ((mouseState.Y - defaultMousePosition.Y) / (float)windowWidth) * Game.Settings.MouseYSensitivity * MouseYSensitivityCoef * seconds * Human.RotateAngle * (Game.Settings.MouseYInvert ? 1 : -1);
                    if (LookAngle > MathHelper.PiOver4)
                        LookAngle = MathHelper.PiOver4;
                    if (LookAngle < -MathHelper.PiOver4)
                        LookAngle = -MathHelper.PiOver4;
                    //lastMousePosition = new Point(mouseState.X, mouseState.Y);
                }
                Mouse.SetPosition(defaultMousePosition.X, defaultMousePosition.Y);
            }

            CheckHits(false, gameTime);
            
            //Supress human instincts
            //base.Update(gameTime);
        }

        public override void Hit(Quadrangle something, bool gameLogicOnly, GameTime gameTime)
        {
            if (something is ToolBox)
            {
                Hit(something as ToolBox);
            }
            else if (something is HealBox)
            {
                Hit(something as HealBox);
            }
            else
            {
                MoveTo(LastPosition, Azimuth);
            }
        }

        public override void BecomeShot(GameTime gameTime, int damage, Human by)
        {
            base.BecomeShot(gameTime, damage, by);
            Game.Drawer.ShowFullscreenEffect(gameTime, Game.ContentRepository.HurtFullscreenEffect, HurtFullscreenEffectDuration);
        }


        public override void Destroy()
        {
            Position.Quarter.DestroyObject(this);
        }
    }
}
