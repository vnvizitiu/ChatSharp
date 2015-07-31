using System.Collections.Generic;
using System.Linq;

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
        public UserPoolView Users { get; private set; }
        public Dictionary<char?, UserPoolView> UsersByMode { get; set; }

        internal IrcChannel(IrcClient client, string name)
        {
            Client = client;
            Name = name;
            Users = new UserPoolView(client.Users.Where(u => u.Channels.Contains(this)));
        }

        public void Invite(string nick)
        {
            Client.InviteUser(Name, nick);
        }

        public void Kick(string nick)
        {
            Client.KickUser(Name, nick);
        }

        public void Kick(string nick, string reason)
        {
            Client.KickUser(Name, nick, reason);
        }

        public void Part()
        {
            Client.PartChannel(Name);
        }

        public void Part(string reason)
        {
            Client.PartChannel(Name); // TODO
        }

        public void SendMessage(string message)
        {
            Client.SendMessage(message, Name);
        }

        public void ChangeMode(string change)
        {
            Client.ChangeMode(Name, change);
        }
    }
}
