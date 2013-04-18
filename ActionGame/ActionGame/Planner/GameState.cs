using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            float quarterIntex = EvalQuarters();
            return Damage * Health * quarterIntex;
        }

        float EvalQuarters()
        {
            float quarterIndex = 0;
            foreach (QuarterState qs in QuarterStates)
            {
                int guardCount = (int)(qs.OwnershipDuration.TotalSeconds / TownQuarter.GuardAddTimeout.TotalSeconds);
                if (guardCount > TownQuarter.MaxGuardCount)
                {
                    guardCount = TownQuarter.MaxGuardCount;
                }
                int multiplier;
                switch (qs.Ownership)
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
                float quarterValue = ((float)guardCount / TownQuarter.MaxGuardCount) * multiplier;
                quarterIndex += quarterValue;
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
    }
}
