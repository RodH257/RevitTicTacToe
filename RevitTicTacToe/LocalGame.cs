using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace RevitTicTacToe
{
    class LocalGame: IGame
    {
        private UIDocument _uiDoc;
        private BoardManager _boardManager;
        private ScoreKeeper _scoreKeeper;
        public LocalGame(UIDocument uiDoc, BoardManager boardManager, ScoreKeeper scoreKeeper )
        {
            _uiDoc = uiDoc;
            _boardManager = boardManager;
            _scoreKeeper = scoreKeeper;
        }

        /// <summary>
        /// Does the game
        /// </summary>
        public void StartGame()
        {
            //show instructions
            TaskDialog.Show("Instructions", "The game will start with player X's turn, " +
                                            "click on a room to place your X, then pass the mouse to O. Press Escape to exit.");

            int moveCounter = 0;
            //start off the player as X player
            bool currentPlayer = ScoreKeeper.X_PLAYER;

            //loop for each move
            while (moveCounter <= 8)
            {
                //construct message for status bar 
                string displayText = "Player X's turn";
                if (currentPlayer == ScoreKeeper.O_PLAYER)
                    displayText = "Player O's turn";

                try
                {
                    //get the user to pick a point
                    XYZ xyz = _uiDoc.Selection.PickPoint(displayText);

                    int room = _boardManager.ProcessRoom(xyz, currentPlayer);
                    //Process the room, if its not a valid selection, loop again
                    if (room < 0)
                        continue;

                    bool? winner = _boardManager.CheckWinner();
                    if (winner == null)
                    {
                        //increment move counter
                        moveCounter++;
                        //swap player
                        currentPlayer = !currentPlayer;
                        continue;
                    }

                    if (winner == ScoreKeeper.X_PLAYER)
                    {
                        _scoreKeeper.IncrementX();
                        TaskDialog.Show("Winner!", "X player wins");
                        return;
                    }
                    if (winner == ScoreKeeper.O_PLAYER)
                    {
                        _scoreKeeper.IncrementO();
                        TaskDialog.Show("Winner!", "O player wins");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }
    }
}
