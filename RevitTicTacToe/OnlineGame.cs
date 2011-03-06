using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace RevitTicTacToe
{
    public class OnlineGame : IGame
    {
        private UIDocument _uiDoc;
        private BoardManager _boardManager;
        private ScoreKeeper _scoreKeeper;
        private MultiplayerServer _server;
        private int _sessionId;
        private string _playerId;

        public OnlineGame(UIDocument uiDoc, BoardManager boardManager, ScoreKeeper scoreKeeper, MultiplayerServer multiplayerServer,
            string currentUserId)
        {
            _uiDoc = uiDoc;
            _boardManager = boardManager;
            _scoreKeeper = scoreKeeper;
            _server = multiplayerServer;
            _playerId = currentUserId;
        }

        /// <summary>
        /// Starts the online game
        /// </summary>
        public void StartGame()
        {
            //Show the session ID screen with option to generate new one 
            OnlineGameForm form = new OnlineGameForm(this);
            form.ShowDialog();

            //get Session ID selected
            _sessionId = form.GetEnteredSessionId();
            if (_sessionId < 0)
                throw new InvalidSessionIdException("You must enter a valid Session Id");

            //now we are in a session, we'll see who is X and who is O
            bool currentPlayerType = GetCurrentPlayerType();

            int moveCounter = 0;
            //start off the player as X player
            bool currentPlayer = ScoreKeeper.X_PLAYER;

            while (moveCounter <= 8)
            {
                //if its current players turn, get them to select
                if (currentPlayerType == currentPlayer)
                {
                    if (!DoCurrentPlayersTurn(currentPlayer))
                    {
                        //cancelled turn, must want to quit 
                        return;
                    }
                }
                else
                {
                    if (!DoOtherPlayersTurn(currentPlayer))
                    {
                        //cancelled waiting, must want to quit 
                        return;
                    }
                }

                //check if the game is over
                if (IsGameOver())
                    return;

                //next turn
                moveCounter++;
                currentPlayer = !currentPlayer;
            }
        }

        /// <summary>
        /// Create a new session ID from the server
        /// </summary>
        /// <returns></returns>
        public int GetNewSessionIdFromServer()
        {
            return _server.StartNewGame(_playerId);
        }

        /// <summary>
        /// Checks who the current player is 
        /// </summary>
        /// <returns></returns>
        public bool GetCurrentPlayerType()
        {
            return _server.JoinGame(_sessionId, _playerId);
        }


        /// <summary>
        /// Gets the user to have their turn
        /// </summary>
        /// <param name="currentPlayer"></param>
        private bool DoCurrentPlayersTurn(bool currentPlayer)
        {
            try
            {
                while (true)
                {
                    //get the user to pick a point
                    XYZ xyz = _uiDoc.Selection.PickPoint("Your turn");

                    int roomEntered = _boardManager.ProcessRoom(xyz, currentPlayer);
                    //Process the room, if its not a valid selection, loop again
                    if (roomEntered < 0)
                        continue;

                    //send the selection to the server
                    _server.SendMove(_sessionId, _playerId, roomEntered);
                    return true;
                }
            } catch(OperationCanceledException ex)
            {
                //user cancelled
                return false;
            }
        }


        /// <summary>
        /// Waits for the other player to have their turn.
        /// </summary>
        private bool DoOtherPlayersTurn(bool oppositionPlayer)
        {
            //Show waiting Dialog
            WaitingOnOtherPlayerForm form = new WaitingOnOtherPlayerForm(_server, _sessionId, _playerId);
            DialogResult result = form.ShowDialog();

            if (result == DialogResult.OK)
            {
                //got their turn
                int turn = form.GetOppositionsSquare();
                if (turn < 0)
                    return false;

                _boardManager.ProcessRoom(turn, oppositionPlayer);
                return true;
            }

            //they must have wanted to close the game so exit
            return false;
        }

        /// <summary>
        /// Checks for a winner
        /// </summary>
        /// <returns></returns>
        private bool IsGameOver()
        {
            bool? winner = _boardManager.CheckWinner();
            if (winner == null)
            {
                return false;
            }

            if (winner == ScoreKeeper.X_PLAYER)
            {
                _scoreKeeper.IncrementX();
                TaskDialog.Show("Game Over!", "X player wins");

                //end session
                _server.EndGame(_sessionId);

                return true;
            }
            if (winner == ScoreKeeper.O_PLAYER)
            {
                _scoreKeeper.IncrementO();
                TaskDialog.Show("Game Over!", "O player wins");

                //end session
                _server.EndGame(_sessionId);

                return true;
            }
            return false;
        }
    }
}
