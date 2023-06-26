namespace OrderService.MessageBroker
{
    public interface IMessageBrokerClient
    {
        public void SendMessage<T>(T message, string eventType);


    }
}
