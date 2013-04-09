using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatSharp
{
    public class IrcUser
    {
        public IrcUser(string host)
        {
            string[] mask = host.Split('@', '!');
            Nick = mask[0];
            User = mask[1];
            Hostname = mask[2];
        }

        public IrcUser(string nick, string user)
        {
            Nick = nick;
            User = user;
            RealName = User;
        }

        public IrcUser(string nick, string user, string password) : this(nick, user)
        {
            Password = password;
        }

        public IrcUser(string nick, string user, string password, string realName) : this(nick, user, password)
        {
            RealName = realName;
        }

        public string Nick { get; internal set; }
        public string User { get; internal set; }
        public string Password { get; internal set; }
        public string RealName { get; internal set; }
        public string Hostname { get; internal set; }

        public string Hostmask
        {
            get
            {
                return Nick + "!" + User + "@" + Hostname;
            }
        }
    }
}
