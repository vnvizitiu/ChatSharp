using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatSharp.Events;

namespace ChatSharp.Handlers
{
    internal static class MessageHandlers
    {
        public static void RegisterDefaultHandlers()
        {
            // General
            IrcClient.SetHandler("PING", HandlePing);
            IrcClient.SetHandler("NOTICE", HandleNotice);
            IrcClient.SetHandler("PRIVMSG", HandlePrivmsg);
            IrcClient.SetHandler("431", HandleErronousNick);
            IrcClient.SetHandler("432", HandleErronousNick);
            IrcClient.SetHandler("433", HandleErronousNick);
            IrcClient.SetHandler("436", HandleErronousNick);

            // MOTD Handlers
            IrcClient.SetHandler("375", MOTDHandlers.HandleMOTDStart);
            IrcClient.SetHandler("372", MOTDHandlers.HandleMOTD);
            IrcClient.SetHandler("376", MOTDHandlers.HandleEndOfMOTD);
        }

        public static void HandlePing(IrcClient client, IrcMessage message)
        {
            client.SendRawMessage("PONG :{0}", message.Payload);
        }

        public static void HandleNotice(IrcClient client, IrcMessage message)
        {
            client.OnNoticeRecieved(new IrcNoticeEventArgs(message));
        }

        public static void HandlePrivmsg(IrcClient client, IrcMessage message)
        {
            var eventArgs = new PrivateMessageEventArgs(message);
            client.OnPrivateMessageRecieved(eventArgs);
            if (eventArgs.PrivateMessage.IsChannelMessage)
                client.OnChannelMessageRecieved(eventArgs);
            else
                client.OnUserMessageRecieved(eventArgs);
        }

        public static void HandleErronousNick(IrcClient client, IrcMessage message)
        {
            var eventArgs = new ErronousNickEventArgs(client.User.Nick);
            if (message.Command == "433") // Nick in use
                client.OnNickInUse(eventArgs);
            // else ... TODO
            if (!eventArgs.DoNotHandle)
                client.Nick(eventArgs.NewNick);
        }
    }
}
