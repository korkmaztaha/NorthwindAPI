namespace NorthwindApi.Application.Features.Basket.Queries.GetBasket
{
    public class GetBasketQueryResponse
    {
        public string CustomerId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public List<BasketItemResponse> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class BasketItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
        public decimal LineTotal { get; set; }
    }
}