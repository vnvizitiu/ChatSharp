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
    }
}
