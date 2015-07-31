using System;

namespace ChatSharp.Events
{
    public class ChannelTopicEventArgs : EventArgs
    {
        public IrcChannel Channel { get; set; }
        public string Topic { get; set; }
        public string OldTopic { get; set; }

        public ChannelTopicEventArgs(IrcChannel channel, string oldTopic, string topic)
        {
            Channel = channel;
            Topic = topic;
            OldTopic = oldTopic;
        }
    }
}
