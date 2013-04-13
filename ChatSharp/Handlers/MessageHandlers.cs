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
            IrcClient.SetHandler("MODE", HandleMode);
            IrcClient.SetHandler("324", HandleMode);
            IrcClient.SetHandler("431", HandleErronousNick);
            IrcClient.SetHandler("432", HandleErronousNick);
            IrcClient.SetHandler("433", HandleErronousNick);
            IrcClient.SetHandler("436", HandleErronousNick);

            // MOTD Handlers
            IrcClient.SetHandler("375", MOTDHandlers.HandleMOTDStart);
            IrcClient.SetHandler("372", MOTDHandlers.HandleMOTD);
            IrcClient.SetHandler("376", MOTDHandlers.HandleEndOfMOTD);

            // Channel handlers
            IrcClient.SetHandler("JOIN", ChannelHandlers.HandleJoin);
            IrcClient.SetHandler("PART", ChannelHandlers.HandlePart);
            IrcClient.SetHandler("353", ChannelHandlers.HandleUserListPart);
            IrcClient.SetHandler("366", ChannelHandlers.HandleUserListEnd);

            // User handlers
            IrcClient.SetHandler("311", UserHandlers.HandleWhoIsUser);
            IrcClient.SetHandler("312", UserHandlers.HandleWhoIsServer);
            IrcClient.SetHandler("313", UserHandlers.HandleWhoIsOperator);
            IrcClient.SetHandler("317", UserHandlers.HandleWhoIsIdle);
            IrcClient.SetHandler("318", UserHandlers.HandleWhoIsEnd);
            IrcClient.SetHandler("319", UserHandlers.HandleWhoIsChannels);

            // Listing handlers
            IrcClient.SetHandler("367", ListingHandlers.HandleBanListPart);
            IrcClient.SetHandler("368", ListingHandlers.HandleBanListEnd);
            IrcClient.SetHandler("348", ListingHandlers.HandleExceptionListPart);
            IrcClient.SetHandler("349", ListingHandlers.HandleExceptionListEnd);
            IrcClient.SetHandler("346", ListingHandlers.HandleInviteListPart);
            IrcClient.SetHandler("347", ListingHandlers.HandleInviteListEnd);
            IrcClient.SetHandler("728", ListingHandlers.HandleQuietListPart);
            IrcClient.SetHandler("729", ListingHandlers.HandleQuietListEnd);
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
                    if (c == 'o')
                    {
                        var user = message.Parameters[i++];
                        if (add)
                            channel.Operators.Add(channel.Users[user]);
                        else
                            channel.Operators.Remove(user);
                    }
                    else if (c == 'v')
                    {
                        var user = message.Parameters[i++];
                        if (add)
                            channel.Voiced.Add(channel.Users[user]);
                        else
                            channel.Voiced.Remove(user);
                    }
                    else if (c == 'b')
                    {
                        var mask = new Mask(message.Parameters[i++], new IrcUser(message.Prefix), DateTime.Now);
                        if (add)
                            channel.Bans.Add(mask);
                        else
                        {
                            if (channel.Bans.ContainsMask(mask))
                                channel.Bans.Remove(mask);
                        }
                    }
                    else if (c == 'e')
                    {
                        var mask = new Mask(message.Parameters[i++], new IrcUser(message.Prefix), DateTime.Now);
                        if (add)
                            channel.Exceptions.Add(mask);
                        else
                        {
                            if (channel.Exceptions.ContainsMask(mask))
                                channel.Exceptions.Remove(mask);
                        }
                    }
                    else if (c == 'q')
                    {
                        var mask = new Mask(message.Parameters[i++], new IrcUser(message.Prefix), DateTime.Now);
                        if (add)
                            channel.Quiets.Add(mask);
                        else
                        {
                            if (channel.Quiets.ContainsMask(mask))
                                channel.Quiets.Remove(mask);
                        }
                    }
                    else if (c == 'I')
                    {
                        var mask = new Mask(message.Parameters[i++], new IrcUser(message.Prefix), DateTime.Now);
                        if (add)
                            channel.Invites.Add(mask);
                        else
                        {
                            if (channel.Invites.ContainsMask(mask))
                                channel.Invites.Remove(mask);
                        }
                    }
                    else
                    {
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
                }
                if (message.Command == "324")
                {
                    var operation = RequestOperation.DequeueOperation("MODE " + channel.Name);
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
