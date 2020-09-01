using System.Messaging;

/// <summary>
/// Windows has included a message queues implementation for a long time, something that is often forgotten by developers. 
/// If you don’t have it installed – you can check if the Message Queuing service exists – you can install it through Programs and Features 
/// – Turn Windows features on and off on the Control Panel.
/// 
/// Gist from: https://weblogs.asp.net/ricardoperes/local-machine-interprocess-communication-with-net
/// </summary>
namespace JBToolkit.InterProcessComms.MessageQueues
{
    public class MessageQueueClient : IIpcClient
    {
        string _queueName = typeof(IIpcClient).Name;

        public MessageQueueClient() { }

        public MessageQueueClient(string queueName)
        {
            _queueName = queueName;
        }

        public void Send(string data)
        {
            var name = string.Format(".\\Private$\\{0}", _queueName);

            MessageQueue queue;
            if (MessageQueue.Exists(name) == true)
            {
                queue = new MessageQueue(name);
            }
            else
            {
                queue = MessageQueue.Create(name);
            }

            using (queue)
            {
                queue.Send(data);
            }
        }
    }
}
