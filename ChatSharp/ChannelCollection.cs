using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChatSharp
{
    public class ChannelCollection : IEnumerable<IrcChannel>
    {
        internal ChannelCollection()
        {
            Channels = new List<IrcChannel>();
        }

        internal ChannelCollection(IrcClient client) : this()
        {
            Client = client;
        }

        private IrcClient Client { get; set; }
        private List<IrcChannel> Channels { get; set; }

        internal void Add(IrcChannel channel)
        {
            if (Channels.Any(c => c.Name == channel.Name))
                throw new InvalidOperationException("That channel already exists in this collection.");
            Channels.Add(channel);
        }

        internal void Remove(IrcChannel channel)
        {
            Channels.Remove(channel);
        }

        public void Join(string name)
        {
            if (Client != null)
                Client.JoinChannel(name);
            else
                throw new InvalidOperationException("Cannot make other users join channels.");
        }

        public bool Contains(string name)
        {
            return Channels.Any(c => c.Name == name);
        }

        public IrcChannel this[int index]
        {
            get
            {
                return Channels[index];
            }
        }

        public IrcChannel this[string name]
        {
            get
            {
                var channel = Channels.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
                if (channel == null)
                    throw new KeyNotFoundException();
                return channel;
            }
        }

        internal IrcChannel GetOrAdd(string name)
        {
            if (this.Contains(name))
                return this[name];
            var channel = new IrcChannel(Client, name);
            this.Add(channel);
            return channel;
        }

        public IEnumerator<IrcChannel> GetEnumerator()
        {
            return Channels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
