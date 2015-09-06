using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace NETMF.OpenSource.XBee
{
    public class SerialConnection : IXBeeConnection, IDisposable
    {
        private readonly SerialDevice _serialDevice;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly IInputStream _input;
        private readonly IOutputStream _output;
        private readonly byte[] _buffer;
        private bool _isOpen = false;

        public SerialConnection(SerialDevice serialDevice)
        {
            _serialDevice = serialDevice;
            _input = _serialDevice.InputStream;
            _output = _serialDevice.OutputStream;
            _serialDevice.ErrorReceived += _serialDevice_ErrorReceived;
            _buffer = new byte[1024];
        }

        public void Dispose()
        {
            _isOpen = false;
            _cts.Cancel();
            _input.Dispose();
            _output.Dispose();
            _serialDevice.Dispose();
        }

        public void Open()
        {
            Task.Run(() => ReadLoop(_cts.Token));
            _isOpen = true;
        }

        private void _serialDevice_ErrorReceived(SerialDevice sender, ErrorReceivedEventArgs args)
        {
        }

        public void Close()
        {
            if (_serialDevice != null)
            {
                _serialDevice.ErrorReceived -= _serialDevice_ErrorReceived;
            }
            this.Dispose();
        }

        public bool Connected
        {
            get { return _isOpen; }
        }

        public void Send(byte[] data)
        {
            this.Send(data, 0, data.Length);
        }

        public void Send(byte[] data, int offset, int length)
        {
            var buffer = data.AsBuffer(offset, length);
            var cbSent = _output.WriteAsync(buffer).AsTask().Result;
        }

        private async void ReadLoop(CancellationToken ct)
        {
            _serialDevice.ReadTimeout = TimeSpan.FromSeconds(1.0);
            while (!ct.IsCancellationRequested)
            {
                var buffer = _buffer.AsBuffer();
                await _input.ReadAsync(buffer, (uint)_buffer.Length, InputStreamOptions.Partial);
                if (buffer.Length > 0)
                {
                    if (DataReceived != null)
                        DataReceived(_buffer, 0, (int)buffer.Length);
                }
            }
        }

        public event DataReceivedEventHandler DataReceived;
    }
}