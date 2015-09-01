using GHI.OSHW.Hardware;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using NETMF.OpenSource.XBee.Api.Common;
using Gadgeteer.Modules.OpenSource;

namespace CerbuinoBee
{
    public partial class Program
    {
        void ProgramStarted()
        {
            Debug.Print("Program Started");

            const string serialPortName = "COM1";
            const Cpu.Pin resetPin = FEZCerberus.Pin.PB0;
            const Cpu.Pin sleepPin = FEZCerberus.Pin.PC13;

            xbee = new XBee(serialPortName, resetPin, sleepPin) {DebugPrintEnabled = true};

            xbee.Api.DiscoverNodes(OnNodeDiscovered);
        }

        private static void OnNodeDiscovered(DiscoverResult node)
        {
            Debug.Print("Node discovered: " + node.NodeInfo);
        }
    }
}
