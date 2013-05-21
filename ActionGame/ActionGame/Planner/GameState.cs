using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.Tools;
using ActionGame.World;

namespace ActionGame.Planner
{
    class GameState
    {
        readonly ActionGame game;

        public QuarterState[] QuarterStates { get; set; }
        public PositionInTown Position { get; set; }
        public float Damage { get; set; }
        public float Health { get; set; }

        public GameState(ActionGame game)
        {
            this.game = game;
        }

        public float Evaluate()
        {
            if (QuarterStates.All(qs => qs.Ownership == QuarterOwnership.My))
            {
                return float.MaxValue;
            }
            const float healthQ = 0.01f; // health is in percents
            const float damageQ = 0.02f * TownQuarter.MaxGuardCount; //percentage and it's useful to have enough to clean whole quarter
            float quarterIntex = EvalQuarters();
            return Damage * damageQ + Health * healthQ + quarterIntex * game.Settings.TownQuarterCount;
        }

        float EvalThisQuarter(QuarterState quarterState)
        {
            int guardCount = (int)(quarterState.OwnershipDuration.TotalSeconds / TownQuarter.GuardAddTimeout.TotalSeconds);
            if (guardCount > TownQuarter.MaxGuardCount)
            {
                guardCount = TownQuarter.MaxGuardCount;
            }
            int multiplier;
            switch (quarterState.Ownership)
            {
                case QuarterOwnership.My:
                    multiplier = 1;
                    break;
                case QuarterOwnership.His:
                    multiplier = -1;
                    break;
                default:
                    multiplier = 0;
                    break;
            }
            return ((float)guardCount / TownQuarter.MaxGuardCount) * multiplier;
        }

        float EvalQuarters()
        {
            float quarterIndex = 0;
            foreach (QuarterState qs in QuarterStates)
            {
                quarterIndex += EvalThisQuarter(qs);
            }
            return quarterIndex / QuarterStates.Length;
        }

        public GameState Copy()
        {
            QuarterState[] qs = new QuarterState[QuarterStates.Length];
            Array.Copy(QuarterStates, qs, qs.Length);
            GameState gs = new GameState(game)
            {
                Damage = Damage,
                Health = Health,
                Position = Position,
                QuarterStates = qs
            };
            return gs;
        }

        public void AddTime(TimeSpan timeSpan)
        {
            for (int i = 0; i < QuarterStates.Length; i++)
            {
                QuarterStates[i].OwnershipDuration += timeSpan;
            }
        }

        public Operation[] GetAvailableOperations()
        {
            List<Operation> operations = new List<Operation>();
            if (Health > 0)
            {
                Box availableToolBox = Position.Quarter.GetNearestBox(Position, true);
                Box availableHealBox = Position.Quarter.GetNearestBox(Position, false);
                if (availableToolBox != null)
                {
                    operations.Add(new TakeBoxOperation(game, availableToolBox));
                }
                if (availableHealBox != null)
                {
                    operations.Add(new TakeBoxOperation(game, availableHealBox));
                }

                for (int i = 0; i < game.Town.Quarters.Length; i++)
                {
                    if (QuarterStates[i].Ownership != QuarterOwnership.My
                        && game.Random.NextDouble() <= -(EvalThisQuarter(QuarterStates[i])) ) // branch factor downsizing
                    {
                        operations.Add(new CaptureFlagOperation(game, i));
                    }
                }
                if (Damage > game.Player.Health)
                {
                    operations.Add(new KillPlayerOperation(game));
                }
            }

            return operations.ToArray();
        }
    }
}
