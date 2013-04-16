using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Space;
using ActionGame.Tasks;
using ActionGame.World;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ActionGame.Tools;
using ActionGame.Objects;

namespace ActionGame.People
{
    public class Human : SpatialObject, ITownQuarterOwner
    {
        /// <summary>
        /// Speed of human walk. In meters per second.
        /// </summary>
        const float WalkSpeed = 1.25f;
        /// <summary>
        /// Speed of human run. In meters per second.
        /// </summary>
        const float RunSpeed = 6f;
        /// <summary>
        /// Speed of human rotation. In radians per second.
        /// </summary>
        public const double RotateAngle = MathHelper.Pi;
        const float ThirdHeadHorizontalDistance = 1.5f;
        const float ThirdHeadVerticalDistance = 0.1f;
        public const float LookingAtDistance = 10;
        public const float EpsilonDistance = 0.5f;
        public static readonly TimeSpan CheckEnemiesInViewConeTimeout = new TimeSpan(0, 0, 0, 1, 500);

        /// <summary>
        /// Gets current health of human. In percents.
        /// </summary>
        public int Health { get { return health; } }
        int health;
        private readonly Queue<Task> tasks;
        private readonly List<Tool> tools;
        private int selectedToolIndex;
        private Vector2 lastPosition;
        protected ActionGame Game { get { return game; } }
        private readonly ActionGame game;
        public double LookAngle { get { return lookAngle; } set { lookAngle = value; } }
        double lookAngle = 0f;
        readonly HashSet<Human> friends = new HashSet<Human>();
        readonly HashSet<Human> enemies = new HashSet<Human>();
        readonly HashSet<Human> hasMeAsFriend = new HashSet<Human>();
        readonly HashSet<Human> hasMeAsEnemy = new HashSet<Human>();
        Human lastSeenEnemy;
        TimeSpan lastTimeSawEnemy;
        readonly HashSet<ActionObject> availableActionObjects = new HashSet<ActionObject>();
        public TownQuarterOwnerContent Content { get { return content; } set { content = value; } }
        TownQuarterOwnerContent content;

        public Human(ActionGame game, Model model, PositionInTown position, double azimuth, Matrix worldTransform)
            : base(model, position, azimuth, worldTransform)
        {
            this.game = game;
            health = 100;
            tasks = new Queue<Task>();
            tools = new List<Tool>();
            tools.AddRange(
                from gunType in game.HumanDefaultGuns select new Gun (gunType, gunType.DefaultBulletCount, this)
                );
            selectedToolIndex = 0;
            lastPosition = position.PositionInQuarter;
            lastSeenEnemy = null;
            lastTimeSawEnemy = TimeSpan.Zero;
        }

        public void Load(Model model, PositionInTown position, double azimuth, Matrix worldTransform)
        {
            base.Load(model, position, 0, azimuth, worldTransform);
        }

        protected void Go(bool forward, float seconds)
        {
            lastPosition = Position.PositionInQuarter;
            MoveTo(Position.PositionInQuarter.Go(WalkSpeed * seconds * (forward ? 1 : -1), azimuth), Azimuth);
        }

        protected void Run(float seconds)
        {
            lastPosition = Position.PositionInQuarter;
            MoveTo(Position.PositionInQuarter.Go(RunSpeed * seconds, azimuth), Azimuth);
        }

        protected void Step(bool toLeft, float seconds)
        {
            lastPosition = Position.PositionInQuarter;
            MoveTo(Position.PositionInQuarter.Go(WalkSpeed * seconds, azimuth + (toLeft ? -MathHelper.PiOver2 : MathHelper.PiOver2)), Azimuth);
        }

        protected void Rotate(bool toLeft, float seconds)
        {
            azimuth += (toLeft ? -1 : 1) * RotateAngle * seconds;
        }

        public Vector3 FirstHeadPosition
        {
            get
            { 
                Vector2 ret = Pivot.PositionInQuarter.Go(2*Size.Z, azimuth);
                return ret.ToVector3(Size.Y);
            }
        }

        public Vector3 ThirdHeadPosition
        {
            get
            {
                Vector2 ret = Pivot.PositionInQuarter.Go(-(Size.Z + ThirdHeadHorizontalDistance), azimuth);
                return ret.ToVector3(Size.Y + ThirdHeadVerticalDistance);
            }
        }

        public Vector3 LookingAt
        {
            get
            {
                float distance2D = (float)Math.Cos(lookAngle) * LookingAtDistance;
                Vector2 ret = Pivot.PositionInQuarter.Go((Size.Z + distance2D), azimuth);
                return ret.ToVector3((float)Math.Sin(lookAngle) * LookingAtDistance + Size.Y);
            }
        }

