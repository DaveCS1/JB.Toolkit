using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Windows does not support the AF_UNIX family of sockets, only TCP/IP, so, for demonstrating IP network communication I could have chosen either TCP 
/// or UDP, but I went for UDP because of the better performance and because of the relative simplicity that this example required.
///
/// It leverages the UdpClient class to send byte-array messages, because this class hides some of the complexity of Socket, making it easier to use.
/// As a side note, it is even possible to use WCF with a UDP transport (binding).
///
/// Because the Receive method blocks until there is some contents to receive, we need to spawn a thread to avoid blocking.
/// 
/// Gist from: https://weblogs.asp.net/ricardoperes/local-machine-interprocess-communication-with-net
/// </summary>
namespace JBToolkit.InterProcessComms.Sockets
{
    public sealed class SocketServer : IIpcServer
    {
        private UdpClient server = new UdpClient(9000);

        public SocketServer() { }

        public SocketServer(int port)
        {
            server = new UdpClient(port);
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
        void IDisposable.Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            this.Stop();

            (this.server as IDisposable).Dispose();
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                var ip = new IPEndPoint(IPAddress.Any, 0);

                while (true)
                {
                    var bytes = this.server.Receive(ref ip);
                    var data = Encoding.Default.GetString(bytes);
                    this.OnReceived(new DataReceivedEventArgs(data));
                }
            });
        }

        private void OnReceived(DataReceivedEventArgs e)
        {
            this.Received?.Invoke(this, e);
        }


        public void Stop()
        {
            this.server.Close();
        }

        public event EventHandler<DataReceivedEventArgs> Received;
    }
}