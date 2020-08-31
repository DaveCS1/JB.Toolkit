using System;
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
    /// Give the server an application specific unique name. When implementing the 'Faulted' method, you
    /// need to actually recreate the WcfServer object to undo it's faulted state.
    /// </summary>
    public sealed class WcfServer : IIpcServer
    {
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
        private class Server : IIpcClient
        {
            private readonly WcfServer server;

            public Server(WcfServer server)
            {
                this.server = server;
            }

            public void Send(string data)
            {
                this.server.OnReceived(new DataReceivedEventArgs(data));
            }
        }

        private readonly ServiceHost host;

        private void OnReceived(DataReceivedEventArgs e)
        {
            this.Received?.Invoke(this, e);
        }

        private void OnFaulted(object sender, EventArgs e)
        {
            this.Faulted?.Invoke(this, e);
        }

        internal WcfServer()
        {
            this.host = new ServiceHost(new Server(this), new Uri(string.Format("net.pipe://localhost/{0}", typeof(IIpcClient).Name)));
            this.host.IncrementManualFlowControlLimit(500);
            this.host.Faulted += OnFaulted;
        }

        internal WcfServer(string pipeName)
        {
            this.host = new ServiceHost(new Server(this), new Uri(string.Format("net.pipe://localhost/{0}", pipeName)));
            this.host.IncrementManualFlowControlLimit(500);
        }

        public event EventHandler<DataReceivedEventArgs> Received;
        public event EventHandler<EventArgs> Faulted;

        public void Start()
        {
            this.host.Open();
        }

        public void Stop()
        {
            this.host.Close();
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
        void IDisposable.Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            this.Stop();
            (this.host as IDisposable).Dispose();
        }
    }
}