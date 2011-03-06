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
    public partial class OnlineGameForm : Form
    {
        private OnlineGame _onlineGame;
        public OnlineGameForm(OnlineGame onlineGame)
        {
            _onlineGame = onlineGame;
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnNewSession_Click(object sender, EventArgs e)
        {
            int sessionid = _onlineGame.GetNewSessionIdFromServer();
            this.txtSessionId.Text = sessionid.ToString();
            MessageBox.Show("Your new session ID is: " + sessionid + " - tell this to your opponent");

        }

        /// <summary>
        /// Gets the session ID that was entered
        /// </summary>
        /// <returns></returns>
        public int GetEnteredSessionId()
        {
            int sessionId;
            if (int.TryParse(txtSessionId.Text, out sessionId))
                return sessionId;
            return -1;
        }
    }
}
