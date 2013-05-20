using System;
using System.Drawing;
using ActionGame.MenuForms;

namespace ActionGame
{
#if WINDOWS || XBOX
    static class Program
    {
        static ExitType exitType = global::ActionGame.ExitType.Interruption;
        public static ExitType ExitType
        {
            set
            {
                exitType = value;
            }
        }
        static Image townMap;
        public static Image TownMap
        {
            set
            {
                townMap = value;
            }
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            bool play = true;
            while (play)
            {
                play = false;
                exitType = global::ActionGame.ExitType.Interruption;
                using (ActionGame game = new ActionGame())
                {
                    game.Run();
                }
                if (exitType != global::ActionGame.ExitType.Interruption)
                {
                    using (GameOver gameOver = new GameOver(exitType == global::ActionGame.ExitType.PlayerWin, townMap))
                    {
                        play = gameOver.ShowDialog() != System.Windows.Forms.DialogResult.Cancel;
                    }
                    townMap.Dispose();
                }
            }
        }
    }
#endif
}

