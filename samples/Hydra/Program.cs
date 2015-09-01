using System.Collections;
using System.Text;
using Microsoft.SPOT;
using NETMF.OpenSource.XBee;
using NETMF.OpenSource.XBee.Api;
using NETMF.OpenSource.XBee.Api.Common;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.OpenSource;

namespace Hydra
{
    public partial class Program
    {
        private const string Ping = "PING";
        private const string Pong = "PONG";
        private ArrayList _nodes;

        void ProgramStarted()
        {
            coordinator.Configure();
            coordinator.Api.StatusChanged += OnStatusChanged;
            coordinator.Api.DataReceived += OnDataReceived;

            endDevice.Configure();
            endDevice.Api.StatusChanged += OnStatusChanged;
            endDevice.Api.DataReceived += OnDataReceived;

            router.Configure();
            router.Api.StatusChanged += OnStatusChanged;
            router.Api.DataReceived += OnDataReceived;

            joystick.JoystickPressed += (s, e) => led7r.TurnLightOn(7, true);
            joystick.JoystickReleased += (s, e) => DiscoverNodes();

            Debug.Print("Coordinator config: " + coordinator.Api.Config);
            Debug.Print("Router config: " + router.Api.Config);
            Debug.Print("End device config: " + endDevice.Api.Config);
        }

        private static void OnStatusChanged(XBeeApi sender, ModemStatus status)
        {
            switch (status)
            {
                case ModemStatus.HardwareReset:
                    Debug.Print(sender.Config.SerialNumber + " Hardware reset");
                    break;
                case ModemStatus.WatchdogTimerReset:
                    Debug.Print(sender.Config.SerialNumber + " Software reset");
                    break;
                case ModemStatus.Associated:
                    Debug.Print(sender.Config.SerialNumber + " Associated");
                    break;
                case ModemStatus.Disassociated:
                    Debug.Print(sender.Config.SerialNumber + " Not associated");
                    break;
                default:
                    Debug.Print(sender.Config.SerialNumber + " Status " + status);
                    break;
            }
        }

        private void DiscoverNodes()
        {
            _nodes = new ArrayList();

            router.Api.DiscoverNodes(node =>
            {
                var nodeInfo = node.NodeInfo;

                if (_nodes.Contains(nodeInfo))
                    return;

                _nodes.Add(nodeInfo);

                PrintNode(_nodes.Count, node);

                // depending on XBee settings discovery may return local node info
                if (nodeInfo.SerialNumber != router.Api.Config.SerialNumber)
                    router.Api.Send(Ping).To(nodeInfo).NoResponse();
            });
        }

        private void PrintNode(int nodeNumber, DiscoverResult info)
        {
            Debug.Print("#" + nodeNumber + " - " + info);
            led7r.TurnLightOn(nodeNumber);
        }

        private static void OnDataReceived(XBeeApi receiver, byte[] data, XBeeAddress sender)
        {
            var dataStr = new string(Encoding.UTF8.GetChars(data));

            switch (dataStr)
            {
                case Ping:
                    receiver.Send(Pong).To(sender).NoResponse();
                    break;

                case Pong:
                    Debug.Print("Received Pong from " + sender);
                    break;
            }
        }
    }
}
