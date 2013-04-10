using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatSharp.Handlers
{
    public static class UserHandlers
    {
        static UserHandlers()
        {
            PendingWhoIs = new List<WhoIs>();
        }

        public static List<WhoIs> PendingWhoIs { get; set; }

        private static WhoIs GetWhoIs(string nick)
        {
            return PendingWhoIs.FirstOrDefault(w => w.Nick == nick);
        }

        public static void HandleWhoIsUser(IrcClient client, IrcMessage message)
        {
            var whois = GetWhoIs(message.Parameters[1]);
            whois.User.Nick = message.Parameters[1];
            whois.User.User = message.Parameters[2];
            whois.User.Hostname = message.Parameters[3];
            whois.User.RealName = message.Parameters[5];
        }

        public static void HandleWhoIsServer(IrcClient client, IrcMessage message)
        {
            var whois = GetWhoIs(message.Parameters[1]);
            whois.Server = message.Parameters[2];
            whois.ServerInfo = message.Parameters[3];
        }

        public static void HandleWhoIsOperator(IrcClient client, IrcMessage message)
        {
            var whois = GetWhoIs(message.Parameters[1]);
            whois.IrcOp = true;
        }

        public static void HandleWhoIsIdle(IrcClient client, IrcMessage message)
        {
            var whois = GetWhoIs(message.Parameters[1]);
            whois.SecondsIdle = int.Parse(message.Parameters[2]);
        }

        public static void HandleWhoIsChannels(IrcClient client, IrcMessage message)
        {
            var whois = GetWhoIs(message.Parameters[1]);
            var channels = message.Parameters[2].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < channels.Length; i++)
                if (!channels[i].StartsWith("#"))
                    channels[i] = channels[i].Substring(1);
            whois.Channels = whois.Channels.Concat(channels).ToArray();
        }

        public static void HandleWhoIsEnd(IrcClient client, IrcMessage message)
        {
            var whois = GetWhoIs(message.Parameters[1]);
            PendingWhoIs.Remove(whois);
            if (whois.Callback != null)
                whois.Callback(whois);
        }
    }
}
