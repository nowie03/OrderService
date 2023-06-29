using OrderService.Models;

namespace OrderService.MessageBroker
{
    public interface IMessageBrokerClient
    {
        public void SendMessage(Message message);
        public ulong GetNextSequenceNUmber();

    }
}
