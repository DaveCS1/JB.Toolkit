using System;

/// <summary>
/// .NET Remoting, in the old days, was .NET’s response to Java RMI, and basically was a remote references implementation, similar to CORBA. 
/// With Remoting, one can call methods on an object that resides in a different machine. .NET Remoting has long since been superseded by WCF,
/// but it is still a viable alternative, particularly because WCF does not allow remote references.
/// 
/// Gist from: https://weblogs.asp.net/ricardoperes/local-machine-interprocess-communication-with-net
/// </summary>
namespace JBToolkit.InterProcessComms.NetRemoting
{
    public sealed class RemoteObject : MarshalByRefObject, IIpcClient
    {
        private readonly IIpcClient svc;

        public RemoteObject()
        {
        }

        public RemoteObject(IIpcClient svc)
        {
            this.svc = svc;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        void IIpcClient.Send(string data)
        {
            if (this.svc != null)
            {
                this.svc.Send(data);
            }
        }
    }
}
