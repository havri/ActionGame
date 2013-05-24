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
using ActionGame.Exceptions;
using ActionGame.Components;

namespace ActionGame.People
{
    /// <summary>
    /// Class that represents human (or robot) in the game. It has reflexes, task and generally behaviour.
    /// </summary>
    public class Human : SpatialObject, ITownQuarterOwner
    {
        /// <summary>
        /// Speed of human walk. In meters per second.
        /// </summary>
        public const float WalkSpeed = 1.25f;
        /// <summary>
        /// Speed of human run. In meters per second.
        /// </summary>
        public const float RunSpeed = 6.1f;
        /// <summary>
        /// Speed of human rotation. In radians per second.
        /// </summary>
        public const double RotateAngle = MathHelper.Pi;
        const float ThirdHeadHorizontalDistance = 1.5f;
        const float ThirdHeadVerticalDistance = 0.22f;
        /// <summary>
        /// Distance from target where human decides he's there.
        /// </summary>
        public const float EpsilonDistance = TownQuarter.SquareWidth / 4f;

        public virtual TimeSpan CheckEnemiesInViewConeTimeout { get { return new TimeSpan(0, 0, 0, 0, 900); } }
        protected virtual TimeSpan KillEnemyReflexTimeout { get { return new TimeSpan(0, 0, 0, 0, 650); } }
        protected virtual TimeSpan CheckEnemiesInQuarterTimeout { get { return new TimeSpan(0, 0, 5); } }

        TimeSpan lastKillEnemyReflexTime = TimeSpan.Zero;
        TimeSpan lastCheckEnemiesInQuarterTime = TimeSpan.Zero;
        /// <summary>
        /// Gets current health of human. In percents.
        /// </summary>
        public int Health { get { return health; } }
        int health;
        readonly LinkedList<Task> tasks = new LinkedList<Task>();
        protected LinkedList<Task> Tasks
        {
            get
            {
                return tasks;
            }
        }
        readonly List<Tool> tools = new List<Tool>();
        protected List<Tool> Tools
        {
            get { return tools; }
        }
        private int selectedToolIndex;
        private Vector2 lastPosition;
        protected Vector2 LastPosition
        {
            get
            {
                return lastPosition;
            }
            set
            {
                lastPosition = value;
            }
        }
        protected ActionGame Game { get { return game; } }
        private readonly ActionGame game;
        public double LookAngle
        { 
            get { return lookAngle; } 
            set
            {
                lookAngle = value; 
            }
        }
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
        float actualMoveSpeed = WalkSpeed;
        protected bool Running
        {
            get
            {
                return actualMoveSpeed == RunSpeed;
            }
            set
            {
                if (value)
                {
                    actualMoveSpeed = RunSpeed;
                }
                else
                {
                    actualMoveSpeed = WalkSpeed;
                }
            }
        }

        bool inGodMode = false;
        public bool InGodMode
        {
            get { return inGodMode; }
            set { inGodMode = value; }
        }

        Quadrangle lastHitObject;
        TimeSpan firstTimeHitObject;
        static readonly TimeSpan StuckTimeout = new TimeSpan(0, 0, 0, 3);


        /// <summary>
        /// Creates a new instance of human
        /// </summary>
        /// <param name="game">The game</param>
        /// <param name="model">Model</param>
        /// <param name="position">Position</param>
        /// <param name="azimuth">Azimuth</param>
        /// <param name="worldTransform">World transform matrix</param>
        public Human(ActionGame game, Model model, PositionInTown position, double azimuth, Matrix worldTransform)
            : base(model, position, azimuth, worldTransform)
        {
            this.game = game;
            health = 100;
            tools.AddRange(
                from gunType in game.HumanDefaultGuns select new Gun (gunType, gunType.DefaultBulletCount, this, game)
                );
            selectedToolIndex = 0;
            lastPosition = position.PositionInQuarter;
            lastSeenEnemy = null;
            lastTimeSawEnemy = TimeSpan.Zero;
        }

        /// <summary>
        /// Loads the human specific content. This can be used if the content wasn't specified in the construct.
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="position">Position</param>
        /// <param name="azimuth">Azimuth</param>
        /// <param name="worldTransform">World transform matrix</param>
        public void Load(Model model, PositionInTown position, double azimuth, Matrix worldTransform)
        {
            base.Load(model, position, 0, azimuth, worldTransform);
            lastPosition = position.PositionInQuarter;
        }

