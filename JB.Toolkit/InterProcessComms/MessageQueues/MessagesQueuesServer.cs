using System;
using System.Messaging;
using System.Threading.Tasks;

/// <summary>
/// Windows has included a message queues implementation for a long time, something that is often forgotten by developers. 
/// If you don’t have it installed – you can check if the Message Queuing service exists – you can install it through Programs and Features 
/// – Turn Windows features on and off on the Control Panel.
/// 
/// Gist from: https://weblogs.asp.net/ricardoperes/local-machine-interprocess-communication-with-net
/// </summary>
namespace JBToolkit.InterProcessComms.MessageQueues
{
    public sealed class MessageQueueServer : IIpcServer
    {
        string _queueName = typeof(IIpcClient).Name;

        public MessageQueueServer() { }

        public MessageQueueServer(string queueName)
        {
            _queueName = queueName;
        }

        private MessageQueue queue;

#pragma warning disable CA1063 // Implement IDisposable Correctly
        void IDisposable.Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            this.Stop();
            this.queue.Dispose();
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                var name = string.Format(".\\Private$\\{0}", _queueName);

                if (MessageQueue.Exists(name) == true)
                {
                    queue = new MessageQueue(name);
                }
                else
                {
                    queue = MessageQueue.Create(name);
                }

                queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                while (true)
                {
                    var msg = queue.Receive();
                    var data = msg.Body.ToString();
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
            this.queue.Close();
        }

        public event EventHandler<DataReceivedEventArgs> Received;
    }
}
