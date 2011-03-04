using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TicTacToeServer.Controllers
{
    public class GameController : Controller
    {
        //
        // GET: /Game/
        [HttpPost]
        public ActionResult NewGame()
        {
            return Content("Test");
        }

        public ActionResult CheckForMove(int gameId)
        {
            return Content("None");
        }
    }
}
