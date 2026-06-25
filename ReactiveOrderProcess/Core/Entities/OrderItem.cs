namespace ReactiveOrderProcess.Core.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public int Quantity {  get; set; }
        public decimal Price { get; set; }
    }
}
