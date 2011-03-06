using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;

namespace RevitTicTacToe
{
    /// <summary>
    /// Manages the game itself
    /// </summary>
    public class BoardManager
    {
        //rooms used as squares
        Room[] _rooms = new Room[9];
        //actual game board 
        bool?[] _board = new bool?[9];
        //line drawer to fill in X's and O's
        private LineDrawer _lineDrawer;
        private Document _dbDoc;

        public BoardManager(Document dbDoc)
        {
            _dbDoc = dbDoc;
            _lineDrawer = new LineDrawer(_dbDoc.Application, _dbDoc);
            GetRooms();

        }

        /// <summary>
        /// Gets all the rooms from the document, setting up the board
        /// </summary>
        private void GetRooms()
        {
            FilteredElementCollector collector = new FilteredElementCollector(_dbDoc);
            IEnumerable<Element> roomElements = collector.OfCategory(BuiltInCategory.OST_Rooms).ToElements();
            foreach (Element element in roomElements)
            {
                Room room = (Room)element;
                if (room.Name.ToLower().Contains("gameroom"))
                {
                    string name = room.Name;
                    int number = int.Parse(name[8].ToString());
                    _rooms[number] = room;
                    _board[number] = null;
                }
            }
        }

        /// <summary>
        /// Process the room after the user clicked on it
        /// </summary>
        /// <param name="xyz"></param>
        /// <param name="currentPlayer"></param>
        /// <returns></returns>
        internal int ProcessRoom(XYZ xyz, bool currentPlayer)
        {
            Transaction transaction = new Transaction(_dbDoc);
            transaction.Start("Move");
            //find what room its in)
            for (int i = 0; i <= 8; i++)
            {
                //check if point is in room, set the moves to true
                Room currentRoom = _rooms[i];
                if (currentRoom.IsPointInRoom(xyz))
                {
                    if (_board[i] != null)
                        return -1;
                    _board[i] = currentPlayer;
                    //mark the room
                    _lineDrawer.DrawMove(currentPlayer, currentRoom);
                    transaction.Commit();
                    return i;
                }
            }
            transaction.RollBack();
            return -1;
        }


        /// <summary>
        /// Process room via integer - for online play
        /// </summary>
        /// <param name="turn"></param>
        /// <param name="player"></param>
        internal void ProcessRoom(int turn, bool player)
        {
            Transaction transaction = new Transaction(_dbDoc);
            transaction.Start("Move");
            _board[turn] = player;
            //mark the room
            Room currentRoom = _rooms[turn];
            _lineDrawer.DrawMove(player, currentRoom);
            transaction.Commit();
        }


        /// <summary>
        /// Clears naughts and crosses
        /// </summary>
        public void ClearBoard()
        {
            Transaction transaction = new Transaction(_dbDoc);
            transaction.Start("Board Clear");
            FilteredElementCollector collector = new FilteredElementCollector(_dbDoc);
            foreach (var element in collector.OfCategory(BuiltInCategory.OST_Lines))
            {
                _dbDoc.Delete(element);
            }
            transaction.Commit();
        }

        /// <summary>
        /// Checks for a winner, returns the bool value for the winner or null if none yet
        /// </summary>
        /// <returns></returns>
        public bool? CheckWinner()
        {
            const int BOARD_LENGTH = 3;
            for (int i = 0; i < BOARD_LENGTH * BOARD_LENGTH; i++)
            {
                bool? currentPosition = _board[i];

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

        /// <summary>
        /// Checks a certain line of posible victory
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="length"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        private bool CheckLine(int startPoint, int length, int increment)
        {
            int currentPosition = startPoint;
            bool? searchingFor = _board[startPoint];
            //if no entry in first point, dont bother checking more 
            if (searchingFor == null)
                return false;

            //check each square
            for (int i = 1; i < length; i++)
            {
                //get the position of current square 
                currentPosition += increment;
                //check its the same value as the first one 
                if (_board[currentPosition] != searchingFor)
                {
                    //doesn't match, line isnt a winner
                    return false;
                }
            }
            return true;
        }



    }
}
