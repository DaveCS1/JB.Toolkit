using System.IO;
using System.IO.Pipes;

/// <summary>
/// Named pipes in Windows is a duplex means of sending data between Windows hosts. We used it in the WCF implementation, 
/// but .NET has its own built-in support for named pipes communication.
/// 
/// Gist from: https://weblogs.asp.net/ricardoperes/local-machine-interprocess-communication-with-net
/// </summary>
namespace JBToolkit.InterProcessComms.NamedPipes
{
    public class NamedPipeClient : IIpcClient
    {
        string _pipename = typeof(IIpcClient).Name;

        public NamedPipeClient()
        { }

        public NamedPipeClient(string pipeName)
        {
            _pipename = pipeName;
        }

        public void Send(string data)
        {
            using (var client = new NamedPipeClientStream(".", _pipename, PipeDirection.Out))
            {
                client.Connect();

                using (var writer = new StreamWriter(client))
                {
                    writer.WriteLine(data);
                }
            }
        }
    }
}
