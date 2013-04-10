using ChatSharp.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatSharp
{
    public partial class IrcClient
    {
        public void Nick(string newNick)
        {
            SendRawMessage("NICK {0}", newNick);
            User.Nick = newNick;
        }

        public void SendMessage(string message, params string[] destinations)
        {
            if (!destinations.Any()) throw new InvalidOperationException("Message must have at least one target.");
            string to = string.Join(",", destinations);
            SendRawMessage("PRIVMSG {0} :{1}", to, message);
        }

        public void PartChannel(string channel)
        {
            if (!Channels.Contains(channel))
                throw new InvalidOperationException("Client is not present in channel.");
            SendRawMessage("PART {0}", channel);
            Channels.Remove(Channels[channel]);
        }

        public void PartChannel(string channel, string reason)
        {
            if (!Channels.Contains(channel))
                throw new InvalidOperationException("Client is not present in channel.");
            SendRawMessage("PART {0} :{1}", channel, reason);
            Channels.Remove(Channels[channel]);
        }

        public void JoinChannel(string name)
        {
            if (Channels.Contains(name))
                throw new InvalidOperationException("Client is not already present in channel.");
            SendRawMessage("JOIN {0}", name);
        }

        public void SetTopic(string name, string topic)
        {
            if (!Channels.Contains(name))
                throw new InvalidOperationException("Client is not present in channel.");
            SendRawMessage("TOPIC {0} :{1}", name, topic);
        }

        public void WhoIs(string nick, Action<WhoIs> callback)
        {
            var whois = new WhoIs(nick);
            whois.Callback = callback;
            UserHandlers.PendingWhoIs.Add(whois);
            SendRawMessage("WHOIS {0}", nick);
        }
    }
}
