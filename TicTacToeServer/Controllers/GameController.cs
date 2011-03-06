using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TicTacToeServer.Controllers
{
    /// <summary>
    /// Handles the hosting of the game. 
    /// It's poor practice to have database calls in your controller but 
    /// it's such a small application there is not a great need to have it separated.
    /// </summary>
    public class GameController : Controller
    {
        private TicTacToeDbDataContext _db;
        private const bool X_PLAYER = true;
        private const bool O_PLAYER = false;

        public GameController()
        {
            _db = new TicTacToeDbDataContext();

        }

        /// <summary>
        /// Sets up a new game session
        /// </summary>
        /// <param name="playerId">player starting it, who will become the X player</param>
        /// <returns>the game ID</returns>
        [HttpPost]
        public ActionResult NewGame(string playerId)
        {
            Game game = new Game();
            game.XPlayer = playerId;
            game.Active = true;
            _db.Games.InsertOnSubmit(game);
            _db.SubmitChanges();

            return Content(game.SessionId.ToString());
        }

        /// <summary>
        /// Ends the game, marking it as inactive
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>success</returns>
        [HttpPost]
        public ActionResult EndGame(int sessionId)
        {
            Game game = _db.Games.Where(g => g.SessionId == sessionId).FirstOrDefault();
            game.Active = false;
            return Content("Success");
        }

        /// <summary>
        /// Updates the server with a players move.
        /// </summary>
        /// <param name="sessionId">id of game</param>
        /// <param name="playerId">player</param>
        /// <param name="room">room</param>
        /// <returns>Success</returns>
        [HttpPost]
        public ActionResult DoMove(int sessionId, string playerId, int room)
        {
            //add the move 
            Move move = new Move();
            move.PlayerId = playerId;
            move.SessionId = sessionId;
            move.RoomNumber = room;
            _db.Moves.InsertOnSubmit(move);
            //update last turn on the game
            Game game = _db.Games.Where(g => g.SessionId == sessionId).FirstOrDefault();
            game.LastTurn = room;

            if (_db.Games.GetOriginalEntityState(game) == null)
            {
                _db.Games.Attach(game);
                _db.Games.Context.Refresh(RefreshMode.KeepCurrentValues, game);
            }

            _db.SubmitChanges();
            return Content("Success");
        }


        /// <summary>
        /// Gets whether a player is X or O
        /// </summary>
        /// <param name="sessionId">game</param>
        /// <param name="playerId">player asking for their type</param>
        /// <returns>boolean representing true for X or false for O</returns>
        [HttpPost]
        public ActionResult JoinGame(int sessionId, string playerId)
        {
            Game game = _db.Games.Where(g => g.SessionId == sessionId && g.Active).FirstOrDefault();

            if (game == null)
                return Content("INVALID_GAME");
            //X player is set by initial start of game
            if (game.XPlayer == playerId)
                return Content(X_PLAYER.ToString());

            if (string.IsNullOrEmpty(game.OPlayer))
            {
                //must be second player, add them as O player
                game.OPlayer = playerId;
                if (_db.Games.GetOriginalEntityState(game) == null)
                {
                    _db.Games.Attach(game);
                    _db.Games.Context.Refresh(RefreshMode.KeepCurrentValues, game);
                }
                _db.SubmitChanges();
            }

            if (game.OPlayer == playerId)
                return Content(O_PLAYER.ToString());

            return Content("TOO_MANY_PLAYERS");
        }

        /// <summary>
        /// Gets the latest move
        /// </summary>
        /// <param name="sessionId">game session</param>
        /// <param name="playerId">the player asking for their oppositions move </param>
        /// <returns>room number of players move of -1 if still no move</returns>
        public ActionResult GetLatestMove(int sessionId, string playerId)
        {
            Game game = _db.Games.Where(g => g.SessionId == sessionId).FirstOrDefault();
            Move move = _db.Moves.Where(m => m.SessionId == sessionId && m.RoomNumber == game.LastTurn).FirstOrDefault();
            
            //check that the move was not the currentPlayers
            if (move == null || move.PlayerId == playerId)
            {
                //still waiting on oppositions move return fase
                return Content("-1");
            }

            //must be the oppositions, which is what we weant
            return Content(move.RoomNumber.ToString());
        }
    }
}
