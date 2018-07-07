using Superbar.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superbar
{
    public class MessageEntry : IMessageEntry
    {
        public MessageEntry(Type type, String name)
        {
            Type = type;
            FriendlyName = name;
        }

        public Type Type { get; }
        public String FriendlyName { get; }
    }

    public class Message : IMessage
    {
        public Message(Object o, IMessageEntry entry)
        {
            Object = o;
            MessageEntry = entry;
        }

        public Object Object { get; }
        public IMessageEntry MessageEntry { get; }
    }

    public class ConfigurationEntry : IConfigurationEntry
    {
        public ConfigurationEntry(Object obj, String name)
        {
            Object = obj;
            FriendlyName = name;
        }

        public Object Object { get; }

        public String FriendlyName { get; }
    }
}
