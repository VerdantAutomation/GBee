using System;
using System.Threading;
using Microsoft.SPOT.Hardware;
using NETMF.OpenSource.XBee;
using Logger = NETMF.OpenSource.XBee.Util.Logger;
using LogLevel = NETMF.OpenSource.XBee.Util.LogLevel;
using IXBeeConnection = NETMF.OpenSource.XBee.IXBeeConnection;
using SerialConnection = NETMF.OpenSource.XBee.SerialConnection;
using Gadgeteer.SocketInterfaces;

namespace Gadgeteer.Modules.OpenSource
{
    /// <summary>
    /// A XBee module for Microsoft .NET Gadgeteer
    /// </summary>
    public class XBee : Module
    {
        // It seems that the module is ready to work after aprox. 100 ms after the reset
        private const int StartupTime = 200;

        private XBeeApi _api;
        private IXBeeConnection _connection;
        private readonly string _serialPortName;
        private readonly bool _connectedToGadgeteerSocket;
        private readonly DigitalOutput _gadgeteerResetPin;
        private readonly DigitalOutput _gadgeteerSleepPin;
        private readonly OutputPort _netmfResetPin;
        private readonly OutputPort _netmfSleepPin;

        private static class ResetState
        {
            public const bool NotRunning = false;
            public const bool Running = true;
        }

        private static class SleepState
        {
            public const bool Awaken = false;
            public const bool Sleeping = true;
        }

        /// <summary>
        /// Gets the <see cref="XBeeApi"/> of the connected XBee module.
        /// </summary>
        public XBeeApi Api
        {
            get
            {
                if (_api == null)
                    Configure();

                return _api;
            }
        }

        /// <summary>
        /// Gets the <see cref="IXBeeConnection"/> associated with this instance.
        /// </summary>
        public IXBeeConnection SerialLine
        {
            get
            {
                if (_connection == null)
                    Configure();

                return _connection;
            }
        }

        /// <summary>
        /// Returns state of the module that is controlled by reset pin.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return _connectedToGadgeteerSocket
                    ? _gadgeteerResetPin.Read() == ResetState.Running
                    : _netmfResetPin.Read() == ResetState.Running;
            }
        }

        /// <summary>
        /// If the module is configured to work in PinSleep mode this value determines if it's asleep or not.
        /// </summary>
        public bool Sleeping
        {
            get
            {
                return _connectedToGadgeteerSocket
                    ? _gadgeteerSleepPin.Read() == SleepState.Sleeping
                    : _netmfSleepPin.Read() == SleepState.Sleeping ; 
            }
        }

        /// <summary>
        /// Use this constructor if you are connecting XBee using Gadgteteer socket.
        /// </summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        /// <remarks>
        /// The function <see cref="Configure"/> can be called to configure the <see cref="SerialLine"/> before it is used.
        /// If it is not called before first use, then the following defaults will be used and cannot be changed afterwards:
        /// <list type="bullet">
        ///  <item>Baud Rate - 9600</item>
        ///  <item>Parity - <see cref="System.IO.Ports.Parity">Parity.None</see></item>
        ///  <item>Stop Bits - <see cref="System.IO.Ports.StopBits">StopBits.One</see></item>
        ///  <item>Data Bits - 8</item>
        /// </list>
        /// </remarks>
        public XBee(int socketNumber)
        {
            _connectedToGadgeteerSocket = true;

            // This finds the Socket instance from the user-specified socket number.  
            // This will generate user-friendly error messages if the socket is invalid.
            // If there is more than one socket on this module, then instead of "null" for the last parameter, 
            // put text that identifies the socket to the user (e.g. "S" if there is a socket type S)
            var socket = Socket.GetSocket(socketNumber, true, this, null);
            socket.EnsureTypeIsSupported(new[] { 'K', 'U' }, this);

            _serialPortName = socket.SerialPortName;
            _gadgeteerResetPin = DigitalOutputFactory.Create(socket, Socket.Pin.Three, ResetState.NotRunning, this);
            _gadgeteerSleepPin = DigitalOutputFactory.Create(socket, Socket.Pin.Eight, SleepState.Awaken, this);
        }

        /// <summary>
        /// Use this constructor if you want to connect XBee to mainboard using raw pins, not Gadgeteer sockets.
        /// </summary>
        /// <remarks>
        /// Use this with Cerbuino Bee for example.
        /// </remarks>
        /// <param name="serialPortName">serial port name to use</param>
        /// <param name="resetPin">pin number that controls module reset</param>
        /// <param name="sleepPin">pin number that controls module sleep</param>
        public XBee(string serialPortName, Cpu.Pin resetPin, Cpu.Pin sleepPin)
        {
            _connectedToGadgeteerSocket = false;
            _serialPortName = serialPortName;
            _netmfResetPin = new OutputPort(resetPin, ResetState.NotRunning);
            _netmfSleepPin = new OutputPort(sleepPin, SleepState.Awaken);
        }

        /// <summary>
        /// Configures this serial line.
        /// </summary>
        /// <remarks>
        /// This should be called at most once.
        /// </remarks>
        /// <param name="baudRate">The baud rate.</param>
        public void Configure(int baudRate = 9600)
        {
            if (_api != null)
                throw new Exception("XBee.Configure can only be called once");

            Logger.Initialize(ErrorPrint, LogLevel.Error, LogLevel.Fatal);
            Logger.Initialize(DebugPrint, LogLevel.Warn, LogLevel.Info, LogLevel.Debug, LogLevel.LowDebug);

            _connection = new SerialConnection(_serialPortName, baudRate);
            _api = new XBeeApi(_connection);

            Enable();

            _api.Open();
        }

        /// <summary>
        /// Perform module hardware reset.
        /// </summary>
        public void Reset()
        {
            // reset pulse must be at least 200 ns
            // .net mf latency between calls is enough
            // no need to add any extra Thread.Sleep
            Disable();
            Enable();
        }

        /// <summary>
        /// Disables the module (power off).
        /// </summary>
        public void Disable()
        {
            if (_connectedToGadgeteerSocket)
            {
                _gadgeteerResetPin.Write(ResetState.NotRunning);   
            }
            else
            {
                _netmfResetPin.Write(ResetState.NotRunning);
            }
        }

        /// <summary>
        /// Enabled the module (power on).
        /// </summary>
        public void Enable()
        {
            if (_connectedToGadgeteerSocket)
            {
                _gadgeteerResetPin.Write(ResetState.Running);
            }
            else
            {
                _netmfResetPin.Write(ResetState.Running);
            }

            Thread.Sleep(StartupTime);
        }

        /// <summary>
        /// Sets the sleep control pin to active state (sleep request).
        /// </summary>
        public void Sleep()
        {
            if (_connectedToGadgeteerSocket)
            {
                _gadgeteerSleepPin.Write(SleepState.Sleeping);
            }
            else
            {
                _netmfSleepPin.Write(SleepState.Sleeping);
            }
        }

        /// <summary>
        /// Sets the sleep control pin to inactive state (no sleep request)
        /// </summary>
        public void Awake()
        {
            if (_connectedToGadgeteerSocket)
            {
                _gadgeteerSleepPin.Write(SleepState.Awaken);
            }
            else
            {
                _netmfSleepPin.Write(SleepState.Awaken);
            }
        }
    }
}
