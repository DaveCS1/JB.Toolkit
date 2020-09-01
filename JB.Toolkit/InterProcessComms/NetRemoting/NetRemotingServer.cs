using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// .NET Remoting, in the old days, was .NET’s response to Java RMI, and basically was a remote references implementation, similar to CORBA. 
/// With Remoting, one can call methods on an object that resides in a different machine. .NET Remoting has long since been superseded by WCF,
/// but it is still a viable alternative, particularly because WCF does not allow remote references.
/// 
/// Gist from: https://weblogs.asp.net/ricardoperes/local-machine-interprocess-communication-with-net
/// </summary>
namespace JBToolkit.InterProcessComms.NetRemoting
{
    public sealed class RemotingServer : IIpcServer
    {
        private class Server : IIpcClient
        {
            private readonly RemotingServer server;

            public Server(RemotingServer server)
            {
                this.server = server;
            }

            public void Send(string data)
            {
                this.server.OnReceived(new DataReceivedEventArgs(data));
            }
        }

        private readonly ManualResetEvent killer = new ManualResetEvent(false);

        private static readonly IServerChannelSinkProvider serverSinkProvider = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                var properties = new Hashtable
                {
                    ["portName"] = typeof(IIpcClient).Name
                };
                var channel = new IpcChannel(properties, null, serverSinkProvider);

                try
                {
                    ChannelServices.RegisterChannel(channel, true);
                }
                catch
                {
                    //might be already registered, ignore it
                }

                var remoteObject = new RemoteObject(new Server(this));

                RemotingServices.Marshal(remoteObject, typeof(RemoteObject).Name + ".rem");

                this.killer.WaitOne();

                RemotingServices.Disconnect(remoteObject);

                try
                {
                    ChannelServices.UnregisterChannel(channel);
                }
                catch
                {
                }
            });
        }

        public void Stop()
        {
            this.killer.Set();
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
        void IDisposable.Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            this.Stop();

            this.killer.Dispose();
        }

        private void OnReceived(DataReceivedEventArgs e)
        {
            Received?.Invoke(this, e);
        }

        public event EventHandler<DataReceivedEventArgs> Received;
    }
}
