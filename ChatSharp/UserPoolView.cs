using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatSharp
{
    public class UserPoolView
    {
        private UserPool Pool { get; set; }
        private IEnumerable<IrcUser> Users { get; set; }

        internal UserPoolView(IEnumerable<IrcUser> users)
        {
            Users = users;
        }

        public IrcUser this[string nick]
        {
            get
            {
                var user = Users.FirstOrDefault(u => u.Nick == nick);
                if (user == null)
                    throw new KeyNotFoundException();
                return user;
            }
        }

        internal IrcUser this[int index]
        {
            get
            {
                return Users.ToList()[index];
            }
        }

        public bool ContainsMask(string mask)
        {
            return Users.Any(u => u.Match(mask));
        }

        public bool Contains(string nick)
        {
            return Users.Any(u => u.Nick == nick);
        }

        public bool Contains(IrcUser user)
        {
            return Users.Any(u => u.Hostmask == user.Hostmask);
        }
    }
}

