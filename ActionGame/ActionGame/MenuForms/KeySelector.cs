using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ActionGame.MenuForms
{
    public partial class KeySelector : UserControl
    {
        Microsoft.Xna.Framework.Input.Keys value;

        public KeySelector(string name, Microsoft.Xna.Framework.Input.Keys @default)
        {
            InitializeComponent();
            label.Text = name;
            value = @default;
            textBox.Text = value.ToString();
        }

        private void CatchKeyTextBoxEntered(object sender, EventArgs e)
        {
            using (KeyReader kr = new KeyReader(label.Text))
            {
                value = kr.ShowSelectDialog();
                textBox.Text = value.ToString();
            }
        }

        public Microsoft.Xna.Framework.Input.Keys Value
        {
            get
            {
                return value;
            }
        }
    }
}
