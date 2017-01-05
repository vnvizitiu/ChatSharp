using ChatSharp.Events;
using System.Linq;

namespace ChatSharp.Handlers
{
    /// <summary>
    /// </summary>
    internal static class ErrorHandlers
    {
        /// <summary>
        /// </summary>
        public static void HandleError(IrcClient client, IrcMessage message)
        {
//            System.Diagnostics.Trace.WriteLine("HandleNoSuchNick");
            client.OnErrorReplie(new Events.ErrorReplieEventArgs(message));
        }
    }
}
