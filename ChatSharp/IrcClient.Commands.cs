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

        public void JoinChannel(string channel)
        {
            if (Channels.Contains(channel))
                throw new InvalidOperationException("Client is not already present in channel.");
            SendRawMessage("JOIN {0}", channel);
        }

        public void SetTopic(string channel, string topic)
        {
            if (!Channels.Contains(channel))
                throw new InvalidOperationException("Client is not present in channel.");
            SendRawMessage("TOPIC {0} :{1}", channel, topic);
        }

        public void WhoIs(string nick, Action<WhoIs> callback)
        {
            var whois = new WhoIs();
            RequestOperation.QueueOperation("WHOIS " + nick, new RequestOperation(whois, ro =>
                {
                    if (callback != null)
                        callback((WhoIs)ro.State);
                }));
            SendRawMessage("WHOIS {0}", nick);
        }

        public void GetMode(string channel, Action<IrcChannel> callback)
        {
            RequestOperation.QueueOperation("MODE " + channel, new RequestOperation(channel, ro =>
                {
                    var c = Channels[(string)ro.State];
                    if (callback != null)
                        callback(c);
                }));
            SendRawMessage("MODE {0}", channel);
        }

        public void GetBanList(string channel, Action<Mask[]> callback)
        {
            RequestOperation.QueueOperation("BLIST " + channel, new RequestOperation(new List<Mask>(), ro =>
                {
                    var bans = ro.State as List<Mask>;
                    if (Channels.Contains(channel))
                    {
                        // Update channel list
                        foreach (var ban in bans)
                            Channels[channel].Bans.Add(ban);
                    }
                    if (callback != null)
                        callback(bans.ToArray());
                }));
            SendRawMessage("MODE {0} b", channel);
        }

        public void GetExceptionList(string channel, Action<Mask[]> callback)
        {
            RequestOperation.QueueOperation("ELIST " + channel, new RequestOperation(new List<Mask>(), ro =>
            {
                var exceptions = ro.State as List<Mask>;
                if (Channels.Contains(channel))
                {
                    // Update channel list
                    foreach (var ex in exceptions)
                        Channels[channel].Exceptions.Add(ex);
                }
                if (callback != null)
                    callback(exceptions.ToArray());
            }));
            SendRawMessage("MODE {0} e", channel);
        }

        public void GetInviteList(string channel, Action<Mask[]> callback)
        {
            RequestOperation.QueueOperation("ILIST " + channel, new RequestOperation(new List<Mask>(), ro =>
            {
                var invites = ro.State as List<Mask>;
                if (Channels.Contains(channel))
                {
                    // Update channel list
                    foreach (var i in invites)
                        Channels[channel].Invites.Add(i);
                }
                if (callback != null)
                    callback(invites.ToArray());
            }));
            SendRawMessage("MODE {0} I", channel);
        }

        public void GetQuietList(string channel, Action<Mask[]> callback)
        {
            RequestOperation.QueueOperation("QLIST " + channel, new RequestOperation(new List<Mask>(), ro =>
            {
                var quiets = ro.State as List<Mask>;
                if (Channels.Contains(channel))
                {
                    // Update channel list
                    foreach (var q in quiets)
                        Channels[channel].Quiets.Add(q);
                }
                if (callback != null)
                    callback(quiets.ToArray());
            }));
            SendRawMessage("MODE {0} q", channel);
        }
    }
}
