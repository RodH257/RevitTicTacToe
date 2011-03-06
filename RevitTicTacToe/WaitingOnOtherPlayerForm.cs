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
    public partial class WaitingOnOtherPlayerForm : Form
    {
        private MultiplayerServer _server;
        private int _sessionid;
        private string _currentPlayerId;
        private int _oppositionsMove;
        public WaitingOnOtherPlayerForm(MultiplayerServer server, int sessionid, string currentPlayerId)
        {
            _server = server;
            _sessionid = sessionid;
            _currentPlayerId = currentPlayerId;
            InitializeComponent();
            tmrMove.Interval = 2000;
            tmrMove.Tick += new EventHandler(tmrMove_Tick);
            tmrMove.Start();
        }

        void tmrMove_Tick(object sender, EventArgs e)
        {
            //check if the other player has had their go yet
            _oppositionsMove = _server.CheckForOppositionsMove(_sessionid, _currentPlayerId);
            if (_oppositionsMove > 0)
            {
                //got a move, stop waiting
                tmrMove.Stop();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnQuitGame_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Gets the room that the opposition entered
        /// </summary>
        /// <returns></returns>
        internal int GetOppositionsSquare()
        {
            return _oppositionsMove;
        }
    }
}
