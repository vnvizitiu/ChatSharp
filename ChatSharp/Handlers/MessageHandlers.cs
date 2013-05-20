using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatSharp.Events;

namespace ChatSharp.Handlers
{
    internal static class MessageHandlers
    {
        public static void RegisterDefaultHandlers(IrcClient client)
        {
            // General
            client.SetHandler("PING", HandlePing);
            client.SetHandler("NOTICE", HandleNotice);
            client.SetHandler("PRIVMSG", HandlePrivmsg);
            client.SetHandler("MODE", HandleMode);
            client.SetHandler("324", HandleMode);
            client.SetHandler("431", HandleErronousNick);
            client.SetHandler("432", HandleErronousNick);
            client.SetHandler("433", HandleErronousNick);
            client.SetHandler("436", HandleErronousNick);

            // MOTD Handlers
            client.SetHandler("375", MOTDHandlers.HandleMOTDStart);
            client.SetHandler("372", MOTDHandlers.HandleMOTD);
            client.SetHandler("376", MOTDHandlers.HandleEndOfMOTD);

            // Channel handlers
            client.SetHandler("JOIN", ChannelHandlers.HandleJoin);
            client.SetHandler("PART", ChannelHandlers.HandlePart);
            client.SetHandler("353", ChannelHandlers.HandleUserListPart);
            client.SetHandler("366", ChannelHandlers.HandleUserListEnd);

            // User handlers
            client.SetHandler("311", UserHandlers.HandleWhoIsUser);
            client.SetHandler("312", UserHandlers.HandleWhoIsServer);
            client.SetHandler("313", UserHandlers.HandleWhoIsOperator);
            client.SetHandler("317", UserHandlers.HandleWhoIsIdle);
            client.SetHandler("318", UserHandlers.HandleWhoIsEnd);
            client.SetHandler("319", UserHandlers.HandleWhoIsChannels);

            // Listing handlers
            client.SetHandler("367", ListingHandlers.HandleBanListPart);
            client.SetHandler("368", ListingHandlers.HandleBanListEnd);
            client.SetHandler("348", ListingHandlers.HandleExceptionListPart);
            client.SetHandler("349", ListingHandlers.HandleExceptionListEnd);
            client.SetHandler("346", ListingHandlers.HandleInviteListPart);
            client.SetHandler("347", ListingHandlers.HandleInviteListEnd);
            client.SetHandler("728", ListingHandlers.HandleQuietListPart);
            client.SetHandler("729", ListingHandlers.HandleQuietListEnd);

            // Server handlers
            client.SetHandler("005", ServerHandlers.HandleISupport);
        }

        public static void HandlePing(IrcClient client, IrcMessage message)
        {
            client.SendRawMessage("PONG :{0}", message.Parameters[0]);
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

        public static void HandleMode(IrcClient client, IrcMessage message)
        {
            string target, mode = null;
            int i = 2;
            if (message.Command == "MODE")
            {
                target = message.Parameters[0];
                mode = message.Parameters[1];
            }
            else
            {
                target = message.Parameters[1];
                mode = message.Parameters[2];
                i++;
            }

            var eventArgs = new ModeChangeEventArgs(target, new IrcUser(message.Prefix), mode);
            client.OnModeChanged(eventArgs);
            // Handle change
            bool add = mode[0] == '+';
            mode = mode.Substring(1);
            if (target.StartsWith("#"))
            {
                var channel = client.Channels[target];
                foreach (char c in mode)
                {
                    // TODO: Handle other types of MODEs
                    if (channel.Mode == null)
                        channel.Mode = string.Empty;
                    if (add)
                    {
                        if (!channel.Mode.Contains(c))
                            channel.Mode += c.ToString();
                    }
                    else
                        channel.Mode = channel.Mode.Replace(c.ToString(), string.Empty);
                }
                if (message.Command == "324")
                {
                    var operation = client.RequestManager.DequeueOperation("MODE " + channel.Name);
                    operation.Callback(operation);
                }
            }
            else
            {
                // TODO: Handle user modes other than ourselves?
                foreach (char c in mode)
                {
                    if (add)
                    {
                        if (!client.User.Mode.Contains(c))
                            client.User.Mode += c;
                    }
                    else
                        client.User.Mode = client.User.Mode.Replace(c.ToString(), string.Empty);
                }
            }
        }
    }
}
