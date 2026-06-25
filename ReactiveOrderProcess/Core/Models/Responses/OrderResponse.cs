namespace ReactiveOrderProcess.Core.Models.Responses
{
    public class OrderResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public int OrderId { get; set; }    
        public string Status { get; set; }
    }
}
