using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RevitTicTacToe
{
    /// <summary>
    /// Menu for starting a new game
    /// </summary>
    public partial class NewGameMenu : Form
    {
        private bool _onlineSelected = false;
        public NewGameMenu()
        {
            InitializeComponent();
        }

        internal bool IsOnlineGame()
        {
            return _onlineSelected;
        }

        private void btnLocal_Click(object sender, EventArgs e)
        {
            _onlineSelected = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnOnline_Click(object sender, EventArgs e)
        {
            _onlineSelected = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //rests the score tally
            DialogResult = DialogResult.No;
            Close();
        }
    }
}
