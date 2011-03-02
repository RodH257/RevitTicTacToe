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
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Automatic)]
    public class Command : IExternalCommand
    {
        private const bool X_PLAYER = true;
        private const bool O_PLAYER = false;
        private Document _dbDoc;
        private UIDocument _uiDoc;
        private UIApplication _uiApp;
        private Application _app;
        Room[] rooms = new Room[9];
        bool?[] board = new bool?[9];


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("Instructions", "The game will start with player X's turn, " +
                                            "click on a room to place your X, then pass the mouse to O. Press Escape to exit.");
            _dbDoc = commandData.Application.ActiveUIDocument.Document;
            _uiDoc = commandData.Application.ActiveUIDocument;
            _uiApp = commandData.Application;
            _app = commandData.Application.Application;
            FilteredElementCollector collector = new FilteredElementCollector(_dbDoc);
            IEnumerable<Element> roomElements = collector.OfCategory(BuiltInCategory.OST_Rooms).ToElements();

            //get the rooms
            foreach (Element element in roomElements)
            {
                Room room = (Room)element;
                if (room.Name.ToLower().Contains("gameroom"))
                {
                    string name = room.Name;
                    int number = int.Parse(name[8].ToString());
                    rooms[number] = room;
                    board[number] = null;
                }
            }

            //loop for each move
            int moveCounter = 0;
            bool gameFinished = false;
            bool currentPlayer = X_PLAYER;

            while (!gameFinished && moveCounter <= 8)
            {
                //get the point
                string displayText = "Player X's turn";
                if (currentPlayer == O_PLAYER)
                    displayText = "Player O's turn";

                try
                {
                    XYZ xyz = _uiDoc.Selection.PickPoint(displayText);

                    if (!ProcessRoom(xyz, rooms, currentPlayer))
                        continue;

                    bool? winner = CheckWinner();
                    if (winner == null)
                    {
                        //increment move counter
                        moveCounter++;
                        //swap player
                        currentPlayer = !currentPlayer;
                        continue;
                    }
                    if (winner == X_PLAYER)
                        TaskDialog.Show("Winner!", "X player wins");
                    if (winner == O_PLAYER)
                        TaskDialog.Show("Winner!", "O player wins");
                }
                catch
                {
                    return Result.Cancelled;
                }
            }
            return Result.Succeeded;
        }

        private bool? CheckWinner()
        {
            const int BOARD_LENGTH = 3;
            for (int i = 0; i < BOARD_LENGTH; i++)
            {
                bool? currentPosition = board[i];

                //if spot is empty, forget about it 
                if (currentPosition == null)
                    continue;

                //if its 0 or board_length -1 check diagonals
                if (i == 0)
                {
                    //check diagonal
                    if (CheckLine(i, BOARD_LENGTH, BOARD_LENGTH + 1))
                        //return the value of the current position
                        return currentPosition.Value;
                }
                if (i == BOARD_LENGTH - 1)
                {
                    //check diagonal
                    if (CheckLine(i, BOARD_LENGTH, BOARD_LENGTH - 1))
                        return currentPosition.Value;
                }
                //if its on line 1, check top to bottom
                if (i < BOARD_LENGTH)
                {
                    //check top to bottom.
                    if (CheckLine(i, BOARD_LENGTH, BOARD_LENGTH))
                        return currentPosition.Value;
                }
                //if its on left column, check left to right 
                if (i == 0 || i % BOARD_LENGTH == 0)
                {
                    //check left to right
                    if (CheckLine(i, BOARD_LENGTH, 1))
                        return currentPosition.Value;
                }
            }
            return null;
        }

        private bool CheckLine(int startPoint, int length, int increment)
        {
            int currentPosition = startPoint;
            bool? searchingFor = board[startPoint];
            if (searchingFor == null)
                return false;

            for (int i = 1; i < length; i++)
            {

                currentPosition += increment;
                if (board[currentPosition] != searchingFor)
                {
                    //doesn't match, line isnt a winner
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check
        /// </summary>
        /// <returns></returns>
        private bool ProcessRoom(XYZ xyz, Room[] rooms, bool currentPlayer)
        {
            //find what room its in
            for (int i = 0; i <= 8; i++)
            {
                //check if point is in room, set the moves to true
                Room currentRoom = rooms[i];
                if (currentRoom.IsPointInRoom(xyz))
                {
                    if (board[i] != null)
                        return false;
                    board[i] = currentPlayer;
                    //mark the room
                    DrawMove(currentPlayer, currentRoom);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Draw the current move 
        /// </summary>
        /// <param name="currentPlayer"></param>
        /// <param name="currentRoom"></param>
        private void DrawMove(bool currentPlayer, Room currentRoom)
        {
            if (currentPlayer == X_PLAYER)
                DrawX(currentRoom);
            else
                DrawO(currentRoom);
        }

        /// <summary>
        /// Draw X in room 
        /// </summary>
        /// <param name="currentRoom"></param>
        private void DrawX(Room currentRoom)
        {
            //mark the room
            LocationPoint point = currentRoom.Location as LocationPoint;

            const int LENGTH = 16;
            XYZ startOfLine = _app.Create.NewXYZ(point.Point.X + LENGTH, point.Point.Y + LENGTH, point.Point.Z);
            XYZ endOfLine = _app.Create.NewXYZ(point.Point.X - LENGTH, point.Point.Y - LENGTH, point.Point.Z);

            Line line = _app.Create.NewLineBound(startOfLine, endOfLine);
            _dbDoc.Create.NewDetailCurve(_dbDoc.ActiveView, line);

            XYZ startOfLine2 = _app.Create.NewXYZ(point.Point.X - LENGTH, point.Point.Y + LENGTH, point.Point.Z);
            XYZ endOfLine2 = _app.Create.NewXYZ(point.Point.X + LENGTH, point.Point.Y - LENGTH, point.Point.Z);

            Line line2 = _app.Create.NewLineBound(startOfLine2, endOfLine2);
            _dbDoc.Create.NewDetailCurve(_dbDoc.ActiveView, line2);
        }

        /// <summary>
        /// Draw O in room
        /// </summary>
        /// <param name="currentRoom"></param>
        private void DrawO(Room currentRoom)
        {
            LocationPoint point = currentRoom.Location as LocationPoint;
            XYZ p = XYZ.Zero;
            XYZ norm = XYZ.BasisZ;
            double startAngle = 0;
            double endAngle = 2 * Math.PI;
            double radius = 16;

            Plane plane = new Plane(norm, point.Point);

            Arc arc = _app.Create.NewArc(
              plane, radius, startAngle, endAngle);

            DetailArc detailArc
              = _dbDoc.Create.NewDetailCurve(
                _dbDoc.ActiveView, arc) as DetailArc;
        }
    }
}
