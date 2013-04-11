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
            var client = new IrcClient("irc.freenode.net", new IrcUser("ChatSharp", "ChatSharp"));
            client.NetworkError += (s, e) => Console.WriteLine("Error: " + e.SocketError);
            client.RawMessageRecieved += (s, e) => Console.WriteLine("<< {0}", e.Message);
            client.RawMessageSent += (s, e) => Console.WriteLine(">> {0}", e.Message);
            client.UserMessageRecieved += (s, e) =>
                {
                    if (e.PrivateMessage.Message.StartsWith(".join "))
                        client.Channels.Join(e.PrivateMessage.Message.Substring(6));
                    else if (e.PrivateMessage.Message.StartsWith(".list "))
                    {
                        var channel = client.Channels[e.PrivateMessage.Message.Substring(6)];
                        var list = channel.Users.Select(u => u.Nick).Aggregate((a, b) => a + "," + b);
                        client.SendMessage(list, e.PrivateMessage.User.Nick);
                    }
                    else if (e.PrivateMessage.Message.StartsWith(".whois "))
                        client.WhoIs(e.PrivateMessage.Message.Substring(7), null);
                    else if (e.PrivateMessage.Message.StartsWith(".raw "))
                        client.SendRawMessage(e.PrivateMessage.Message.Substring(5));
                    else if (e.PrivateMessage.Message.StartsWith(".bans "))
                    {
                        client.GetBanList(e.PrivateMessage.Message.Substring(6), bans =>
                            {
                                client.SendMessage(string.Join(",", bans.Select(b => 
                                    string.Format("{0} by {1} at {2}", b.Value, b.Creator, b.CreationTime)
                                    ).ToArray()), e.PrivateMessage.User.Nick);
                            });
                    }
                    else if (e.PrivateMessage.Message.StartsWith(".exceptions "))
                    {
                        client.GetExceptionList(e.PrivateMessage.Message.Substring(12), exceptions =>
                        {
                            client.SendMessage(string.Join(",", exceptions.Select(ex =>
                                    string.Format("{0} by {1} at {2}", ex.Value, ex.Creator, ex.CreationTime)
                                    ).ToArray()), e.PrivateMessage.User.Nick);
                        });
                    }
                };
            client.ChannelMessageRecieved += (s, e) =>
                {
                    Console.WriteLine("<{0}> {1}", e.PrivateMessage.User.Nick, e.PrivateMessage.Message);
                };
            client.ConnectAsync();
            while (true) ;
        }
    }
}
