using Microsoft.SPOT;
using NETMF.OpenSource.XBee.Api.Common;
using Gadgeteer.Modules.OpenSource;

namespace Cerberus
{
    public partial class Program
    {
        void ProgramStarted()
        {
            Debug.Print("Program Started");

            xbee.DebugPrintEnabled = true;
            xbee.Api.DiscoverNodes(OnNodeDiscovered);
        }

        private static void OnNodeDiscovered(DiscoverResult node)
        {
            Debug.Print("Node discovered: " + node.NodeInfo);
        }
    }
}
