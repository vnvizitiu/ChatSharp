using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatSharp;
using ChatSharp.Events;

namespace TestChatSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new IrcClient("irc.freenode.net", new IrcUser("SirCmpwnBot", "SirCmpwnBot"));
            client.NetworkError += (s, e) => Console.WriteLine("Error: " + e.SocketError);
            client.RawMessageRecieved += (s, e) => Console.WriteLine("<< {0}", e.Message);
            client.RawMessageSent += (s, e) => Console.WriteLine(">> {0}", e.Message);
            client.ConnectAsync();
            while (true) ;
        }
    }
}