        protected void Go(float seconds)
        {
            lastPosition = Position.PositionInQuarter;
            MoveTo(Position.PositionInQuarter.Go(actualMoveSpeed * seconds, Azimuth), Azimuth);
        }

        protected void GoBack(float seconds)
        {
            lastPosition = Position.PositionInQuarter;
            MoveTo(Position.PositionInQuarter.Go(WalkSpeed * seconds * -1, Azimuth), Azimuth);
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

        /// <summary>
        /// Gets the position of camera in first person see mode.
        /// </summary>
        public Vector3 FirstHeadPosition
        {
            get
            { 
                Vector2 ret = Pivot.PositionInQuarter.Go(2*Size.Z, azimuth);
                return ret.ToVector3(Size.Y);
            }
        }
        /// <summary>
        /// Gets the position of camera in third person mode.
        /// </summary>
        public Vector3 ThirdHeadPosition
        {
            get
            {
                return GetLookingAtCoordinates(Pivot.PositionInQuarter.ToVector3(Size.Y + ThirdHeadVerticalDistance), -ThirdHeadHorizontalDistance);
                /*Vector2 ret = Pivot.PositionInQuarter.Go(-zDist, azimuth);
                return ret.ToVector3(Size.Y + yDist);*/
            }
        }
        /// <summary>
        /// Calculates point that the human is looking at.
        /// </summary>
        /// <param name="from">The eyes position</param>
        /// <param name="distance">Looking distance</param>
        /// <returns>The watched point</returns>
        Vector3 GetLookingAtCoordinates(Vector3 from, float distance)
        {
            float distance2D = (float)Math.Cos(lookAngle) * distance;
            Vector2 ret = from.XZToVector2().Go(distance2D, azimuth);
            return ret.ToVector3((float)Math.Sin(lookAngle) * distance + from.Y);
        }
        /// <summary>
        /// Gets the point that the human is looking at.
        /// </summary>
        public Vector3 LookingAt
        {
            get
            {
                return GetLookingAtCoordinates(FirstHeadPosition, Drawer.ViewDistance);
            }
        }

        private bool IsAzimuthTooFarFrom(double direction, double maxDeltaAngle)
        {
            return (Azimuth + MathHelper.TwoPi + MathHelper.TwoPi - direction) % MathHelper.TwoPi > maxDeltaAngle;
        }
        /// <summary>
        /// Rotates the human toward the specified destination.
        /// </summary>
        /// <param name="destination">The destination</param>
        /// <param name="seconds">Moving duration</param>
        public void TurnThisWay(PositionInTown destination, float seconds)
        {
            if (destination.Quarter == Position.Quarter)
            {
                double actualRotateAngle = RotateAngle * seconds;
                float direction = (destination.PositionInQuarter - Position.PositionInQuarter).GetAngle() + 1 * MathHelper.PiOver2;
                direction = direction % MathHelper.TwoPi;
                if (IsAzimuthTooFarFrom(direction, actualRotateAngle))
                {
                    bool toLeft = (azimuth > direction && direction >= 0 && azimuth - direction < MathHelper.Pi) || (direction > azimuth && direction - azimuth > MathHelper.Pi);
                    Rotate(toLeft, seconds);
                }
                else
                {
                    azimuth = direction;
                }
            }
        }
        /// <summary>
        /// Performs move to the spcifed target.
        /// </summary>
        /// <param name="destination">Wished destination</param>
        /// <param name="seconds">Duration</param>
        public void GoThisWay(PositionInTown destination, float seconds)
        {
            if (destination.Quarter == Position.Quarter)
            {
                double actualRotateAngle = RotateAngle * seconds;
                float direction = (destination.PositionInQuarter - Position.PositionInQuarter).GetAngle() + 1*MathHelper.PiOver2;
                direction = direction % MathHelper.TwoPi;
                if (IsAzimuthTooFarFrom(direction, actualRotateAngle))
                {
                    bool toLeft = (azimuth > direction && direction >= 0 && azimuth - direction < MathHelper.Pi) || (direction > azimuth && direction - azimuth > MathHelper.Pi);
                    Rotate(toLeft, seconds);

                    if(Math.Abs(azimuth - direction) < MathHelper.PiOver2 && Position.MinimalDistanceTo(destination) > TownQuarter.SquareWidth)
                    {
                        Go(seconds);
                    }
                }
                else
                {
                    azimuth = direction;
                    Go(seconds);
                }
            }
            else
            {
                TownQuarterInterface rightIface = null;
                foreach (TownQuarterInterface iface in Position.Quarter.Interfaces)
                {
                    if (iface.OppositeInterface.Quarter == destination.Quarter)
                    {
                        rightIface = iface;
                    }
                }
                if(rightIface != null)
                {
                    if(Position.MinimalDistanceTo(rightIface.LeftPathGraphVertex.Position) <= Human.EpsilonDistance
                        || Position.MinimalDistanceTo(rightIface.RightPathGraphVertex.Position) <= Human.EpsilonDistance)
                    {
                        //Changes home quarter
                        TownQuarter newQuarter = rightIface.OppositeInterface.Quarter;
                        Position.Quarter.BeLeftBy(this);
                        Game.Drawer.StopDrawingObject(this);
                        Vector2 posDelta = Town.ResolveQuarterPositionDelta(rightIface);
                        float azDelta = Town.ResolveQuarterAzimuthDelta(rightIface.SidePosition, rightIface.OppositeInterface.SidePosition);
                        MoveTo(
                            new PositionInTown(newQuarter, Vector3.Transform(PositionInQuarter,
                                Matrix.CreateTranslation(-posDelta.ToVector3(0)) * Matrix.CreateRotationY(azDelta)).XZToVector2()),
                            Azimuth - azDelta
                                );
                        newQuarter.BeEnteredBy(this);
                        if (newQuarter.CurrentlyDrawed)
                        {
                            Game.Drawer.StartDrawingObject(this, newQuarter.CurrentDrawingAzimuthDelta, newQuarter.CurrentDrawingPositionDelta);
                        }
                    }
                    else
                    {
                        GoThisWay(rightIface.LeftPathGraphVertex.Position, seconds);
                    }
                }
            }
        }
        /// <summary>
        /// Updates the human's logic. Solves the behaviour.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="gameLogicOnly"></param>
        public virtual void Update(GameTime gameTime, bool gameLogicOnly)
        {
            bool moved = false;
            //Order of reflexes is important.
            if (!moved)
            {
                moved = KillEnemyReflex(gameTime);
            }
            if (!moved && !gameLogicOnly)
            {
                moved = BalkReflex(gameTime);
            }

            if(!SelectedTool.Usable)
            {
                SelectNextGun(1);
            }

            //Solve tasks
            if (!moved)
            {
                if (tasks.Count != 0)
                {
                    Task currentTask = tasks.First.Value;
                    if (this is Opponent)
                    {
                        Debug.Write("Opponents task", currentTask.ToString());
                    }
                    currentTask.Update(gameTime);
                    if (currentTask.IsComplete())
                    {
                        tasks.RemoveFirst();
                    }
                }
            }

            //post reflexes
            CheckEnemiesInMyQuarter(gameTime);
            CheckHits(gameLogicOnly, gameTime);
        }
        
        private void CheckEnemiesInMyQuarter(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - lastCheckEnemiesInQuarterTime > CheckEnemiesInQuarterTimeout && (!HasAnythingToDo || tasks.First.Value.TargetQuarter != Position.Quarter || (!(tasks.First.Value is KillTask) && !(tasks.First.Value is TemporaryTask<KillTask>))))
            {
                foreach (Human enemy in enemies)
                {
                    if (enemy.Position.Quarter == this.Position.Quarter)
                    {
                        KillTask killTask = new KillTask(this, enemy);
                        TemporaryTask<KillTask> tt = new TemporaryTask<KillTask>(this, killTask, task => task.Holder.Position.Quarter == task.Target.Position.Quarter);
                        tasks.AddFirst(tt);
                        break;
                    }
                }
                lastCheckEnemiesInQuarterTime = gameTime.TotalGameTime;
            }
        }

