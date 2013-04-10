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

        public static void HandleMode(IrcClient client, IrcMessage message)
        {
            string target, mode;
            if (message.Command == "MODE")
            {
                target = message.Parameters[0];
                mode = message.Payload.Substring(message.Payload.IndexOf(' ') + 1);
            }
            else
            {
                target = message.Parameters[1];
                mode = message.Payload.Substring(message.Payload.IndexOf(' ') + 1);
                mode = mode.Substring(mode.IndexOf(' ') + 1);
            }

            var eventArgs = new ModeChangeEventArgs(target, new IrcUser(message.Prefix), mode);
            client.OnModeChanged(eventArgs);
            // Handle change
            var change = mode;
            var parameters = new string[0];
            if (change.Contains(' '))
            {
                parameters = change.Substring(change.IndexOf(' ') + 1).Split(' ');
                change = change.Remove(change.IndexOf(' '));
            }
            bool add = change[0] == '+';
            change = change.Substring(1);
            if (target.StartsWith("#"))
            {
                var channel = client.Channels[target];
                int i = 0;
                foreach (char c in change)
                {
                    if (c == 'o')
                    {
                        var user = parameters[i++];
                        if (add)
                            channel.Opped.Add(channel.Users[user]);
                        else
                            channel.Opped.Remove(user);
                    }
                    else if (c == 'v')
                    {
                        var user = parameters[i++];
                        if (add)
                            channel.Voiced.Add(channel.Users[user]);
                        else
                            channel.Voiced.Remove(user);
                    }
                    else if (c == 'b')
                    {
                        var mask = parameters[i++];
                        if (add)
                            channel.Bans.Add(mask);
                        else
                        {
                            if (channel.Bans.Contains(mask))
                                channel.Bans.Remove(mask);
                        }
                    }
                    else if (c == 'e')
                    {
                        var mask = parameters[i++];
                        if (add)
                            channel.Exceptions.Add(mask);
                        else
                        {
                            if (channel.Exceptions.Contains(mask))
                                channel.Exceptions.Remove(mask);
                        }
                    }
                    else if (c == 'I')
                    {
                        var mask = parameters[i++];
                        if (add)
                            channel.Invites.Add(mask);
                        else
                        {
                            if (channel.Invites.Contains(mask))
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
                foreach (char c in change)
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
