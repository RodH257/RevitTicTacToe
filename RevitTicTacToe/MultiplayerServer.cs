using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

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
        /// <returns></returns>
        internal int StartNewGame()
        {
            string result = _restfulCommunicator.NewPostRequest("Game/NewGame/", new Dictionary<string, string>());
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
        internal bool GetPlayerType(int sessionId, Guid playerId)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("sessionId", sessionId.ToString());
            parameters.Add("playerId", playerId.ToString());

            return bool.Parse(_restfulCommunicator.NewPostRequest("Game/GetPlayerType/", parameters));
        }

        /// <summary>
        /// Sends the move to the server
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="playerId"></param>
        /// <param name="roomEntered"></param>
        internal void SendMove(int sessionId, Guid playerId, int roomEntered)
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
        internal int CheckForOppositionsMove(int sessionId, Guid playerId)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("sessionId", sessionId.ToString());
            parameters.Add("playerId", playerId.ToString());

            return int.Parse(_restfulCommunicator.NewPostRequest("Game/GetLatestMove/", parameters));
        }
    }
}
