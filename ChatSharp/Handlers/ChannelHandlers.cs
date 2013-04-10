using ChatSharp.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatSharp.Handlers
{
    internal static class ChannelHandlers
    {
        public static void HandleJoin(IrcClient client, IrcMessage message)
        {
            if (client.User.Nick == new IrcUser(message.Prefix).Nick)
            {
                // We've joined this channel
                var channel = new IrcChannel(client, message.Payload);
                client.Channels.Add(channel);
            }
            else
            {
                // Someone has joined a channel we're already in
                client.Channels[message.Payload].Users.Add(new IrcUser(message.Prefix));
            }
            client.OnUserJoinedChannel(new ChannelUserEventArgs(client.Channels[message.Payload], new IrcUser(message.Prefix)));
        }

        public static void HandlePart(IrcClient client, IrcMessage message)
        {
            if (client.User.Match(message.Prefix)) // We've parted this channel
                client.Channels.Remove(client.Channels[message.Parameters[0]]);
            else // Someone has parted a channel we're already in
                client.Channels[message.Parameters[0]].Users.Remove(new IrcUser(message.Prefix).Nick);
            client.OnUserPartedChannel(new ChannelUserEventArgs(client.Channels[message.Parameters[0]], new IrcUser(message.Prefix)));
        }

        public static void HandleUserListPart(IrcClient client, IrcMessage message)
        {
            var channel = client.Channels[message.Parameters[2]];
            var users = message.Parameters[3].Split(' ');
            for (int i = 0; i < users.Length; i++)
            {
                var user = users[i];
                if (user.StartsWith("@"))
                {
                    user = user.Substring(1);
                    channel.Opped.Add(new IrcUser(user));
                }
                if (user.StartsWith("+"))
                {
                    user = user.Substring(1);
                    channel.Voiced.Add(new IrcUser(user));
                }
                channel.Users.Add(new IrcUser(user));
            }
        }

        public static void HandleUserListEnd(IrcClient client, IrcMessage message)
        {
            var channel = client.Channels[message.Parameters[1]];
            client.OnChannelListRecieved(new ChannelEventArgs(channel));
        }
    }
}
