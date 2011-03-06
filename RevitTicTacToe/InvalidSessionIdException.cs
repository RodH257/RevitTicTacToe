using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevitTicTacToe
{
    public class InvalidSessionIdException : Exception
    {
        public InvalidSessionIdException(string message): base(message)
        {
            
        }
    }
}
