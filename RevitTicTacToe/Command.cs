using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace RevitTicTacToe
{
    /// <summary>
    /// Tic Tac Toe for Autodesk Revit by Rod Howarth (http://www.rodhowarth.com)
    /// </summary>
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {

        private Document _dbDoc;
        private UIDocument _uiDoc;
        private BoardManager _boardManager;
        private ScoreKeeper _scoreKeeper;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //show instructions
            TaskDialog.Show("Instructions", "The game will start with player X's turn, " +
                                            "click on a room to place your X, then pass the mouse to O. Press Escape to exit.");

            //setup class variables 
            _dbDoc = commandData.Application.ActiveUIDocument.Document;
            _uiDoc = commandData.Application.ActiveUIDocument;

            //setup board manager
            _boardManager = new BoardManager(_dbDoc);

            //clear the board
            _boardManager.ClearBoard();

            //setup ScoreKeeper
            _scoreKeeper = new ScoreKeeper(_dbDoc);

            MultiplayerServer server = new MultiplayerServer();
            TaskDialog.Show("result", server.StartNewGame());

            //run the game
            DoGame();

            return Result.Succeeded;
        }



        /// <summary>
        /// Does the game
        /// </summary>
        private void DoGame()
        {
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

                    //Process the room, if its not a valid selection, loop again
                    if (!_boardManager.ProcessRoom(xyz, currentPlayer))
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
                catch
                {
                    return;
                }
            }
        }
    }
}
