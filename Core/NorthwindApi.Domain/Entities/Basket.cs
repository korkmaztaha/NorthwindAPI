using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Domain.Entities
{
    public class Basket
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string CustomerId { get; set; } = null!;

        public string CompanyName { get; set; } = null!;

        public List<BasketItem> Items { get; set; } = new();

        public decimal TotalAmount => Items.Sum(x => x.LineTotal);

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class BasketItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
        public decimal LineTotal => (decimal)(Quantity * UnitPrice * (decimal)(1 - Discount));
    }
}
