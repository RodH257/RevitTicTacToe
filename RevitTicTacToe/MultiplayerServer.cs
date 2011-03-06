using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Autodesk.Revit.UI;

namespace RevitTicTacToe
{
    public class MultiplayerServer
    {
        //communicator to send REST requests
        private RestfulCommunicator _restfulCommunicator;

        public MultiplayerServer(RestfulCommunicator communicator)
        {
            _restfulCommunicator = communicator;
        }

        /// <summary>
        /// Starts a new session and returns its ID
        /// </summary>
        /// <param name="playerId">player who created the game</param>
        /// <returns></returns>
        internal int StartNewGame(string playerId)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("playerId", playerId.ToString());
            string result = _restfulCommunicator.NewPostRequest("Game/NewGame/", parameters);
            return int.Parse(result);
        }

        /// <summary>
        /// Ends the session
        /// </summary>
        /// <param name="sessionId"></param>
        internal void EndGame(int sessionId)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("sessionId", sessionId.ToString());
            _restfulCommunicator.NewPostRequest("Game/EndGame/", parameters);
        }

        /// <summary>
        /// Gets wether the player is an X or an O
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        internal bool JoinGame(int sessionId, string playerId)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("sessionId", sessionId.ToString());
            parameters.Add("playerId", playerId.ToString());

            string result = _restfulCommunicator.NewPostRequest("Game/JoinGame/", parameters);

            if (result == "INVALID_GAME")
            {
                TaskDialog.Show("Invalid Game", "That game does not exist or has already ended");
                throw new Exception("Invalid Game");
            } else if  (result == "TOO_MANY_PLAYERS")
            {
                TaskDialog.Show("Game Full", "There are already 2 players in that game");
                throw new Exception("Game Full");
            }

            return bool.Parse(result);
        }

        /// <summary>
        /// Sends the move to the server
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="playerId"></param>
        /// <param name="roomEntered"></param>
        internal void SendMove(int sessionId, string playerId, int roomEntered)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("sessionId", sessionId.ToString());
            parameters.Add("playerId", playerId.ToString());
            parameters.Add("room", roomEntered.ToString());

            _restfulCommunicator.NewPostRequest("Game/DoMove/", parameters);
        }

        /// <summary>
        /// Checks for the oppositions move
        /// </summary>
        /// <returns></returns>
        internal int CheckForOppositionsMove(int sessionId, string playerId)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("sessionId", sessionId.ToString());
            parameters.Add("playerId", playerId.ToString());

            return int.Parse(_restfulCommunicator.NewGetRequest("Game/GetLatestMove/", parameters));
        }
    }
}
