using System;
using System.Reflection;

namespace NETMF.OpenSource.XBee.Api
{
    public class PacketTypeFilter : IPacketFilter
    {
        private readonly Type _expectedType;
        
        public PacketTypeFilter(Type expectedType)
        {
            if (expectedType == null)
                throw new ArgumentException("expectedType needs to be specified");

            _expectedType = expectedType;
        }

        public virtual bool Accepted(XBeeResponse packet)
        {
#if WINDOWS_UWP
            return packet.GetType() == _expectedType || packet.GetType().GetTypeInfo().IsSubclassOf(_expectedType);
#else
            return _expectedType.IsInstanceOfType(packet);
#endif
        }

        public virtual bool Finished()
        {
            return false;
        }
    }
}