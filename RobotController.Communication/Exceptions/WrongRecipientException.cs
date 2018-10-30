using System;

namespace RobotController.Communication.Exceptions
{
    public class WrongRecipientException : Exception
    {
        public WrongRecipientException()
        {
        }

        public WrongRecipientException(string message)
            : base(message)
        {
        }

        public WrongRecipientException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
