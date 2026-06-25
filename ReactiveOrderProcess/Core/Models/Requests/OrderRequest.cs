namespace ReactiveOrderProcess.Core.Models.Requests
{
    public class OrderRequest
    {
        public string CustomerName { get; set; }
        public List<ProductItems> ProductItems { get; set; }
    }

    public class ProductItems
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
