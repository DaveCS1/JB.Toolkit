using System.ServiceModel;

/// <summary>
///It is binary;
// It is fast;
// Doesn’t need to open TCP sockets;
// Is optimized for same machine(actually, the WCF implementation only works this way, even though the named pipes protocol can be used across machines).
///
/// Gist from: https://weblogs.asp.net/ricardoperes/local-machine-interprocess-communication-with-net
/// </summary>
namespace JBToolkit.InterProcessComms.Wcf
{
    /// <summary>
    /// Client to host communication (1 direction) Named Pipe implementation.
    /// 
    /// Obivously the pipe name needs to be what you defined in your host / server.
    /// When implementing the 'Faulted' method, you need to actually recreate the WcfClient object to undo it's faulted state.
    /// </summary>
    public class WcfClient : ClientBase<IIpcClient>, IIpcClient
    {
        internal WcfClient(string pipeName) : base(
            new NetNamedPipeBinding(),
            new EndpointAddress(string.Format("net.pipe://localhost/{0}", pipeName)))
        {
        }

        internal WcfClient() : base(
            new NetNamedPipeBinding(),
            new EndpointAddress(string.Format("net.pipe://localhost/{0}", typeof(IIpcClient).Name)))
        {
        }

        public void Send(string data)
        {
            this.Channel.Send(data);
        }
    }
}