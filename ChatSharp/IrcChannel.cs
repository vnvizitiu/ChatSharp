using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatSharp
{
    public class IrcChannel
    {
        private IrcClient Client { get; set; }

        internal string _Topic;
        public string Topic 
        {
            get
            {
                return _Topic;
            }
            set
            {
                Client.SetTopic(Name, value);
                _Topic = value;
            }
        }

        public string Name { get; internal set; }
        public string Mode { get; internal set; }
        public UserCollection Users { get; private set; }
        public UserCollection Operators { get; private set; }
        public UserCollection Voiced { get; private set; }
        public MaskCollection Bans { get; private set; }
        public MaskCollection Exceptions { get; private set; }
        public MaskCollection Quiets { get; private set; }
        public MaskCollection Invites { get; private set; }

        internal IrcChannel(IrcClient client, string name)
        {
            Client = client;
            Users = new UserCollection();
            Operators = new UserCollection();
            Voiced = new UserCollection();
            Bans = new MaskCollection();
            Exceptions = new MaskCollection();
            Quiets = new MaskCollection();
            Invites = new MaskCollection();
            Name = name;
        }

        public void Part()
        {
            Client.PartChannel(Name);
        }

        public void Part(string reason)
        {
            Client.PartChannel(Name);
        }

        public void GetBanList(Action<IrcChannel> callback)
        {
            Client.GetBanList(Name, bans =>
                {
                    if (callback != null)
                        callback(this);
                });
        }

        public void GetExceptionList(Action<IrcChannel> callback)
        {
            Client.GetExceptionList(Name, exceptions =>
            {
                if (callback != null)
                    callback(this);
            });
        }

        public void GetQuietList(Action<IrcChannel> callback)
        {
            Client.GetQuietList(Name, quiets =>
            {
                if (callback != null)
                    callback(this);
            });
        }

        public void GetInviteList(Action<IrcChannel> callback)
        {
            Client.GetInviteList(Name, invites =>
            {
                if (callback != null)
                    callback(this);
            });
        }

        public void SendMessage(string message)
        {
            Client.SendMessage(message, Name);
        }

        public void Ban(string mask)
        {
            Client.SendRawMessage("MODE {0} +b {1}", Name, mask);
        }

        public void Ban(IrcUser user)
        {
            Client.SendRawMessage("MODE {0} +b *!*@{1}", Name, user.Hostname);
        }

        public void Unban(IrcUser user)
        {
            GetBanList(c =>
            {
                if (Bans.ContainsMatch(user))
                    Client.SendRawMessage("MODE {0} -b {1}", Name, Quiets.GetMatch(user));
            });
        }

        public void Quiet(string mask)
        {
            Client.SendRawMessage("MODE {0} +q {1}", Name, mask);
        }

        public void Quiet(IrcUser user)
        {
            Client.SendRawMessage("MODE {0} +q *!*@{1}", Name, user.Hostname);
        }

        public void Unquiet(IrcUser user)
        {
            GetQuietList(c =>
                {
                    if (Quiets.ContainsMatch(user))
                        Client.SendRawMessage("MODE {0} -q {1}", Name, Quiets.GetMatch(user));
                });
        }
    }
}
