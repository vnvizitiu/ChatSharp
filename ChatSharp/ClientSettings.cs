using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatSharp
{
    public class ClientSettings
    {
        public ClientSettings()
        {
            WhoIsOnConnect = true;
            ModeOnJoin = true;
        }

        /// <summary>
        /// If true, the client will WHOIS itself upon joining, which will populate the hostname in
        /// IrcClient.User. This will allow you, for example, to use IrcUser.Match(...) on yourself
        /// to see if you match any masks.
        /// </summary>
        public bool WhoIsOnConnect { get; set; }
        /// <summary>
        /// If true, the client will MODE any channel it joins, populating IrcChannel.Mode. If false,
        /// IrcChannel.Mode will be null until the mode is explicitly requested.
        /// </summary>
        public bool ModeOnJoin { get; set; }
    }
}
