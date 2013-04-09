using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatSharp.Handlers
{
    internal static class MessageHandlers
    {
        public static void RegisterDefaultHandlers()
        {
            IrcClient.SetHandler("PING", MessageHandlers.HandlePing);
        }

        public static void HandlePing(IrcClient client, IrcMessage message)
        {
            client.SendRawMessage("PONG :{0}", message.Payload);
        }
    }
}
