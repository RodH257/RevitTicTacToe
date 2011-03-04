using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace RevitTicTacToe
{
    /// <summary>
    /// Draws the lines on the boards
    /// </summary>
    public class LineDrawer
    {
        const int SQUARE_SIZE = 16;
        private Document _dbDoc;
        private Application _app;

        public LineDrawer(Application app, Document doc)
        {
            _app = app;
            _dbDoc = doc;
        }

        /// <summary>
        /// Draw the current move 
        /// </summary>
        /// <param name="currentPlayer"></param>
        /// <param name="currentRoom"></param>
        public void DrawMove(bool currentPlayer, Room currentRoom)
        {
            if (currentPlayer == ScoreKeeper.X_PLAYER)
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
            LocationPoint point = currentRoom.Location as LocationPoint;

            //construct the first line 
            XYZ startOfLine = _app.Create.NewXYZ(point.Point.X + SQUARE_SIZE, point.Point.Y + SQUARE_SIZE, point.Point.Z);
            XYZ endOfLine = _app.Create.NewXYZ(point.Point.X - SQUARE_SIZE, point.Point.Y - SQUARE_SIZE, point.Point.Z);

            Line line = _app.Create.NewLineBound(startOfLine, endOfLine);
            _dbDoc.Create.NewDetailCurve(_dbDoc.ActiveView, line);

            //construct the second line 
            XYZ startOfLine2 = _app.Create.NewXYZ(point.Point.X - SQUARE_SIZE, point.Point.Y + SQUARE_SIZE, point.Point.Z);
            XYZ endOfLine2 = _app.Create.NewXYZ(point.Point.X + SQUARE_SIZE, point.Point.Y - SQUARE_SIZE, point.Point.Z);

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

            //construct a plane 
            XYZ p = XYZ.Zero;
            XYZ norm = XYZ.BasisZ;
            double startAngle = 0;
            double endAngle = 2 * Math.PI;
            double radius = SQUARE_SIZE;

            Plane plane = new Plane(norm, point.Point);

            //draw a circle
            Arc arc = _app.Create.NewArc(
              plane, radius, startAngle, endAngle);

            //write the circle to the document
            _dbDoc.Create.NewDetailCurve(
                _dbDoc.ActiveView, arc);
        }

    }
}
