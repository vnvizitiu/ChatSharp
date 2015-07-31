using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatSharp
{
    public class UserPool : IEnumerable<IrcUser>
    {
        private List<IrcUser> Users { get; set; }

        internal UserPool()
        {
            Users = new List<IrcUser>();
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

        internal void Add(IrcUser user)
        {
            if (Users.Any(u => u.Hostmask == user.Hostmask))
                return;
            Users.Add(user);
        }

        internal void Remove(IrcUser user)
        {
            Users.Remove(user);
        }

        internal void Remove(string nick)
        {
            if (Contains(nick))
                Users.Remove(this[nick]);
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

        internal IrcUser GetOrAdd(string prefix)
        {
            var user = new IrcUser(prefix);
            if (Contains(user.Nick))
            {
                var ret = this[user.Nick];
                if (string.IsNullOrEmpty(ret.User) && !string.IsNullOrEmpty(user.User))
                    ret.User = user.User;
                if (string.IsNullOrEmpty(ret.Hostname) && !string.IsNullOrEmpty(user.Hostname))
                    ret.Hostname = user.Hostname;
                return ret;
            }
            Add(user);
            return user;
        }

        internal IrcUser Get(string prefix)
        {
            var user = new IrcUser(prefix);
            if (Contains(user.Nick))
                return this[user.Nick];
            throw new KeyNotFoundException();
        }

        public IEnumerator<IrcUser> GetEnumerator()
        {
            return Users.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}