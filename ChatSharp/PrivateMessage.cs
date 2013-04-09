using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatSharp
{
    public class PrivateMessage
    {
        public PrivateMessage(IrcMessage message)
        {
            Medium = message.Payload.Remove(message.Payload.IndexOf(' '));
            Message = message.Payload.Substring(message.Payload.IndexOf(':') + 1);

            User = new IrcUser(message.Prefix);
            if (Medium.StartsWith("#"))
                IsChannelMessage = true;
            else
                Medium = User.Nick;
        }

        public IrcUser User { get; set; }
        public string Message { get; set; }
        public string Medium { get; set; }
        public bool IsChannelMessage { get; set; }
    }
}
