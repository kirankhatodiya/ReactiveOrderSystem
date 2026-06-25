namespace ReactiveOrderProcess.Infrastructure.Messaging
{
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public string EventType { get; set; }
    }
}
