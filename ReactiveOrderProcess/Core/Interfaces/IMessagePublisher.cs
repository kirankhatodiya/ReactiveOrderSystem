namespace ReactiveOrderProcess.Core.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishOrderCreatedAsync(int OrderId, string eventType);
    }
}
