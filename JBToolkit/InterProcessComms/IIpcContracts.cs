using System;
using System.ServiceModel;

/// <summary>
/// Gist from: https://weblogs.asp.net/ricardoperes/local-machine-interprocess-communication-with-netD
/// </summary>
namespace JBToolkit.InterProcessComms
{
    [ServiceContract]
    public interface IIpcClient
    {
        [OperationContract(IsOneWay = true)]
        void Send(string data);
    }

    public interface IIpcServer : IDisposable
    {
        void Start();
        void Stop();

        event EventHandler<DataReceivedEventArgs> Received;
    }

    [Serializable]
    public sealed class DataReceivedEventArgs : EventArgs
    {
        public DataReceivedEventArgs(string data)
        {
            this.Data = data;
        }

        public string Data { get; private set; }
    }
}
