using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;

/// <summary>
/// .NET Remoting, in the old days, was .NET’s response to Java RMI, and basically was a remote references implementation, similar to CORBA. 
/// With Remoting, one can call methods on an object that resides in a different machine. .NET Remoting has long since been superseded by WCF,
/// but it is still a viable alternative, particularly because WCF does not allow remote references.
/// 
/// Gist from: https://weblogs.asp.net/ricardoperes/local-machine-interprocess-communication-with-net
/// </summary>
namespace JBToolkit.InterProcessComms.NetRemoting
{
    public class RemotingClient : IIpcClient
    {
        private static readonly IServerChannelSinkProvider serverSinkProvider = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };

        public void Send(string data)
        {
            var properties = new Hashtable
            {
                ["portName"] = Guid.NewGuid().ToString(),
                ["exclusiveAddressUse"] = false
            };

            var channel = new IpcChannel(properties, null, serverSinkProvider);

            try
            {
                ChannelServices.RegisterChannel(channel, true);
            }
            catch
            {
                //the channel might be already registered, so ignore it
            }

            var uri = string.Format("ipc://{0}/{1}.rem", typeof(IIpcClient).Name, typeof(RemoteObject).Name);
            var svc = Activator.GetObject(typeof(RemoteObject), uri) as IIpcClient;

            svc.Send(data);

            try
            {
                ChannelServices.UnregisterChannel(channel);
            }
            catch
            {
            }
        }
    }
}
