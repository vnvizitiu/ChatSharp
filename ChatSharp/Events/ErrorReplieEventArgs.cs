using System;
using System.Net.Sockets;

namespace ChatSharp.Events
{
    /// <summary>
    /// Raised when a IRC Error replie occurs. See rfc1459 6.1 for details.
    /// </summary>
    public class ErrorReplieEventArgs : EventArgs
    {
        /// <summary>
        /// The IRC error replie that has occured.
        /// </summary>
        public IrcMessage Message { get; set; }

        internal ErrorReplieEventArgs(IrcMessage message)
        {
            Message = message;
        }
    }
}