        bool BalkReflex(GameTime gameTime)
        {
            float balkDistance = this.Size.Z * 6f;
            //const float balkDistance = 0.9f;

            Quadrangle viewCone = GetViewCone(balkDistance);
            IEnumerable<Quadrangle> balks = Position.Quarter.SpaceGrid.GetAllCollisions(viewCone);
            if (balks.Any(x => x != this && (x is Human || x is ActionObject)))
            {
                float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Step(false, totalSeconds);
                return true;
            }
            return false;
        }

        bool KillEnemyReflex(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - lastKillEnemyReflexTime > KillEnemyReflexTimeout && tools.Any(x => (x is Gun && x.Usable)))
            {
                lastKillEnemyReflexTime = gameTime.TotalGameTime;
                Human seenEnemy = null;
                float enemyShotDistance = tools.Max(x => (x is Gun && x.Usable ? ((Gun)x).Type.Range : 0f));
                Quadrangle viewCone = GetViewCone(enemyShotDistance);
                if (lastSeenEnemy != null && viewCone.IsInCollisionWith(lastSeenEnemy))
                {
                    //Quadrangle clearViewQuad = new Quadrangle(lastSeenEnemy.PositionInQuarter.XZToVector2(), lastSeenEnemy.PositionInQuarter.XZToVector2(), Pivot.PositionInQuarter, Pivot.PositionInQuarter);
                    //IEnumerable<Quadrangle> inView = from obj in Position.Quarter.SpaceGrid.GetAllCollisions(clearViewQuad) where obj != this && obj != lastSeenEnemy select obj;
                    //if (inView.Any())
                    //{
                        //lastSeenEnemy = null;
                    //}
                    //else
                    //{
                        seenEnemy = lastSeenEnemy;
                    //}
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
                    float direction = (seenEnemy.PositionInQuarter.XZToVector2() - Position.PositionInQuarter).GetAngle() + 1 * MathHelper.PiOver2;
                    if (!IsAzimuthTooFarFrom(direction, gameTime.ElapsedGameTime.TotalSeconds * RotateAngle))
                    {
                        DoToolAction(gameTime);
                    }
                    else
                    {
                        GoThisWay(seenEnemy.Position, (float)gameTime.ElapsedGameTime.TotalSeconds);
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

        /// <summary>
        /// Adds a task to this human.
        /// </summary>
        /// <param name="task">The added task</param>
        public void AddTask(Task task)
        {
            tasks.AddLast(task);
        }

        /// <summary>
        /// Adds a task that has to be solved with high priority.
        /// </summary>
        /// <param name="task">The added task</param>
        public void AddUrgentTask(Task task)
        {
            tasks.AddFirst(task);
        }
        /// <summary>
        /// Gets the right now selected tool
        /// </summary>
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
        /// <summary>
        /// Performs the selected tool action.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public void DoToolAction(GameTime gameTime)
        {
            if (SelectedTool != null)
            {
                SelectedTool.DoAction(gameTime, new PositionInTown(Position.Quarter, FirstHeadPosition.XZToVector2()), (float)azimuth);
            }
        }
        /// <summary>
        /// Solves collision with specified object.
        /// </summary>
        /// <param name="something">The collision object</param>
        /// <param name="gameLogicOnly">Simple mode indicator</param>
        /// <param name="gameTime">Game time</param>
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
                if (!gameLogicOnly)
                {
                    if (something == lastHitObject && gameTime.TotalGameTime - firstTimeHitObject > StuckTimeout)
                    {
                        Point newPoint = Position.Quarter.GetRandomSquare(mft => mft == MapFillType.Road);
                        MoveTo(newPoint.ToVector2() * TownQuarter.SquareWidth + Vector2.One * TownQuarter.SquareWidth * 0.5f, azimuth);
                        lastHitObject  = null;
                        if(tasks.Count != 0)
                        {
                            tasks.First.Value.ClearWaypoints();
                        }
                    }
                    else
                    {
                        if(something != lastHitObject)
                        {
                            firstTimeHitObject = gameTime.TotalGameTime;
                        }
                        lastHitObject = something;
                        MoveTo(lastPosition, Azimuth);
                        GoBack((float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                }
            }
        }

        protected void Hit(HealBox box)
        {
            health += box.Take();
            if (health > 100)
            {
                health = 100;
            }
        }

        protected void Hit(ToolBox box)
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
        /// <summary>
        /// Shoots the human.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="damage">Damage of the shoot in percents</param>
        /// <param name="by">The shooter</param>
        public override void BecomeShoot(GameTime gameTime, int damage, Human by)
        {
            if (inGodMode)
            {
                damage = 0;
            }
            health -= damage;
            if (health <= 0)
            {
                Destroy();
            }
            else if(by != null)
            {
                AddEnemy(by);
                if (tasks.Count != 0)
                {
                    Task current = tasks.First.Value;
                    if (!(current is KillTask || current is TemporaryTask<KillTask>))
                    { 
                        tasks.AddFirst(new TemporaryTask<KillTask>(this, new KillTask(this, by), kt => kt.Holder.Position.Quarter == kt.Target.Position.Quarter));
                    }
                }
            }
        }

        protected void DropSelectedTool()
        {
            if(tools.Count != 0 && !(SelectedTool is Gun && game.HumanDefaultGuns.Contains(((Gun)SelectedTool).Type)))
            {
                tools.RemoveAt(selectedToolIndex);
            }
        }
        /// <summary>
        /// Adds an enemy to this human.
        /// </summary>
        /// <param name="enemy">The enemy</param>
        public void AddEnemy(Human enemy)
        {
            if(!enemies.Contains(enemy))
            {
                enemy.hasMeAsEnemy.Add(this);
                enemies.Add(enemy);
                RemoveFriend(enemy);
            }
        }
        /// <summary>
        /// Adds a friend to this human.
        /// </summary>
        /// <param name="enemy">The friend</param>
        public void AddFriend(Human friend)
        {
            if (!friends.Contains(friend))
            {
                friend.hasMeAsFriend.Add(this);
                friends.Add(friend);
                RemoveEnemy(friend);
            }
        }

        /// <summary>
        /// Removes person from enemy list.
        /// </summary>
        /// <param name="enemy">The ex-enemy</param>
        public void RemoveEnemy(Human enemy)
        {
            if (enemies.Contains(enemy))
            {
                enemy.hasMeAsEnemy.Remove(this);
                enemies.Remove(enemy);
            }
        }
        /// <summary>
        /// Removes person from friends list.
        /// </summary>
        /// <param name="enemy">The ex-friend</param>
        public void RemoveFriend(Human friend)
        {
            if (friends.Contains(friend))
            {
                friend.hasMeAsFriend.Remove(this);
                friends.Remove(friend);
            }
        }
        /// <summary>
        /// Destroys the human.
        /// </summary>
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
        /// <summary>
        /// Says to human that an action is available for him.
        /// </summary>
        /// <param name="actionObject">Nearby corresponding action object</param>
        public void RegisterAvailableAction(ActionObject actionObject)
        {
            availableActionObjects.Add(actionObject);
        }
        /// <summary>
        /// Says to human that an action is not available for him anymore.
        /// </summary>
        /// <param name="actionObject">Ex-nearby corresponding action object</param>
        public void UnregisterAvailableAction(ActionObject actionObject)
        {
            availableActionObjects.Remove(actionObject);
        }
        /// <summary>
        /// Gets indicator if any action is available.
        /// </summary>
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

        /// <summary>
        /// Creates a new guard for this human.
        /// </summary>
        /// <param name="targetQuarter">Quarted that has to be guarded</param>
        /// <returns>The guard</returns>
        public Human CreateAllyGuard(TownQuarter targetQuarter)
        {
            PositionInTown pos = new PositionInTown(targetQuarter, targetQuarter.GetRandomSquare(x => x == MapFillType.Road).ToVector2() * TownQuarter.SquareWidth);
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
                guard.AddTool(new Gun(gt, gt.DefaultBulletCount, guard, game));
            }
            return guard;
        }
        /// <summary>
        /// Removes all the tasks.
        /// </summary>
        public void ClearTasks()
        {
            tasks.Clear();
        }
        /// <summary>
        /// Gets indicator whether there are any tasks in the task list.
        /// </summary>
        public bool HasAnythingToDo
        {
            get
            {
                return tasks.Count != 0;
            }
        }
        /// <summary>
        /// Respawns the human into a specified quarter.
        /// </summary>
        /// <param name="targetQuarter">The destination quarter</param>
        public void RespawnInto(TownQuarter targetQuarter)
        {
            Position.Quarter.BeLeftBy(this);
            Position = new PositionInTown(targetQuarter,
                targetQuarter.GetRandomSquare(fillType => fillType == MapFillType.Road, point => point != targetQuarter.FlagPoint).ToVector2() * TownQuarter.SquareWidth);
            Position.Quarter.BeEnteredBy(this);
            health = 100;
            tasks.Clear();
        }

        /// <summary>
        /// Gets the guard appearing timeout.
        /// </summary>
        public virtual TimeSpan GuardAddTimeout
        {
            get
            {
                return new TimeSpan(0,0,0,10);
            }
        }
        /// <summary>
        /// Gets the full health value for new guards
        /// </summary>
        public virtual int GuardFullHealth
        {
            get
            {
                return 100;
            }
        }
    }
}
