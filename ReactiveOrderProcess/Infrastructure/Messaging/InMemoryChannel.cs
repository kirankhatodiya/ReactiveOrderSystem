using ReactiveOrderProcess.Core.Interfaces;
using System.Threading.Channels;

namespace ReactiveOrderProcess.Infrastructure.Messaging
{
    public class InMemoryChannel:IMessagePublisher
    {
        private readonly Channel<OrderCreatedEvent> _channel;
        public InMemoryChannel()
        {
            _channel = Channel.CreateUnbounded<OrderCreatedEvent>(
                    new UnboundedChannelOptions
                    {
                        SingleReader = true,
                        SingleWriter = false
                    }
                );
        }

        public ChannelReader<OrderCreatedEvent> _channelReader => _channel.Reader;

        public async Task PublishOrderCreatedAsync(int OrderId, string eventType)
        {
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = OrderId,
                EventType = eventType
            };
            await _channel.Writer.WriteAsync(orderCreatedEvent);
        }
    }
}
