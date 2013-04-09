using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using ChatSharp.Events;
using System.Timers;
using ChatSharp.Handlers;

namespace ChatSharp
{
    public class IrcClient
    {
        static IrcClient()
        {
            Handlers = new Dictionary<string, MessageHandler>();
            MessageHandlers.RegisterDefaultHandlers();
        }
        
        public delegate void MessageHandler(IrcClient client, IrcMessage message);
        internal static Dictionary<string, MessageHandler> Handlers { get; set; }
        public static void SetHandler(string message, MessageHandler handler)
        {
            message = message.ToUpper();
            Handlers[message] = handler;
        }

        private const int ReadBufferLength = 1024;

        private byte[] ReadBuffer { get; set; }
        private int ReadBufferIndex { get; set; }
        private string ServerHostname { get; set; }
        private int ServerPort { get; set; }
        private Timer PingTimer { get; set; }

        public string ServerAddress
        {
            get
            {
                return ServerHostname + ":" + ServerPort;
            }
            internal set
            {
                string[] parts = value.Split(':');
                if (parts.Length > 2 || parts.Length == 0)
                    throw new FormatException("Server address is not in correct format ('hostname:port')");
                ServerHostname = parts[0];
                if (parts.Length > 1)
                    ServerPort = int.Parse(parts[1]);
                else
                    ServerPort = 6667;
            }
        }

        public Socket Socket { get; set; }
        public Encoding Encoding { get; set; }
        public IrcUser User { get; set; }

        public IrcClient(string serverAddress, IrcUser user)
        {
            if (serverAddress == null) throw new ArgumentNullException("serverAddress");
            if (user == null) throw new ArgumentNullException("user");

            User = user;
            ServerAddress = serverAddress;
            Encoding = Encoding.UTF8;
        }

        public void ConnectAsync()
        {
            if (Socket != null && Socket.Connected) throw new InvalidOperationException("Socket is already connected to server.");
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ReadBuffer = new byte[ReadBufferLength];
            ReadBufferIndex = 0;
            Socket.BeginConnect(ServerHostname, ServerPort, ConnectComplete, null);
            PingTimer = new Timer(30000);
        }

        private void ConnectComplete(IAsyncResult result)
        {
            Socket.EndConnect(result);
            Socket.BeginReceive(ReadBuffer, ReadBufferIndex, ReadBuffer.Length, SocketFlags.None, DataRecieved, null);
            // Write login info
            if (!string.IsNullOrEmpty(User.Password))
                SendRawMessage("PASS {0}", User.Password);
            SendRawMessage("NICK {0}", User.Nick);
            SendRawMessage("USER {0} 0.0.0.0 server :{1}", User.User, User.RealName);
        }

        private void DataRecieved(IAsyncResult result)
        {
            SocketError error;
            int length = Socket.EndReceive(result, out error) + ReadBufferIndex;
            if (error != SocketError.Success)
            {
                OnNetworkError(new SocketErrorEventArgs(error));
                return;
            }
            ReadBufferIndex = 0;
            while (length > 0)
            {
                int messageLength = Array.IndexOf(ReadBuffer, (byte)'\n', 0, length);
                if (messageLength == -1) // Incomplete message
                {
                    ReadBufferIndex = length;
                    break;
                }
                messageLength++;
                var message = Encoding.GetString(ReadBuffer, 0, messageLength - 2); // -2 to remove \r\n
                HandleMessage(message);
                Array.Copy(ReadBuffer, messageLength, ReadBuffer, 0, length - messageLength);
                length -= messageLength;
            }
            Socket.BeginReceive(ReadBuffer, ReadBufferIndex, ReadBuffer.Length - ReadBufferIndex, SocketFlags.None, DataRecieved, null);
        }

        private void HandleMessage(string rawMessage)
        {
            OnRawMessageRecieved(new RawMessageEventArgs(rawMessage, false));
            var message = new IrcMessage(rawMessage);
            if (Handlers.ContainsKey(message.Command.ToUpper()))
                Handlers[message.Command.ToUpper()](this, message);
            else
            {
                // TODO: Fire an event or something
            }
        }

        public void SendRawMessage(string message, params object[] format)
        {
            message = string.Format(message, format);
            var data = Encoding.GetBytes(message + "\r\n");
            Socket.BeginSend(data, 0, data.Length, SocketFlags.None, MessageSent, message);
        }

        public void SendMessage(IrcMessage message)
        {
            SendRawMessage(message.RawMessage);
        }

        private void MessageSent(IAsyncResult result)
        {
            SocketError error;
            Socket.EndSend(result, out error);
            if (error != SocketError.Success)
                OnNetworkError(new SocketErrorEventArgs(error));
            else
                OnRawMessageSent(new RawMessageEventArgs((string)result.AsyncState, true));
        }

        public event EventHandler<SocketErrorEventArgs> NetworkError;
        protected internal virtual void OnNetworkError(SocketErrorEventArgs e)
        {
            if (NetworkError != null) NetworkError(this, e);
        }
        public event EventHandler<RawMessageEventArgs> RawMessageSent;
        protected internal virtual void OnRawMessageSent(RawMessageEventArgs e)
        {
            if (RawMessageSent != null) RawMessageSent(this, e);
        }
        public event EventHandler<RawMessageEventArgs> RawMessageRecieved;
        protected internal virtual void OnRawMessageRecieved(RawMessageEventArgs e)
        {
            if (RawMessageRecieved != null) RawMessageRecieved(this, e);
        }
    }
}
