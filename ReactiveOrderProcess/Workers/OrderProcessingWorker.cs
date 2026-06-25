using ReactiveOrderProcess.Constants;
using ReactiveOrderProcess.Core.Interfaces;
using ReactiveOrderProcess.Infrastructure.Messaging;

namespace ReactiveOrderProcess.Workers
{
    public class OrderProcessingWorker:BackgroundService
    {
        private readonly ILogger<OrderProcessingWorker> _logger;
        private readonly InMemoryChannel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public OrderProcessingWorker(ILogger<OrderProcessingWorker> logger,
            InMemoryChannel channel,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _channel = channel;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderProcessingWorker is starting.");
            try
            {
                await foreach (var orderEvent in _channel._channelReader.ReadAllAsync(stoppingToken))
                {
                    _logger.LogInformation(
                            "Received OrderCreatedEvent for OrderId: {OrderId} with event type '{EventType}'.",
                            orderEvent.OrderId,
                            orderEvent.EventType
                        );

                    _ = ProcessOrderAsync(orderEvent.OrderId, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Background OrderProcessingWorker is stopping due to cancellation request.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred in the background worker loop.");
            }
            
        }

        public async Task ProcessOrderAsync(int orderId, CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var orderRepo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                _logger.LogInformation("Fetching Order {OrderId} from SQLite database...", orderId);

                var order = await orderRepo.GetByIdAsync(orderId);
                if(order == null)
                {
                    _logger.LogWarning("Order with OrderId: {OrderId} not found in the database.", orderId);
                    return;
                }
                if (order.Status != "Pending")
                {
                    _logger.LogInformation("Order {OrderId} is already in '{Status}' state. Skipping processing.", orderId, order.Status);
                    return;
                }

                _logger.LogInformation("Simulating order processing delay (2 seconds) for Order {OrderId}...", orderId);
                
                // Simulate processing delay
                await Task.Delay(2000, cancellationToken);
               
                _logger.LogInformation("Updating Order {OrderId} status from 'Pending' to 'Processed'...", orderId);
                order.Status = OrderStatus.Processed.ToString();

                await orderRepo.UpdateAsync(order);

                _logger.LogInformation("Order {OrderId} processed and saved successfully.", orderId);

            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Processing of Order {OrderId} was cancelled during execution.", orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process Order {OrderId} in background.", orderId);
            }
        }
    }
}
