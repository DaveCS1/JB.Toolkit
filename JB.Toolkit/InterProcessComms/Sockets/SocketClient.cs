using System.Net.Sockets;
using System.Text;

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
    public class SocketClient : IIpcClient
    {
        private int _port = 9000;
        public SocketClient() { }

        public SocketClient(int port)
        {
            _port = port;
        }

        public void Send(string data)
        {
            using (var client = new UdpClient())
            {
                client.Connect(string.Empty, _port);

                var bytes = Encoding.Default.GetBytes(data);

                client.Send(bytes, bytes.Length);
            }
        }
    }
}