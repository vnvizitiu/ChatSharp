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
    }
}
