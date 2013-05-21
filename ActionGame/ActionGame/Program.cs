using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ActionGame.MenuForms;

namespace ActionGame
{
#if WINDOWS
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ActionGame game = new ActionGame())
            {
                game.Run();
            }
        }
    }
#endif
}

