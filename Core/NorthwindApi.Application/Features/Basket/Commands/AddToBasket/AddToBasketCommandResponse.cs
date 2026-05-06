namespace NorthwindApi.Application.Features.Basket.Commands.AddToBasket
{
    public class AddToBasketCommandResponse
    {
        public string CustomerId { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }
}