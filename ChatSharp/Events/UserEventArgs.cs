using System;

namespace ChatSharp.Events
{
    public class UserEventArgs : EventArgs
    {
        public IrcUser User { get; set; }

        public UserEventArgs(IrcUser user)
        {
            User = user;
        }
    }
}

