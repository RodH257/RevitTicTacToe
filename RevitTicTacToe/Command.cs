using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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

            //Find out which game to play 
            NewGameMenu menu = new NewGameMenu();
            if (menu.ShowDialog() != DialogResult.OK)
                return Result.Cancelled;

            //setup the game
            IGame game = SetupGame(menu.IsOnlineGame());

            //run the game
            game.StartGame();

            return Result.Succeeded;
        }

        /// <summary>
        /// Sets up eithr a local game or online game
        /// </summary>
        /// <param name="onlinePlay"></param>
        /// <returns></returns>
        private IGame SetupGame(bool onlinePlay)
        {
            //setup board manager
            _boardManager = new BoardManager(_dbDoc);

            //clear the board
            _boardManager.ClearBoard();

            //setup ScoreKeeper
            _scoreKeeper = new ScoreKeeper(_dbDoc);

            if (onlinePlay)
            {
                RestfulCommunicator restfulCommunicator = new RestfulCommunicator();
                MultiplayerServer server = new MultiplayerServer(restfulCommunicator);
                return new OnlineGame(_uiDoc, _boardManager, _scoreKeeper, server);
            }

            return new LocalGame(_uiDoc, _boardManager, _scoreKeeper);
        }



    }
}
