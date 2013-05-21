using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ActionGame.MenuForms
{
    public partial class KeyReader : Form
    {
        Microsoft.Xna.Framework.Input.Keys selectedKey;
        public KeyReader(string title)
        {
            InitializeComponent();
            this.Text = title;
        }

        public Microsoft.Xna.Framework.Input.Keys ShowSelectDialog()
        {
            this.ShowDialog();
            return selectedKey;
        }

        private void CatchKeyPressed(object sender, KeyEventArgs e)
        {
            Microsoft.Xna.Framework.Input.Keys[] keys = Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeys();
            if (keys.Length != 0)
            {
                selectedKey = keys[0];
                Close();
            }
        }
    }
}
