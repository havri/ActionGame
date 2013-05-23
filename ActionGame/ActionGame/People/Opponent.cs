using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ActionGame.Planner;
using ActionGame.Tools;
using ActionGame.Tasks;
using ActionGame.Space;

namespace ActionGame.People
{
    /// <summary>
    /// Class describing the opponent.
    /// </summary>
    public class Opponent : Human
    {
        static readonly TimeSpan TasksReplanTimeout = new TimeSpan(0, 2, 0);
        TimeSpan lastTasksReplanTime = TimeSpan.Zero;
        bool planning = false;

        public override TimeSpan CheckEnemiesInViewConeTimeout { get { return new TimeSpan(0, 0, 0, 0, 200); } }
        protected override TimeSpan KillEnemyReflexTimeout { get { return new TimeSpan(0, 0, 0, 0, 70); } }
        protected override TimeSpan CheckEnemiesInQuarterTimeout { get { return new TimeSpan(0, 0, 1); } }

        /// <summary>
        /// Creates a new opponent human.
        /// </summary>
        /// <param name="game">The game</param>
        public Opponent(ActionGame game)
            : base(game, null, new PositionInTown(null, Vector2.Zero), 0, Matrix.Identity)
        {
            foreach (Tool gun in from gunType in game.PlayerDefaultGuns select new Gun(gunType, gunType.DefaultBulletCount, this, game))
            {
                AddTool(gun);
            }
        }
        /// <summary>
        /// Loads and sets the opponent specific content and settings
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="position">Postion</param>
        /// <param name="azimuth">Azimuth</param>
        /// <param name="worldTransform">World transform matrix</param>
        public void Load(Model model, PositionInTown position, double azimuth, Matrix worldTransform)
        {
            base.Load(model, position, 0, azimuth, worldTransform);

            Content = new TownQuarterOwnerContent
            {
                AllyHumanModel = Game.Content.Load<Model>("Objects/Humans/botYellow"),
                FlagModel = Game.Content.Load<Model>("Objects/Decorations/flagYellow2"),
                RoadSignTexture = Game.Content.Load<Texture2D>("Textures/roadSignYellow"),
                ColorTexture = Game.Content.Load<Texture2D>("Textures/yellow"),
                DrawingColor = System.Drawing.Color.Yellow
            };

            Running = true;

        }
        /// <summary>
        /// Updates the opponent's logic - solves the tasks, plans the tasks asynchronously and call the humans behaviour process.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="gameLogicOnly">The simple mod update indicator</param>
        public override void Update(GameTime gameTime, bool gameLogicOnly)
        {
            bool willPlan = false;
            lock(this)
            {
                base.Update(gameTime, gameLogicOnly);
                bool hasAnythingToDo = HasAnythingToDo;
                Debug.Write("Opponent has plan", hasAnythingToDo.ToString());
                willPlan = (!hasAnythingToDo || gameTime.TotalGameTime - lastTasksReplanTime > TasksReplanTimeout) && !planning;
                if(willPlan)
                {
                    planning = true;
                }
            }
            if (willPlan)
            {
                planning = true;
                lastTasksReplanTime = gameTime.TotalGameTime;
                System.Threading.Tasks.Task.Factory.StartNew(() => { PlanTasks(gameTime); });
                //PlanTasks(gameTime);
            }
            CheckFlagInMyQuarter();
            Debug.Write("Opponent position", Position.ToString());
        }

        void CheckFlagInMyQuarter()
        {
            if (Position.Quarter.Owner != this && (!HasAnythingToDo || Tasks.First.Value.TargetQuarter != Position.Quarter))
            {
                Tasks.AddFirst(new ActionObjectTask(Position.Quarter.Flag, this));
            }
        }

        void PlanTasks(GameTime gameTime)
        {
            lock (this)
            {
                ClearTasks();
            }

            GameState currentState = GetCurrentGameState(gameTime);

            List<Operation> operationPathToBest = PreparePlan(currentState);

            lock (this)
            {
                foreach (Operation operation in operationPathToBest)
                {
                    AddTask(operation.CreateTask(this));
                }
                planning = false;
            }
        }

        private static List<Operation> PreparePlan(GameState currentState)
        {
            const int maxDepth = 4;
            Dictionary<GameState, Tuple<GameState, Operation>> stateParents = new Dictionary<GameState, Tuple<GameState, Operation>>(); //tuple - parent state, step operation
            Dictionary<GameState, int> stateDepths = new Dictionary<GameState, int>();
            Queue<GameState> searchQueue = new Queue<GameState>();
            searchQueue.Enqueue(currentState);
            stateDepths.Add(currentState, 0);
            float bestRank = float.MinValue;
            GameState bestState = null;
            while (searchQueue.Count != 0)
            {
                GameState state = searchQueue.Dequeue();
                float rank = state.Evaluate();
                if (rank > bestRank)
                {
                    bestState = state;
                    bestRank = rank;
                }
                int depth = stateDepths[state];
                if (depth < maxDepth)
                {
                    Operation[] ops = state.GetAvailableOperations();
                    foreach (Operation op in ops)
                    {
                        GameState gs = op.Operate(state);
                        stateParents.Add(gs, new Tuple<GameState, Operation>(state, op));
                        searchQueue.Enqueue(gs);
                        stateDepths.Add(gs, depth + 1);
                    }
                }
            }

            List<Operation> operationPathToBest = new List<Operation>(stateDepths[bestState]);
            {
                GameState state = bestState;
                while (stateParents.ContainsKey(state))
                {
                    operationPathToBest.Add(stateParents[state].Item2);
                    state = stateParents[state].Item1;
                }
                operationPathToBest.Reverse();
            }
            return operationPathToBest;
        }

        GameState GetCurrentGameState(GameTime gameTime)
        {
            QuarterState[] qStates = new QuarterState[Game.Town.Quarters.Length];
            for (int i = 0; i < qStates.Length; i++)
            {
                qStates[i] = new QuarterState();
                if (Game.Town.Quarters[i].Owner == this)
                {
                    qStates[i].Ownership = QuarterOwnership.My;
                }
                else if (Game.Town.Quarters[i].Owner == Game.Player)
                {
                    qStates[i].Ownership = QuarterOwnership.His;
                }
                else
                {
                    qStates[i].Ownership = QuarterOwnership.Empty;
                }
                qStates[i].OwnershipDuration = gameTime.TotalGameTime - Game.Town.Quarters[i].OwnershipBeginTime;
            }
            float damage = 0;
            foreach (Tool tool in Tools)
            {
                if (tool is Gun)
                {
                    Gun gun = (Gun)tool;
                    float gunUtility = gun.Type.Damage * gun.Bullets;
                    damage += gunUtility;
                }
            }
            return new GameState(Game) { QuarterStates = qStates, Damage = damage, Position = Position, Health = Health };
        }

        public override void Destroy()
        {
            Position.Quarter.DestroyObject(this);
        }
    }
}
