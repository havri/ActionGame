using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ActionGame.MenuForms;

namespace ActionGame
{
#if WINDOWS
    /// <summary>
    /// The main class of the program. It contains the Main method which is called after application start.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                using (ActionGame game = new ActionGame())
                {
                    game.Run();
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Could not find XNA assemblies. Try to install DirecX 9.c and XNA Framework Redistributable 4.0 package located on installation disk.", "Assembly not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
#endif
}

