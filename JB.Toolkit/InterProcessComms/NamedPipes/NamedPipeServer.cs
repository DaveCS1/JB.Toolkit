using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

/// <summary>
/// Named pipes in Windows is a duplex means of sending data between Windows hosts. We used it in the WCF implementation, 
/// but .NET has its own built-in support for named pipes communication.
/// 
/// Gist from: https://weblogs.asp.net/ricardoperes/local-machine-interprocess-communication-with-net
/// </summary>
namespace JBToolkit.InterProcessComms.NamedPipes
{
    public sealed class NamedPipeServer : IIpcServer
    {
        internal static string _pipename = typeof(IIpcClient).Name;

        public NamedPipeServer()
        { }

        public NamedPipeServer(string pipeName)
        {
            _pipename = pipeName;
        }

        private readonly NamedPipeServerStream server = new NamedPipeServerStream(_pipename, PipeDirection.In);

        private void OnReceived(DataReceivedEventArgs e)
        {
            this.Received?.Invoke(this, e);
        }

        public event EventHandler<DataReceivedEventArgs> Received;

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    this.server.WaitForConnection();

                    using (var reader = new StreamReader(this.server))
                    {
                        this.OnReceived(new DataReceivedEventArgs(reader.ReadToEnd()));
                    }
                }
            });
        }

        public void Stop()
        {
            this.server.Disconnect();
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
        void IDisposable.Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly

        {
            this.Stop();
            this.server.Dispose();
        }
    }
}
