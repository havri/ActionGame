using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Space;
using ActionGame.Tasks;
using ActionGame.World;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.People
{
    public class Human : SpatialObject
    {
        const float StepLength = 0.05f;
        public const double RotateAngle = MathHelper.Pi / 50f;
        const float ThirdHeadHorizontalDistance = 1.5f;
        const float ThirdHeadVerticalDistance = 0.1f;
        const float LookingAtDistance = 10;
        const float LookingAtHeightStep = 0.3f;
        public const float EpsilonDistance = 0.5f;
        public static readonly TimeSpan StepTimeOut = new TimeSpan(0, 0, 0, 0, 30);

        protected int health;
        protected float lookingAtHeight;

        Queue<Task> tasks;

        public Human(Model model, PositionInTown position, double azimuth, Matrix worldTransform)
            : base(model, position, azimuth, worldTransform)
        {
            health = 100;
            tasks = new Queue<Task>();
        }

        protected void Go(bool forward)
        {
            position.PositionInQuarter = position.PositionInQuarter.Go(StepLength * (forward ? 1 : -1), azimuth);
        }

        protected void Step(bool toLeft)
        {
            position.PositionInQuarter = position.PositionInQuarter.Go(StepLength, azimuth + (toLeft ? -MathHelper.PiOver2 : MathHelper.PiOver2));
        }

        protected void Rotate(bool toLeft)
        {
            azimuth += (toLeft ? -1 : 1) * RotateAngle;
        }

        public Vector3 FirstHeadPosition
        {
            get
            { 
                Vector2 ret = Pivot.PositionInQuarter.Go(2*size.Z, azimuth);
                return ret.ToVector3(size.Y);
            }
        }

        public Vector3 ThirdHeadPosition
        {
            get
            {
                Vector2 ret = Pivot.PositionInQuarter.Go(-(size.Z + ThirdHeadHorizontalDistance), azimuth);
                return ret.ToVector3(size.Y + ThirdHeadVerticalDistance);
            }
        }

        public Vector3 LookingAt
        {
            get
            {
                Vector2 ret = Pivot.PositionInQuarter.Go((size.Z + LookingAtDistance), azimuth);
                return ret.ToVector3(lookingAtHeight * LookingAtHeightStep);
            }
        }

        public void GoThisWay(PositionInTown destination)
        {
            if (destination.Quarter == this.position.Quarter)
            {
                float direction = (destination.PositionInQuarter - this.position.PositionInQuarter).GetAngle() + 1*MathHelper.PiOver2;
                while (direction >= MathHelper.TwoPi) direction -= MathHelper.TwoPi;
                if (Math.Abs(azimuth - direction) > RotateAngle || (azimuth + MathHelper.TwoPi - direction) > RotateAngle && direction > azimuth)
                {
                    this.Rotate((azimuth > direction && direction >= 0 && azimuth - direction < MathHelper.Pi) || (direction > azimuth && direction - azimuth > MathHelper.Pi));
                }
                else
                {
                    azimuth = direction;
                    this.Go(true);
                }
            }
            else
            {
                ///TODO: Impletemnt going to another quarter using interfaces.
                throw new NotImplementedException("Going to another quarter isn't implemented.");
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (tasks.Count > 0)
            {
                Task currentTask = tasks.Peek();
                currentTask.Update(gameTime);
                if (currentTask.IsComplete())
                    tasks.Dequeue();
            }
        }

        public int Health
        {
            get
            {
                return health;
            }
        }

        public void AddTask(Task task)
        {
            tasks.Enqueue(task);
        }
    }
}
