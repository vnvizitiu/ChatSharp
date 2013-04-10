using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatSharp
{
    public class UserCollection : IEnumerable<IrcUser>
    {
        internal UserCollection()
        {
            Users = new List<IrcUser>();
        }
        
        private List<IrcUser> Users { get; set; }

        internal void Add(IrcUser user)
        {
            if (Users.Any(u => u.Hostmask == user.Hostmask))
                throw new InvalidOperationException("That user already exists in this collection.");
            Users.Add(user);
        }

        internal void Remove(IrcUser user)
        {
            Users.Remove(user);
        }

        internal void Remove(string nick)
        {
            Users.Remove(this[nick]);
        }

        public IrcUser this[int index]
        {
            get
            {
                return Users[index];
            }
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

        public IEnumerator<IrcUser> GetEnumerator()
        {
            return Users.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
