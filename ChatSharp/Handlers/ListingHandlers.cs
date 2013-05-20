using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatSharp.Handlers
{
    internal static class ListingHandlers
    {
        public static void HandleBanListPart(IrcClient client, IrcMessage message)
        {
            var parameterString = message.RawMessage.Substring(message.RawMessage.IndexOf(' '));
            var parameters = parameterString.Substring(parameterString.IndexOf(' ')).Split(' ');
            var request = client.RequestManager.PeekOperation("BLIST " + parameters[1]);
            var list = (List<Mask>)request.State;
            list.Add(new Mask(parameters[2], new IrcUser(parameters[3]), IrcClient.DateTimeFromIrcTime(int.Parse(parameters[4]))));
        }

        public static void HandleBanListEnd(IrcClient client, IrcMessage message)
        {
            var request = client.RequestManager.DequeueOperation("BLIST " + message.Parameters[1]);
            if (request.Callback != null)
                request.Callback(request);
        }

        public static void HandleExceptionListPart(IrcClient client, IrcMessage message)
        {
            var parameterString = message.RawMessage.Substring(message.RawMessage.IndexOf(' ') + 1);
            var parameters = parameterString.Substring(parameterString.IndexOf(' ') + 1).Split(' ');
            var request = client.RequestManager.PeekOperation("ELIST " + parameters[1]);
            var list = (List<Mask>)request.State;
            list.Add(new Mask(parameters[2], new IrcUser(parameters[3]), IrcClient.DateTimeFromIrcTime(int.Parse(parameters[4]))));
        }

        public static void HandleExceptionListEnd(IrcClient client, IrcMessage message)
        {
            var request = client.RequestManager.DequeueOperation("ELIST " + message.Parameters[1]);
            if (request.Callback != null)
                request.Callback(request);
        }

        public static void HandleInviteListPart(IrcClient client, IrcMessage message)
        {
            var parameterString = message.RawMessage.Substring(message.RawMessage.IndexOf(' ') + 1);
            var parameters = parameterString.Substring(parameterString.IndexOf(' ') + 1).Split(' ');
            var request = client.RequestManager.PeekOperation("ILIST " + parameters[1]);
            var list = (List<Mask>)request.State;
            list.Add(new Mask(parameters[2], new IrcUser(parameters[3]), IrcClient.DateTimeFromIrcTime(int.Parse(parameters[4]))));
        }

        public static void HandleInviteListEnd(IrcClient client, IrcMessage message)
        {
            var request = client.RequestManager.DequeueOperation("ILIST " + message.Parameters[1]);
            if (request.Callback != null)
                request.Callback(request);
        }

        public static void HandleQuietListPart(IrcClient client, IrcMessage message)
        {
            var parameterString = message.RawMessage.Substring(message.RawMessage.IndexOf(' ') + 1);
            var parameters = parameterString.Substring(parameterString.IndexOf(' ') + 1).Split(' ');
            var request = client.RequestManager.PeekOperation("QLIST " + parameters[1]);
            var list = (List<Mask>)request.State;
            list.Add(new Mask(parameters[2], new IrcUser(parameters[3]), IrcClient.DateTimeFromIrcTime(int.Parse(parameters[4]))));
        }

        public static void HandleQuietListEnd(IrcClient client, IrcMessage message)
        {
            var request = client.RequestManager.DequeueOperation("QLIST " + message.Parameters[1]);
            if (request.Callback != null)
                request.Callback(request);
        }
    }
}