        public void GoThisWay(PositionInTown destination, float seconds)
        {
            if (destination.Quarter == Position.Quarter)
            {
                float direction = (destination.PositionInQuarter - Position.PositionInQuarter).GetAngle() + 1*MathHelper.PiOver2;
                while (direction >= MathHelper.TwoPi) direction -= MathHelper.TwoPi;
                if (Math.Abs(azimuth - direction) > RotateAngle || (azimuth + MathHelper.TwoPi - direction) > RotateAngle && direction > azimuth)
                {
                    Rotate(
                        (azimuth > direction && direction >= 0 && azimuth - direction < MathHelper.Pi) || (direction > azimuth && direction - azimuth > MathHelper.Pi),
                        seconds);
                }
                else
                {
                    azimuth = direction;
                    Go(true, seconds);
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
            bool moved = false;
            //Order of reflexes is important.
            if (!moved)
                moved = KillEnemyReflex(gameTime);
            if (!moved)
                moved = BalkReflex(gameTime);

            //Solve tasks reflex
            if (!moved)
            {
                if (tasks.Count > 0)
                {
                    Task currentTask = tasks.Peek();
                    currentTask.Update(gameTime);
                    if (currentTask.IsComplete())
                    {
                        tasks.Dequeue();
                    }
                }
            }
        }

        bool BalkReflex(GameTime gameTime)
        {
            const float balkDistance = 0.9f;
            Quadrangle viewCone = GetViewCone(balkDistance);
            IEnumerable<Quadrangle> balks = Position.Quarter.SpaceGrid.GetAllCollisions(viewCone);
            if(balks.Any(x => x != this && x is Human))
            {
                float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Step(false, totalSeconds);
                //Rotate(false, totalSeconds);
                return true;
            }
            return false;
        }

        bool KillEnemyReflex(GameTime gameTime)
        {
            if (tools.Any(x => (x is Gun && x.Usable)))
            {
                Human seenEnemy = null;
                float enemyShotDistance = tools.Max(x => (x is Gun && x.Usable ? ((Gun)x).Type.Range : 0f));
                Quadrangle viewCone = GetViewCone(enemyShotDistance);
                if (lastSeenEnemy != null && viewCone.IsInCollisionWith(lastSeenEnemy))
                {
                    Quadrangle clearViewQuad = new Quadrangle(lastSeenEnemy.PositionInQuarter.XZToVector2(), lastSeenEnemy.PositionInQuarter.XZToVector2(), UpperLeftCorner, UpperRightCorner);
                    IEnumerable<Quadrangle> inView = from obj in Position.Quarter.SpaceGrid.GetAllCollisions(clearViewQuad) where obj != this && obj != lastSeenEnemy select obj;
                    if (inView.Any())
                    {
                        lastSeenEnemy = null;
                    }
                    else
                    {
                        seenEnemy = lastSeenEnemy;
                    }
                }
                else
                {
                    lastSeenEnemy = null;
                }
                if (gameTime.TotalGameTime - lastTimeSawEnemy >= CheckEnemiesInViewConeTimeout && seenEnemy == null)
                {
                    IEnumerable<Human> seenEnemies = from obj in Position.Quarter.SpaceGrid.GetAllCollisions(viewCone)
                                                     where obj is Human && obj != this && enemies.Contains((Human)obj)
                                                     select obj as Human;
                    if (seenEnemies.Any())
                    {
                        seenEnemy = seenEnemies.First();
                    }
                    lastTimeSawEnemy = gameTime.TotalGameTime;
                }
                if (seenEnemy != null)
                {
                    lastSeenEnemy = seenEnemy;
                    selectedToolIndex = tools.FindIndex(x => (x is Gun && ((Gun)x).Type.Range == enemyShotDistance));
                    GoThisWay(seenEnemy.Position, (float)gameTime.ElapsedGameTime.TotalSeconds);
                    float direction = (seenEnemy.PositionInQuarter.XZToVector2() - Position.PositionInQuarter).GetAngle() + 1 * MathHelper.PiOver2;
                    if (direction == azimuth)
                    {
                        DoToolAction(gameTime);
                    }
                    return true;
                }
            }
            return false;
        }

        Quadrangle GetViewCone(float distance)
        { 
            const float viewConeWidthCoef = 1.2f;
            float humanWidth = (UpperLeftCorner - UpperRightCorner).Length();
            float coneExpansion = ((distance * viewConeWidthCoef) - humanWidth) * 0.5f;
            Vector2 ul = UpperLeftCorner.Go(distance, azimuth).Go(coneExpansion, azimuth - MathHelper.PiOver2);
            Vector2 ur = UpperLeftCorner.Go(distance, azimuth).Go(coneExpansion, azimuth + MathHelper.PiOver2);
            return new Quadrangle(ul, ur, UpperLeftCorner, UpperRightCorner);
        }

        public void AddTask(Task task)
        {
            tasks.Enqueue(task);
        }

        public Tool SelectedTool
        { 
            get
            {
                if (tools.Count == 0)
                    return null;
                else
                    return tools[selectedToolIndex];
            }
        }

        protected void SelectNextGun(int jumpLength)
        {
            jumpLength %= tools.Count;
            selectedToolIndex = (selectedToolIndex + jumpLength + tools.Count) % tools.Count;
        }

        public void DoToolAction(GameTime gameTime)
        {
            if (SelectedTool != null)
            {
                SelectedTool.DoAction(gameTime, Position, (float)azimuth);
            }
        }
        public override void Hit(Quadrangle something)
        {
            if (something is ToolBox)
            {
                Hit(something as ToolBox);
            }
            else
            {
                MoveTo(lastPosition, Azimuth);
            }
        }

        private void Hit(ToolBox box)
        {
            Tool takenTool = box.Take();
            if(takenTool is Gun)
            {
                Gun takenGun = (Gun)takenTool;
                int ind = tools.FindIndex(x => (x is Gun && ((Gun)x).Type == takenGun.Type));
                if(ind >= 0 && ind < tools.Count)
                {
                    ((Gun)tools[ind]).Load(takenGun.Bullets);
                    selectedToolIndex = ind;
                }
                else
                {
                    AddTool(takenGun);
                }
            }
        }

        protected void AddTool(Tool tool)
        {
            tool.Holder = this;
            tools.Add(tool);
            selectedToolIndex = tools.Count - 1;
        }

        public override void BecomeShot(int damage, Human by)
        {
            health -= damage;
            if (health <= 0)
            {
                Destroy();
            }
            else if(by != null)
            {
                AddEnemy(by);
            }
        }

        protected void DropSelectedTool()
        {
            if(tools.Count != 0 && !(SelectedTool is Gun && game.HumanDefaultGuns.Contains(((Gun)SelectedTool).Type)))
            {
                tools.RemoveAt(selectedToolIndex);
            }
        }

        public void AddEnemy(Human enemy)
        {
            if(!enemies.Contains(enemy))
            {
                enemy.hasMeAsEnemy.Add(this);
                enemies.Add(enemy);
                RemoveFriend(enemy);
            }
        }

        public void AddFriend(Human friend)
        {
            if (!friends.Contains(friend))
            {
                friend.hasMeAsFriend.Add(this);
                friends.Add(friend);
                RemoveEnemy(friend);
            }
        }


        public void RemoveEnemy(Human enemy)
        {
            if (enemies.Contains(enemy))
            {
                enemy.hasMeAsEnemy.Remove(this);
                enemies.Remove(enemy);
            }
        }

        public void RemoveFriend(Human friend)
        {
            if (friends.Contains(friend))
            {
                friend.hasMeAsFriend.Remove(this);
                friends.Remove(friend);
            }
        }

        public override void Destroy()
        {
            foreach (Human hasMe in new List<Human>(hasMeAsEnemy))
            {
                hasMe.RemoveEnemy(this);
            }
            foreach (Human hasMe in new List<Human>(hasMeAsFriend))
            {
                hasMe.RemoveFriend(this);
            }
            base.Destroy();
        }

        public void RegisterAvailableAction(ActionObject actionObject)
        {
            availableActionObjects.Add(actionObject);
        }

        public void UnregisterAvailableAction(ActionObject actionObject)
        {
            availableActionObjects.Remove(actionObject);
        }

        public bool HasAvailableAnyAction
        {
            get
            {
                return availableActionObjects.Count != 0;
            }
        }

        protected ActionObject FirstActionObject
        {
            get
            {
                return availableActionObjects.First();
            }
        }


        public Human CreateAllyGuard(TownQuarter targetQuarter)
        {
            PositionInTown pos = new PositionInTown(targetQuarter, targetQuarter.GetRandomSquare(x => x == MapFillType.Empty).ToVector2() * TownQuarter.SquareWidth);
            Human guard = new Human(game, Content.AllyHumanModel, pos, 0, game.Drawer.WorldTransformMatrix);
            foreach (Human enemy in enemies)
                guard.AddEnemy(enemy);
            foreach (Human friend in friends)
                guard.AddFriend(friend);
            foreach (Human has in hasMeAsEnemy)
                has.AddEnemy(guard);
            guard.AddTask(new InfinityWalkingTask(guard, targetQuarter.GetRandomWalkingWaypoints()));
            foreach(GunType gt in game.GuardDefaultGuns)
            {
                guard.AddTool(new Gun(gt, gt.DefaultBulletCount, guard));
            }
            return guard;
        }
    }
}
